using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpenFlier.Services
{
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

        public static ServiceProvider? ServiceProvider { get; set; }
        public static IFtpServerHost? FtpServerHost { get; set; }
        public static void Initialize()
        {
            if (Directory.Exists("Screenshots"))
                Directory.Delete("Screenshots", true);
            Directory.CreateDirectory("Screenshots");
            StartFtpServer();
        }
        public static void StartFtpServer()
        {
            var services = new ServiceCollection();
            services
                .Configure<DotNetFileSystemOptions>(opt => opt.RootPath = "Screenshots");
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem()
                .EnableAnonymousAuthentication());
            services.AddSingleton<IMembershipProvider, OpenFlierMerbershipProvider>();
            ServiceProvider = services.BuildServiceProvider();
            FtpServerHost = ServiceProvider.GetRequiredService<IFtpServerHost>();
            FtpServerHost.StartAsync();

        }

        public static void StopFtpServer()
        {
            FtpServerHost?.StopAsync();
            ServiceProvider.Dispose();
        }
    }
}
