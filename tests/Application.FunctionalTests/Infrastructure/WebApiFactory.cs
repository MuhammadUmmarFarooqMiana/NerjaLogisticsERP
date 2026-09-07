using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.FunctionalTests.Infrastructure;

public class WebApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseSetting("ConnectionStrings:NerjaLogisticsERPDb", connectionString);

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(provider =>
                {
                    var mock = new Mock<IUser>();
                    mock.SetupGet(x => x.Roles).Returns(TestApp.GetRoles());
                    mock.SetupGet(x => x.Id).Returns(Guid.TryParse(TestApp.GetUserId(), out var userId) ? userId : (Guid?)null);
                    return mock.Object;
                });

            // The real implementation needs an actual Excel workbook's bytes — swapped for a
            // fake here so PlatformReconciliation tests can supply PlatformRiderRow data
            // directly and exercise GenerateReconciliationReportCommandHandler's matching/
            // summing logic on its own, exactly as IPlatformReconciliationFileParser's own doc
            // comment says it's meant to be tested.
            services
                .RemoveAll<IPlatformReconciliationFileParser>()
                .AddTransient(provider =>
                {
                    var mock = new Mock<IPlatformReconciliationFileParser>();
                    mock.Setup(p => p.ParseRiderLevelSheet(It.IsAny<byte[]>()))
                        .Returns(() => TestApp.GetReconciliationRows());
                    return mock.Object;
                });
        });
    }
}
