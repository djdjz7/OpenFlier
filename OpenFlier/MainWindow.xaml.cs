using OpenFlier.Services;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
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
            var serviceManager = new ServiceManager();
            serviceManager.LoadCompleted += ServiceManager_LoadCompleted;
            serviceManager.BeginLoad();
        }

        private void ServiceManager_LoadCompleted(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IPAddress.Text = LocalStorage.IPAddress;
                ConnectCode.Text = LocalStorage.ConnectCode;
                MachineIdentifier.Text = LocalStorage.MachineIdentifier;
                LoadingScreen.Visibility = Visibility.Hidden;
            });
           
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

        protected override void OnClosing(CancelEventArgs e)
        {
            UdpService.StopUdpBroadcast();
            FtpService.StopFtpServer();
            base.OnClosing(e);
        }

        public class ServiceManager
        {
            public event EventHandler? LoadCompleted;
            private Thread LoadServiceThread { get; set; }
            public ServiceManager()
            {
                LoadServiceThread = new Thread(() =>
                {
                    VerificationService.Initialize();
                    HardwareService.Initialize();
                    UdpService.Initialize();
                    FtpService.Initialize();
                    MqttService.Initialize();
                    LoadCompleted?.Invoke(this, EventArgs.Empty);
                });
                LoadServiceThread.TrySetApartmentState(ApartmentState.STA);
            }
            public void BeginLoad()
            {
                LoadServiceThread?.Start();
            }
        }
    }
}
