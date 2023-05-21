using OpenFlier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static OpenFlier.Controls.PInvoke.Methods;
using static OpenFlier.Controls.PInvoke.ParameterTypes;

namespace OpenFlier
{
    /// <summary>
    /// SelectNetworkInterface.xaml 的交互逻辑
    /// </summary>
    public partial class SelectNetworkInterface : Window
    {
        List<IPAddress> IPAddressList;
        public SelectNetworkInterface(List<IPAddress> ipAddressList)
        {
            InitializeComponent();
            IPAddressList = ipAddressList;
            IPListBox.ItemsSource = IPAddressList;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigService.OutputDefaultConfig();
            ConfigService.ReadConfig();
            VerificationService.Initialize();
            if (LocalStorage.Config.Appearances.EnableWindowEffects ?? true)
            {
                RefreshFrame();
                RefreshDarkMode();
            }

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
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        public event EventHandler? InterfaceSelected;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InterfaceSelected?.Invoke(this, e);
            this.Close();
        }
    }
}
