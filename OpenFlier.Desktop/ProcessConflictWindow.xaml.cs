using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// ProcessConflictWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessConflictWindow : Window
    {
        public enum ProcessConflictResolution
        {
            QuitCurrent,
            KillAndContinue,
        }

        private TaskCompletionSource<ProcessConflictResolution> taskCompletionSource = new();
        public ProcessConflictWindow()
        {
            InitializeComponent();
        }

        private void QuitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            taskCompletionSource.SetResult(ProcessConflictResolution.QuitCurrent);
            this.Close();
        }

        private void KillAndContinue_Click(object sender, RoutedEventArgs e)
        {
            taskCompletionSource.SetResult(ProcessConflictResolution.KillAndContinue);
            this.Close();
        }

        public new Task<ProcessConflictResolution> ShowDialog()
        {
            base.ShowDialog();
            return taskCompletionSource.Task;
        }
    }
}
