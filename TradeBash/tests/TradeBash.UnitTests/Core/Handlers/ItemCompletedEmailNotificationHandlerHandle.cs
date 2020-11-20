using System;
using System.Threading.Tasks;
using Moq;
using TradeBash.Core.Entities;
using TradeBash.Core.Events;
using TradeBash.Core.Handlers;
using TradeBash.Core.Interfaces;
using Xunit;

namespace TradeBash.UnitTests.Core.Handlers
{
    public class ItemCompletedEmailNotificationHandlerHandle
    {
        private ItemCompletedEmailNotificationHandler _handler;
        private Mock<IEmailSender> _emailSenderMock;

        public ItemCompletedEmailNotificationHandlerHandle()
        {
            _emailSenderMock = new Mock<IEmailSender>();
            _handler = new ItemCompletedEmailNotificationHandler(_emailSenderMock.Object);
        }

        [Fact]
        public async Task ThrowsExceptionGivenNullEventArgument()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null));
        }

        [Fact]
        public async Task SendsEmailGivenEventInstance()
        {
            await _handler.Handle(new ToDoItemCompletedEvent(new ToDoItem()));

            _emailSenderMock.Verify(sender => sender.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}