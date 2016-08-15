using System;
using System.Linq;
using System.Threading.Tasks;
using Bridge.Domain.EventAggregate;
using Bridge.Domain.EventAggregate.DomainEvents;
using Bridge.WebAPI.Contracts;
using Bridge.WebAPI.Tests.Helpers;
using Domain.Contracts;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.WebAPI.Tests.Aggregates
{
    [TestClass]
    public class EventAggregateTests
    {
        [TestMethod]
        public async Task TestDomainEventsAreRegistered()
        {
            //Arrange
            var aggregate = ServiceLocator.Current.GetInstance<EventAggregate>();
            var module = ServiceLocator.Current.GetInstance<IExtractEventMetadataService>();
            var command = await module.ExtractEventMetadata(new DateTime(2015, 7, 14));

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

        [TestMethod]
        public async Task TestCommandProcessor()
        {
            //Arrange
            var commandProcessor = ServiceLocator.Current.GetInstance<ICommandProcessor>();
            var module = ServiceLocator.Current.GetInstance<IExtractEventMetadataService>();
            var command = await module.ExtractEventMetadata(new DateTime(2015, 7, 14));
            //Act
            commandProcessor.Process(command);

            var successEvent = commandProcessor.DomainEvents.FirstOrDefault();

            //Assert
            Assert.IsNotNull(successEvent);
            Assert.IsTrue(successEvent is EventImported);

            var importedEvent = successEvent as EventImported;
            Assert.AreEqual(26, importedEvent.DealIds.Count());
            Assert.AreEqual(26, importedEvent.PairIds.Count());
        }
    }
}
