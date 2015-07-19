using System;
using Domain.Contracts;
using ElmahExtensions;

namespace Domain.Extensions
{
    public class DomainEventExtensions
    {
        public static void PrepareSuccessfulEvent(IDomainEvent successEvent, CommandBase command)
        {
            successEvent.EventId = command.Id;
            successEvent.ProcessId = command.ProcessId;
            successEvent.OccurredOn = DateTime.Now;
            successEvent.UserId = command.UserId;
        }

        public static void PrepareAttemptedEvent(IDomainEventError errorEvent, Exception ex, CommandBase command)
        {
            errorEvent.EventId = command.Id;
            errorEvent.ProcessId = command.ProcessId;
            errorEvent.OccurredOn = DateTime.Now;
            errorEvent.UserId = command.UserId;
            errorEvent.Errors = GetErrorFromException(ex);

            var error = CustomErrorSignal.Handle(ex);
            errorEvent.ErrorId = new Guid(error.Id);
        }

        private static string GetErrorFromException(Exception ex)
        {
            return ex.Message;
        }
    }
}
