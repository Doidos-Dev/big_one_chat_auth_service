using Application.Services;
using Data.Persistence;
using Auth.UnitTest.Data.Helper.Database;
using Domain.Models;
using Moq;
using Domain.Interfaces;
using Application.Utils.Auth.Interfaces;


namespace AuthUnitTest.Application
{
    public class AuthServiceTests : IDisposable
    {
        private static readonly DatabaseContext _context = DatabaseUtils.CreateDbContext("AuthTestDb");
        private static readonly Mock<ITokenRepository> _tokenRepositoryMock = new Mock<ITokenRepository>();
        private static readonly Mock<ITokensGenerator> _tokenGeneratorMock = new Mock<ITokensGenerator>();
        private UserModel _userModel;
        public AuthServiceTests()
        {
            _userModel = new UserModel()
            {
                Nickname = "TestNickname",
                Role = "TestRole"
            };

        }

        public void Dispose()
        {
            DatabaseUtils.ClearDatabase(_context);
        }

        [Fact]
        public async void Generate_Token_Success()
        {
            //Arrange
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel))
                .Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns("238128jkj1k31m2k3km12km");
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.GenerateToken(_userModel);

            //Assert
            Assert.NotNull(result.Response);
        }

        [Fact]
        public async void Generate_Token_With_Refresh_Token_And_Access_Token()
        {
            //Arrange
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel))
                .Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns("238128jkj1k31m2k3km12km");
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.GenerateToken(_userModel);

            //Assert
            Assert.NotNull(result.Response.Token);
            Assert.NotNull(result.Response.RefreshToken);
        }

    }
}
