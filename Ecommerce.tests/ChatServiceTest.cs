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
    public class ChatServiceTests
    {
        private Mock<IChatRepository> _repositoryMock = null!;
        private ChatService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IChatRepository>();
            _service = new ChatService(_repositoryMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllChats()
        {
            var buyer1 = Guid.NewGuid();
            var seller1 = Guid.NewGuid();
            var buyer2 = Guid.NewGuid();
            var seller2 = Guid.NewGuid();

            var chats = new List<Chat>
            {
                new Chat { Id = Guid.NewGuid(), BuyerId = buyer1, SellerId = seller1 },
                new Chat { Id = Guid.NewGuid(), BuyerId = buyer2, SellerId = seller2 }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(chats);

            var result = (await _service.GetAllAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].BuyerId, Is.EqualTo(buyer1));
            Assert.That(result[1].SellerId, Is.EqualTo(seller2));

            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnChat_WhenExists()
        {
            var buyerId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();
            var chat = new Chat { Id = Guid.NewGuid(), BuyerId = buyerId, SellerId = sellerId };

            _repositoryMock.Setup(r => r.GetByIdAsync(chat.Id)).ReturnsAsync(chat);

            var result = await _service.GetByIdAsync(chat.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.BuyerId, Is.EqualTo(buyerId));
            Assert.That(result.SellerId, Is.EqualTo(sellerId));

            _repositoryMock.Verify(r => r.GetByIdAsync(chat.Id), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var chatId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(chatId)).ReturnsAsync((Chat?)null);

            var result = await _service.GetByIdAsync(chatId);

            Assert.That(result, Is.Null);
            _repositoryMock.Verify(r => r.GetByIdAsync(chatId), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldReturnCreatedChat()
        {
            var buyerId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();
            var chat = new Chat { Id = Guid.NewGuid(), BuyerId = buyerId, SellerId = sellerId };

            _repositoryMock.Setup(r => r.AddAsync(chat)).ReturnsAsync(chat);

            var result = await _service.CreateAsync(chat);

            Assert.That(result, Is.EqualTo(chat));
            Assert.That(result.BuyerId, Is.EqualTo(buyerId));
            Assert.That(result.SellerId, Is.EqualTo(sellerId));

            _repositoryMock.Verify(r => r.AddAsync(chat), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            var chatId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(chatId)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(chatId);

            Assert.That(result, Is.True);
            _repositoryMock.Verify(r => r.DeleteAsync(chatId), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            var chatId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(chatId)).ReturnsAsync(false);

            var result = await _service.DeleteAsync(chatId);

            Assert.That(result, Is.False);
            _repositoryMock.Verify(r => r.DeleteAsync(chatId), Times.Once);
        }
    }
}
