using System;
using System.Collections.Generic;
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

namespace OpenFlier.Desktop
{
    /// <summary>
    /// PrivilegeErrorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PrivilegeErrorWindow : Window
    {
        public enum PrivilegeResult
        {
            DisableAndContinue,
            RestartAsAdmin,
        }

        private TaskCompletionSource<PrivilegeResult> taskCompletionSource = new();
        public PrivilegeErrorWindow()
        {
            InitializeComponent();
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e)
        {
            taskCompletionSource.SetResult(PrivilegeResult.DisableAndContinue);
            this.Close();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            taskCompletionSource.SetResult(PrivilegeResult.RestartAsAdmin);
            this.Close();
        }

        public new Task<PrivilegeResult> ShowDialog()
        {
            base.ShowDialog();
            return taskCompletionSource.Task;
        }
    }
}
