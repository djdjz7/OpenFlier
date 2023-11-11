using OpenFlier.Desktop.Services;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace OpenFlier.Desktop.MqttService
{
    internal class ImageHandler
    {
        public static void FetchScreenshot(string filename, bool usePng)
        {
            Rectangle rect = LocalStorage.ScreenSize;
            Bitmap src = new Bitmap(rect.Width, rect.Height);
            Graphics graphics = Graphics.FromImage(src);
            graphics.CopyFromScreen(0, 0, 0, 0, rect.Size);
            graphics.Save();
            graphics.Dispose();
            src.Save(
                $"Screenshots\\{filename}.{(usePng ? "png" : "jpeg")}",
                usePng ? ImageFormat.Png : ImageFormat.Jpeg
            );
        }

        public static async Task HandleSpecialChannels(User user, string filename, bool usePng)
        {

        }
    }
}
