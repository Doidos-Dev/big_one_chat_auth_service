using Application.Services;
using Application.Utils.Auth.Interfaces;
using Auth.UnitTest.Application.Fixture;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using Moq;


namespace Auth.UnitTest.Application
{
    public class AuthServiceTokenGeneratorTests
    {
        private static readonly Mock<ITokenRepository> _tokenRepositoryMock = new Mock<ITokenRepository>();
        private static readonly Mock<ITokensGenerator> _tokenGeneratorMock = new Mock<ITokensGenerator>();
        private static string _secret = "3c8727e019a42b444667a587b6001251becadabbb36bfed8087a92c18882d111";
        private UserModel _userModel;
        private TokenModel _tokenModel;

        public AuthServiceTokenGeneratorTests()
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
        public async void Generate_Token_Is_Valid_Issuer_Success()
        {
            //Arrange
            string domain = "https://test-issuer-domain/";
            string expectedDomain = "https://test-issuer-domain/";
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel)).Returns(JWTTokenFixture.GenerateToken(_userModel, _secret, domain, 4));
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(JWTTokenFixture.GenerateRefreshToken());
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.GenerateToken(_userModel);
            var tokenInformation = ClaimsFixture.GetTokenInformations(result.Response.Token, _secret, domain);

            //Assert
            Assert.Equal(expectedDomain, tokenInformation.Issuer);

        }

        [Fact]
        public async void Generate_Token_Is_Not_Valid_Issuer_Fail()
        {
            //Arrange
            string expectedDomain = "https://test-issuer-domain/";
            string domain = "https://wrong-test-issuer-domain/";
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel)).Returns(JWTTokenFixture.GenerateToken(_userModel, _secret, domain, 4));
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(JWTTokenFixture.GenerateRefreshToken());
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act & Assert
            var expection = await Assert.ThrowsAsync<SecurityTokenInvalidIssuerException>(async () =>
            {
                var result = await authService.GenerateToken(_userModel);
                var tokenInformation = ClaimsFixture.GetTokenInformations(result.Response.Token, _secret, expectedDomain);
            });   
        }

        [Fact]
        public async void Generate_Token_The_Token_Must_Have_Six_Claims_Success()
        {
            //Arrange
            string domain = "https://test-issuer-domain/";
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel)).Returns(JWTTokenFixture.GenerateToken(_userModel, _secret, domain, 4));
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(JWTTokenFixture.GenerateRefreshToken());
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.GenerateToken(_userModel);
            var claimsPrincipal = ClaimsFixture.GenerateClaimsPrincipal(result.Response.Token, _secret, domain);

            //Assert
            Assert.Equal(6, claimsPrincipal.Claims.Count());
        }

        [Fact]
        public async void Generate_Token_The_Token_Claim_Name_And_Role_Must_Have_A_Value_Success()
        {
            //Arrange
            string domain = "https://test-issuer-domain/";
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel)).Returns(JWTTokenFixture.GenerateToken(_userModel, _secret, domain, 4));
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(JWTTokenFixture.GenerateRefreshToken());
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.GenerateToken(_userModel);
            var claimsPrincipal = ClaimsFixture.GenerateClaimsPrincipal(result.Response.Token, _secret, domain);

            //Assert
            Assert.NotEmpty(claimsPrincipal.Claims.ToList()[0].Value);
            Assert.NotEmpty(claimsPrincipal.Claims.ToList()[1].Value);
        }

        [Fact]
        public async void Generate_Token_The_Token_Claim_Name_And_Role_Must_Have_A_Value_Fail()
        {
            //Arrange
            var wrongUserModelInfo = new UserModel()
            {
                Nickname = "TestNickname"
            };
            string domain = "https://test-issuer-domain/";
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel)).Returns(JWTTokenFixture.GenerateToken(wrongUserModelInfo, _secret, domain, 4));
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(JWTTokenFixture.GenerateRefreshToken());
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var result = await authService.GenerateToken(wrongUserModelInfo);
                var claimsPrincipal = ClaimsFixture.GenerateClaimsPrincipal(result.Response.Token, _secret, domain);

            });

        }

        [Fact]
        public async void Generate_Token_Alg_Header_Must_Be_HS256_Success()
        {
            //Arrange
            string domain = "https://test-issuer-domain/";
            _tokenGeneratorMock.Setup(x => x.GenerateToken(_userModel)).Returns(JWTTokenFixture.GenerateToken(_userModel, _secret, domain, 4));
            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken()).Returns(JWTTokenFixture.GenerateRefreshToken());
            var authService = new AuthService(_tokenRepositoryMock.Object, _tokenGeneratorMock.Object);

            //Act
            var result = await authService.GenerateToken(_userModel);
            var tokenInformations = ClaimsFixture.GetTokenInformations(result.Response.Token, _secret, domain);

            //Assert
            Assert.True(tokenInformations.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase));                    
        }


    }
}
