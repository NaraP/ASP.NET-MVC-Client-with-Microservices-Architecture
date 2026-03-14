using ECommerce.AuthApi.Repository;
using ECommerce.AuthApi.Services;
using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using Moq;

namespace TestProject.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IAuthApiService> _authApiMock;
        private readonly Mock<IUserRepository> _userRepository; 
        private readonly Mock<IRoleRepository> _roleRepository;

        private readonly UserService _service;
        public UserServiceTests()
        {
            _authApiMock = new Mock<IAuthApiService>();
            _userRepository = new Mock<IUserRepository>();
            _roleRepository = new Mock<IRoleRepository>();
            _service = new UserService(_userRepository.Object ,_roleRepository.Object);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ReturnsNull_WhenLoginFails()
        {
            _authApiMock
                .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((AuthApiResponse?)null);

            var result = await _service.ValidateCredentialsAsync("a", "b");

            Assert.Null(result);
        }

        [Fact]
        public async Task EmailExistsAsync_ReturnsFalse_WhenEmailAvailable()
        {
            _authApiMock
                .Setup(x => x.IsEmailAvailableAsync("test@test.com"))
                .ReturnsAsync(true);

            var result = await _service.EmailExistsAsync("test@test.com");

            Assert.False(result);
        }

    }
}
