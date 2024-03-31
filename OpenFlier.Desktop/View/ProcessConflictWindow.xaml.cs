using System.Threading.Tasks;
using System.Windows;

namespace OpenFlier.Desktop.View
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
