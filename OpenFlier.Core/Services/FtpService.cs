using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace OpenFlier.Core.Services;

public class FtpService
{
    public class OpenFlierMerbershipProvider : IMembershipProvider
    {
        public Task<MemberValidationResult> ValidateUserAsync(string username, string password)
        {
            if (username != "tester" || password != "testing")
                return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
            return Task.FromResult(
                new MemberValidationResult(MemberValidationStatus.AuthenticatedUser,
                    new ClaimsPrincipal(new ClaimsIdentity(
                        new Claim[3]
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.Role, username),
                            new Claim(ClaimTypes.Role, "user")
                        }, "custom"
                        ))));
        }
    }

    public ServiceProvider? ServiceProvider { get; set; }
    public IFtpServerHost? FtpServerHost { get; set; }

    private string? specifiedFtpDirectory;
    public void Initialize()
    {
        specifiedFtpDirectory = string.IsNullOrEmpty(CoreStorage.CoreConfig.FtpDirectory) ? "Screenshots" : CoreStorage.CoreConfig.FtpDirectory;
        if (Directory.Exists(specifiedFtpDirectory))
            Directory.Delete(specifiedFtpDirectory, true);
        Directory.CreateDirectory(specifiedFtpDirectory);

        StartFtpServer();
    }
    public void StartFtpServer()
    {
        var services = new ServiceCollection();
        services
            .Configure<DotNetFileSystemOptions>(opt => opt.RootPath = specifiedFtpDirectory);
        services.AddFtpServer(builder => builder
            .UseDotNetFileSystem()
            .EnableAnonymousAuthentication());
        services.AddSingleton<IMembershipProvider, OpenFlierMerbershipProvider>();
        ServiceProvider = services.BuildServiceProvider();
        FtpServerHost = ServiceProvider.GetRequiredService<IFtpServerHost>();
        FtpServerHost.StartAsync();

    }

    public void StopFtpServer()
    {
        FtpServerHost?.StopAsync();
        ServiceProvider?.Dispose();
    }
}
