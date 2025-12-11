using Catalog.Models;
using Catalog.Repositories;
using Catalog.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Tests
{
    public class MessageServiceTests
    {
        private Mock<IMessageRepository> _repositoryMock = null!;
        private MessageService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IMessageRepository>();
            _service = new MessageService(_repositoryMock.Object);
        }

        [Test]
        public async Task GetByChatIdAsync_ShouldReturnMessagesForChat()
        {
            var chatId = Guid.NewGuid();
            var messages = new List<Message>
            {
                new Message { Id = Guid.NewGuid(), ChatId = chatId, SenderId = Guid.NewGuid(), Content = "Hello" },
                new Message { Id = Guid.NewGuid(), ChatId = chatId, SenderId = Guid.NewGuid(), Content = "World" }
            };

            _repositoryMock.Setup(r => r.GetMessagesByChatIdAsync(chatId)).ReturnsAsync(messages);

            var result = (await _service.GetByChatIdAsync(chatId)).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].ChatId, Is.EqualTo(chatId));
            Assert.That(result[1].ChatId, Is.EqualTo(chatId));

            _repositoryMock.Verify(r => r.GetMessagesByChatIdAsync(chatId), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldReturnCreatedMessage_WhenContentValid()
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                Content = "Test message"
            };

            _repositoryMock.Setup(r => r.AddAsync(message)).ReturnsAsync(message);

            var result = await _service.CreateAsync(message);

            Assert.That(result, Is.EqualTo(message));
            Assert.That(result.Content, Is.EqualTo("Test message"));

            _repositoryMock.Verify(r => r.AddAsync(message), Times.Once);
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenContentEmpty()
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                Content = "   " // Vacío o espacios
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(message));
            Assert.That(ex!.Message, Is.EqualTo("El mensaje no puede estar vacío."));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Never);
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenContentNull()
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                Content = null! // Nulo
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(message));
            Assert.That(ex!.Message, Is.EqualTo("El mensaje no puede estar vacío."));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Never);
        }
    }
}
