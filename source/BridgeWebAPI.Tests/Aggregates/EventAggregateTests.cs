using System;
using System.Linq;
using Bridge.Domain.EventAggregate;
using Bridge.Domain.EventAggregate.DomainEvents;
using Bridge.WebAPI.Modules;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.WebAPI.Tests.Aggregates
{
    [TestClass]
    public class EventAggregateTests
    {
        [TestMethod]
        public void TestDomainEventsAreRegistered()
        {
            //Arrange
            var aggregate = ServiceLocator.Current.GetInstance<EventAggregate>();
            var module = ServiceLocator.Current.GetInstance<ExtractEventMetadataModule>();
            var command = module.ExtractEventMetadata(new DateTime(2015, 7, 14));
            //Act
            aggregate.ImportEvent(command);
            //Assert
            Assert.IsTrue(aggregate.DomainEvents.Any(a => a is EventImported));
        }
    }
}
