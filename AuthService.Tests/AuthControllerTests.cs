using AuthService.Controllers;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AuthService.Tests;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly IConfiguration _configuration;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestOnly_SuperSecretKey_MinLength32Chars!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:TokenExpirationMinutes"] = "30"
            })
            .Build();

        var loggerMock = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_userManagerMock.Object, _configuration, loggerMock.Object);
    }

    [Fact]
    public async Task Register_Success_ReturnsOk()
    {
        var model = new RegisterModel { Email = "test@test.com", Password = "P@ssw0rd" };
        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(model);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Register_Failure_ReturnsBadRequest()
    {
        var model = new RegisterModel { Email = "test@test.com", Password = "weak" };
        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var result = await _controller.Register(model);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var user = new ApplicationUser { UserName = "test@test.com", Email = "test@test.com" };
        _userManagerMock
            .Setup(um => um.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, "P@ssw0rd"))
            .ReturnsAsync(true);

        var model = new LoginModel { Email = "test@test.com", Password = "P@ssw0rd" };
        var result = await _controller.Login(model);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        var tokenProperty = okResult.Value.GetType().GetProperty("token");
        Assert.NotNull(tokenProperty);
        var tokenValue = tokenProperty.GetValue(okResult.Value) as string;
        Assert.False(string.IsNullOrEmpty(tokenValue));
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var user = new ApplicationUser { UserName = "test@test.com", Email = "test@test.com" };
        _userManagerMock
            .Setup(um => um.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(um => um.CheckPasswordAsync(user, "wrong"))
            .ReturnsAsync(false);

        var model = new LoginModel { Email = "test@test.com", Password = "wrong" };
        var result = await _controller.Login(model);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_NonExistingUser_ReturnsUnauthorized()
    {
        _userManagerMock
            .Setup(um => um.FindByEmailAsync("nobody@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var model = new LoginModel { Email = "nobody@test.com", Password = "P@ssw0rd" };
        var result = await _controller.Login(model);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
