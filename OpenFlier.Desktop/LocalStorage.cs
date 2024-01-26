using OpenFlier.Core.Services;
using System.Drawing;

namespace OpenFlier.Desktop
{
    public static class LocalStorage
    {
        public static Config Config { get; set; } = new Config();
        public static Rectangle ScreenSize = new Rectangle(0, 0, 1920, 1080);
        public static ServiceManager? ServiceManager { get; set; }
        public static Services.DesktopMqttService? DesktopMqttService { get; set; }
    }
}
