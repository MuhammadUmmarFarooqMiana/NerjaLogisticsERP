using Microsoft.Extensions.Logging;
using Moq;
using NerjaLogisticsERP.Application.Common.Behaviours;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Platforms.Commands.CreatePlatform;
using NUnit.Framework;

namespace NerjaLogisticsERP.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<CreatePlatformCommand>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreatePlatformCommand>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Guid.NewGuid());

        var requestLogger = new LoggingBehaviour<CreatePlatformCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreatePlatformCommand { Name = "Hunger" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreatePlatformCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreatePlatformCommand { Name = "Hunger" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<Guid>()), Times.Never);
    }
}
