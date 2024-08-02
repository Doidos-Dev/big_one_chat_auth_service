using Application.Services;
using Domain.Models;
using Moq;
using Domain.Interfaces;
using Application.Utils.Auth.Interfaces;
using Auth.UnitTest.Application.Fixture;
using System.Security.Claims;


namespace AuthUnitTest.Application
{
    public class AuthServiceTests
    {
        private static readonly Mock<ITokenRepository> _tokenRepositoryMock = new Mock<ITokenRepository>();
        private static readonly Mock<ITokensGenerator> _tokenGeneratorMock = new Mock<ITokensGenerator>();
        private static string _secret = "3c8727e019a42b444667a587b6001251becadabbb36bfed8087a92c18882d111";
        private UserModel _userModel;
        private TokenModel _tokenModel;

        public AuthServiceTests()
        {
            _userModel = new UserModel()
            {
                Nickname = "TestNickname",
                Role = "TestRole"
            };

            _tokenModel = new TokenModel()
            {
                Nickname = "TestNickname",
                Token = "TestToken",
                RefreshToken = "TestRefreshToken",
            };
        }

        [Fact]
        public async void Generate_Token_Success()
        {
            //Arrange
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel))
                .Returns("TestToken");
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns("TestRefreshToken");
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
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).
                Returns("238128jkj1k31m2k3km12km");
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.GenerateToken(_userModel);

            //Assert
            Assert.NotNull(result.Response.Token);
            Assert.NotNull(result.Response.RefreshToken);
        }

        [Fact]
        public async void Refresh_Token_Success()
        {
            //Arrange

            var oldTokenModel = new TokenModel()
            {
                Nickname = "TestNickname",
                Token = JWTTokenFixture.GenerateToken(_userModel, _secret),
                RefreshToken = JWTTokenFixture.GenerateRefreshToken()
            };
            var newTokenModel = new TokenModel()
            {
                Nickname = _userModel.Nickname,
                Token = "NewToken",
                RefreshToken = "NewRefreshToken"
            };
            _tokenRepositoryMock.Setup(x => x.GetByNickname(oldTokenModel.Nickname)).Returns(Task.FromResult(oldTokenModel));
            _tokenGeneratorMock.Setup(x => x.GetClaimsPrincipal(oldTokenModel.Token)).Returns(ClaimsFixture.GenerateClaimsPrincipal(oldTokenModel.Token, _secret));
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel)).Returns(newTokenModel.Token);
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(newTokenModel.RefreshToken);
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //act
            var result = await authService.RefreshToken(oldTokenModel);

            //Assert
            Assert.NotEqual(result.Response.Token, _tokenModel.Token);
            Assert.NotEqual(result.Response.RefreshToken, _tokenModel.RefreshToken);
            
        }

        [Fact]
        public async void Refresh_Token_Old_Tolken_Is_Not_Found_Fail()
        {
            //Arrange
            var oldTokenModel = new TokenModel()
            {
                Nickname = "TestNickname"
            };
            _tokenRepositoryMock.Setup(x => x.GetByNickname(oldTokenModel.Nickname)).Returns(Task.FromResult((TokenModel)null));
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.RefreshToken(oldTokenModel);

            //Assert
            Assert.Equal(result.CodeResponse, 404);
            Assert.NotNull(result.Message);

        }

        [Fact]
        public async void Refresh_Token_Invalid_Access_Or_Refresh_Token_Fail()
        {
            //Arrange
            var requestTokenModel = new TokenModel()
            {
                Nickname = "TestNickname",
                Token = JWTTokenFixture.GenerateToken(_userModel, _secret),
                RefreshToken = JWTTokenFixture.GenerateRefreshToken()
            };
            var oldTokenModelFetched = new TokenModel()
            {
                Nickname = "TestNickname",
                Token = JWTTokenFixture.GenerateToken(_userModel, _secret),
                RefreshToken = JWTTokenFixture.GenerateRefreshToken()
            };
            _tokenRepositoryMock.Setup(x => x.GetByNickname(requestTokenModel.Nickname)).Returns(Task.FromResult(oldTokenModelFetched));
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.RefreshToken(requestTokenModel);

            //Assert
            Assert.Equal(result.CodeResponse, 400);
            Assert.NotNull(result.Message);

        }

        [Fact]
        public async void Refresh_Token_Invalid_Token_Claims_Principal_Return_Null_Fail()
        {
            //Arrange
            var requestTokenModel = new TokenModel()
            {
                Nickname = "TestNickname",
                Token = JWTTokenFixture.GenerateToken(_userModel, _secret),
                RefreshToken = JWTTokenFixture.GenerateRefreshToken()
            };

            _tokenRepositoryMock.Setup(x => x.GetByNickname(requestTokenModel.Nickname)).Returns(Task.FromResult(requestTokenModel));
            _tokenGeneratorMock.Setup(x => x.GetClaimsPrincipal(requestTokenModel.Token)).Returns((ClaimsPrincipal)null);
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.RefreshToken(requestTokenModel);

            //Assert
            Assert.Equal(result.CodeResponse, 400);
            Assert.NotNull(result.Message);
        }


    }
}
