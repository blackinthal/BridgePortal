// Copyright 2012,2013 Vaughn Vernon
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Contracts;
using ElmahExtensions;
using Microsoft.Practices.ServiceLocation;

namespace Domain.Common
{
    public sealed class DomainEventPublisher
    {
        static DomainEventPublisher _instance;

        public static DomainEventPublisher Instance
        {
            get { return _instance ?? (_instance = new DomainEventPublisher()); }
        }

        DomainEventPublisher()
        {
            Subscribers = new ConcurrentList<IDomainEventSubscriber<IDomainEvent>>();
        }

        ConcurrentList<IDomainEventSubscriber<IDomainEvent>> Subscribers { get; set; }

        public void Publish<T>(T domainEvent) where T : class, IDomainEvent
        {
            try
            {
                if (HasSubscribers())
                {
                    var eventType = domainEvent.GetType();
                    var enumerator = Subscribers.GetEnumerator();

                    while(enumerator.MoveNext())
                    {
                        var subscribedToType = enumerator.Current.SubscribedToEventType();
                        if (eventType == subscribedToType || subscribedToType == typeof(IDomainEvent))
                        {
                            enumerator.Current.HandleEvent(domainEvent);
                        }
                    }
                }

                var ds = HasDomainSubscribers(domainEvent);

                if (!ds.Any()) return;

                foreach (var service in ds)
                {
                    ((dynamic)service).ApplyEvent(((dynamic)domainEvent));

                }
            }
            catch (Exception ex)
            {
                CustomErrorSignal.Handle(ex);
            }
        }

        public void PublishAll(ICollection<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                Publish(domainEvent);
            }
        }

        public void Reset()
        {
            Subscribers.Clear();
        }

        public T Subscribe<T>(T subscriber)
            where T : IDomainEventSubscriber<IDomainEvent>
        {

            Subscribers.Add(subscriber);
            return subscriber;
        }

        public bool UnSubscribe<T>(T subscriber)
            where T : IDomainEventSubscriber<IDomainEvent>
        {

            return Subscribers.Remove(subscriber);


        }

        public DomainEventSubscriber<IDomainEvent> Subscribe(Action<IDomainEvent> handle)
        {
            return Subscribe(new DomainEventSubscriber<IDomainEvent>(handle));
        }

        public class DomainEventSubscriber<TEvent> : IDomainEventSubscriber<TEvent>
            where TEvent : IDomainEvent
        {
            public DomainEventSubscriber(Action<TEvent> handle)
            {
                this.handle = handle;
            }

            readonly Action<TEvent> handle;

            public void HandleEvent(TEvent domainEvent)
            {
                handle(domainEvent);
            }

            public Type SubscribedToEventType()
            {
                return typeof(TEvent);
            }
        }

        bool HasSubscribers()
        {
            return Subscribers.Any();
        }

        IEnumerable<object> HasDomainSubscribers<TDomainEvent>(TDomainEvent @event)
            where TDomainEvent : class, IDomainEvent
        {
            var type = @event.GetType();

            var type2 = typeof(IApplyEvent<>).MakeGenericType(type);

            var subscriberInstances = ServiceLocator.Current.GetAllInstances(type2);

            var result = subscriberInstances;

            return result;
        }

        public DomainEventListener WaitFor<TEvent>(Predicate<IDomainEvent> predicate = null)
             where TEvent : IDomainEvent
        {
            var listner = new DomainEventListener();
            Subscribe(e => listner.ForEvent<TEvent>(e, predicate));

            return listner;
        }
    }

    public sealed class DomainEventListener : IDisposable
    {
        private readonly Guid? _processId;
        private IDomainEvent _listenFor;
        public CancellationTokenSource CancellationToken;
        public readonly Task<IDomainEvent> For;

        public DomainEventListener(Guid? processId = null, CancellationTokenSource cancel = null)
        {
            _processId = processId;
            CancellationToken = cancel ?? new CancellationTokenSource(TimeSpan.FromSeconds(10));

            For = Task.Factory.StartNew(
               () =>
               {

                   while (!CancellationToken.IsCancellationRequested) { }

                   return _listenFor;

               }
               , CancellationToken.Token);

        }


        public void ForEvent<TEvent>(IDomainEvent domainEvent, Predicate<IDomainEvent> predicate = null)
            where TEvent : IDomainEvent
        {
            if (predicate == null && _processId != null)
                predicate = @event => @event.ProcessId == _processId;

            if (
                domainEvent as IDomainEventError != null &&
                (predicate == null || predicate(domainEvent)))
            {
                _listenFor = domainEvent;
                CancellationToken.Cancel();
                return;
            }

            if (
                CancellationToken.IsCancellationRequested ||
                !(domainEvent is TEvent) ||
                (predicate != null && !predicate(domainEvent))
                ) return;

            _listenFor = domainEvent;
            CancellationToken.Cancel();


        }

        public void StopListening()
        {
            CancellationToken.Cancel();
        }

        public IDomainEvent ForResult(double miliseconds = 10000)
        {
            CancellationToken.CancelAfter(TimeSpan.FromMilliseconds(miliseconds));

            try
            {
                For.Wait();
            }
            catch (Exception ex)
            {
                CustomErrorSignal.Handle(ex);
            }

            return _listenFor;
        }

        public void Dispose()
        {
            StopListening();
        }
    }

    public interface IDomainEventSubscriber<in T> where T : IDomainEvent
    {
        void HandleEvent(T domainEvent);
        Type SubscribedToEventType();
    }

    public class DomainEventSubscriberForProcess : IDomainEventSubscriber<IDomainEvent>
    {
        public List<IDomainEvent> DomainEvents { get; private set; }
        public Guid ProcessId { get; set; }
        public DomainEventSubscriberForProcess(Guid processId)
        {
            DomainEvents = new List<IDomainEvent>();
            handle = DomainEvents.Add;
            ProcessId = processId;
        }

        readonly Action<IDomainEvent> handle;

        public void HandleEvent(IDomainEvent domainEvent)
        {
            if (domainEvent.ProcessId == ProcessId)
                handle(domainEvent);
        }

        public Type SubscribedToEventType()
        {
            return typeof(IDomainEvent);
        }
    }

}
