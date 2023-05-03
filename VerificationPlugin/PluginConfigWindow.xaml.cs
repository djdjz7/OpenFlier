using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VerificationPlugin
{
    /// <summary>
    /// PluginConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PluginConfigWindow : Window
    {
        public PluginConfigWindow()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("Plugins\\VerificationContent",VerificationContentTextBox.Text);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
