using System;
using System.Linq;
using Bridge.Domain.EventAggregate;
using Bridge.Domain.EventAggregate.DomainEvents;
using Bridge.WebAPI.Modules;
using Bridge.WebAPI.Tests.Helpers;
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

            using (var scope = TransactionHelpers.GetTransactionScope())
            {
                //Act
                aggregate.ImportEvent(command);

                var successEvent = aggregate.DomainEvents.FirstOrDefault();

                //Assert
                Assert.IsNotNull(successEvent);
                Assert.IsTrue(successEvent is EventImported);

                var importedEvent = successEvent as EventImported;
                Assert.AreEqual(26, importedEvent.DealIds.Count());
                Assert.AreEqual(26, importedEvent.PairIds.Count());

                scope.Dispose();
            }
        }
    }
}
