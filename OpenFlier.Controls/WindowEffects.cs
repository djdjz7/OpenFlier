using System.Windows;
using System.Windows.Interop;
using static OpenFlier.Controls.PInvoke.Methods;
using static OpenFlier.Controls.PInvoke.ParameterTypes;

namespace OpenFlier.Controls
{
    public class WindowEffects
    {
        public static void EnableWindowEffects(Window window)
        {
            RefreshFrame(window);
            RefreshDarkMode(window);
        }

        private static void RefreshFrame(Window window)
        {
            var mainWindowPtr = new WindowInteropHelper(window).Handle;
            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc.CompositionTarget.BackgroundColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);

            var margins = new MARGINS();
            margins.cxLeftWidth = -1;
            margins.cxRightWidth = -1;
            margins.cyTopHeight = -1;
            margins.cyBottomHeight = -1;

            ExtendFrame(mainWindowSrc.Handle, margins);

            SetWindowAttribute(
                new WindowInteropHelper(window).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                2);
        }

        private static void RefreshDarkMode(Window window)
        {
            SetWindowAttribute(
                new WindowInteropHelper(window).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                0);
        }
    }
}
