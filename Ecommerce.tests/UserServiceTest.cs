using Identity.DTO;
using Identity.Models;
using Identity.Models.Query;
using Identity.Repository;
using Identity.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Events;
using MockQueryable.Moq;
namespace Ecommerce.tests
{
    internal class UserServiceTest
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<ILogger<UserService>> _loggerMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IPublishEndpoint> _publishEndpointMock;
        private Mock<IConfiguration> _configurationMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _configurationMock = new Mock<IConfiguration>();

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["SecretKey"]).Returns("SuperSecretKeyForJWTTokenGeneration12345");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("YourIssuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("YourAudience");
            jwtSectionMock.Setup(x => x["ExpiryInMinutes"]).Returns("60");

            _configurationMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            var userStoreMock = new Mock<IUserStore<User>>();
            var optionsAccessorMock = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            var passwordHasherForUserManager = new Mock<IPasswordHasher<User>>();
            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var userManagerLogger = new Mock<ILogger<UserManager<User>>>();

            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object,
                optionsAccessorMock.Object,
                passwordHasherForUserManager.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                userManagerLogger.Object
            );

            _userService = new UserService(
                _userRepositoryMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object,
                _tokenServiceMock.Object,
                _publishEndpointMock.Object
            );
        }

        [Test]
        public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var userId = "user123";
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                Nombre = "Test",
                Apellido = "User"
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userId));
            Assert.That(result.Email, Is.EqualTo("test@example.com"));
            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var userId = "nonexistent";
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.That(result, Is.Null);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_ShouldCreateUserAndPublishEvent()
        {
            // Arrange
            var request = new CreateUserRequest
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                Nombre = "New",
                Apellido = "User",
                Dni = "12345678",
                Direccion = "Test Address",
                Telefono = 123456789
            };

            var createdUser = new User
            {
                Id = "newUserId",
                UserName = request.UserName,
                Email = request.Email,
                Nombre = request.Nombre,
                Apellido = request.Apellido
            };

            _userRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.CreateUserAsync(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("newUserId"));
            Assert.That(result.Email, Is.EqualTo(request.Email));

            _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), request.Password), Times.Once);
            _publishEndpointMock.Verify(x => x.Publish(
                It.Is<UserRegisteredEvent>(e =>
                    e.UserId == createdUser.Id &&
                    e.Email == createdUser.Email),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Test]
        public Task CreateUserAsync_ShouldFail_WhenUserRepositoryThrowsExceptionForDuplicateEmail()
        {
            var request = new CreateUserRequest
            {
                UserName = "newuser",
                Email = "existing@test.com",
                Password = "Password123!",
                Nombre = "Test",
                Apellido = "User"
            };

            _userRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ThrowsAsync(new Exception("El correo electrónico ya está en uso."));

            var ex = Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(request));
            Assert.That(ex!.Message, Is.EqualTo("El correo electrónico ya está en uso."));

            _publishEndpointMock.Verify(
                x => x.Publish(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);

            return Task.CompletedTask;
        }

        [Test]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
        {
            // Arrange
            var user = new User
            {
                Id = "user123",
                UserName = "testuser",
                Email = "test@example.com",
                Nombre = "Test",
                Apellido = "User"
            };

            var roles = new List<string> { "User" };
            var expectedToken = "jwt-token-here";

            _userManagerMock
                .Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, "password123"))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            _tokenServiceMock
                .Setup(x => x.GenerateJwtToken(user, roles))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _userService.LoginAsync("testuser", "password123");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Token, Is.EqualTo(expectedToken));
            Assert.That(result.User.Id, Is.EqualTo(user.Id));
            Assert.That(result.User.Email, Is.EqualTo(user.Email));
        }

        [Test]
        public async Task LoginAsync_WithInvalidCredentials_ShouldReturnNull()
        {
            // Arrange
            var user = new User
            {
                Id = "user123",
                UserName = "testuser",
                Email = "test@example.com"
            };

            _userManagerMock
                .Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, "wrongpassword"))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.LoginAsync("testuser", "wrongpassword");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteUserAsync_WhenUserExists_ShouldReturnTrue()
        {
            // Arrange
            var userId = "user123";
            _userRepositoryMock
                .Setup(x => x.DeleteAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result, Is.True);
            _userRepositoryMock.Verify(x => x.DeleteAsync(userId), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var userId = "nonexistent";
            _userRepositoryMock
                .Setup(x => x.DeleteAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.That(result, Is.False);
            _userRepositoryMock.Verify(x => x.DeleteAsync(userId), Times.Once);
        }
       



        [TearDown]
        public void TearDown()
        {
            // Limpiar recursos si es necesario
        }
    }
}