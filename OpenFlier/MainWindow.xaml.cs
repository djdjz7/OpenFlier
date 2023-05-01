using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using OpenFlier.Services;
using static OpenFlier.Controls.PInvoke.Methods;
using static OpenFlier.Controls.PInvoke.ParameterTypes;

namespace OpenFlier
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFlierConfig.OutputDefaultConfig();
            OpenFlierConfig.ReadConfig();
            if (LocalStorage.Config.Appearances.EnableWindowEffects ?? true)
            {
                RefreshFrame();
                RefreshDarkMode();
            }
            VerificationService.InitializeVerificationService();
            HardwareService.InitializeHardwareService();
            IPAddress.Text = LocalStorage.IPAddress;
            

        }

        private void RefreshFrame()
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
            HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            MARGINS margins = new MARGINS();
            margins.cxLeftWidth = -1;
            margins.cxRightWidth = -1;
            margins.cyTopHeight = -1;
            margins.cyBottomHeight = -1;

            ExtendFrame(mainWindowSrc.Handle, margins);

            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                2);
        }

        private void RefreshDarkMode()
        {
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                0);
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
