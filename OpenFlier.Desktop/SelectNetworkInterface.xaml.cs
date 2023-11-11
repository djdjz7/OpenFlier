using OpenFlier.Controls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace OpenFlier.Desktop
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
            if (LocalStorage.Config.Appearances.EnableWindowEffects ?? true)
            {
                WindowEffects.EnableWindowEffects(this);
            }

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
