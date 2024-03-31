using System.Threading.Tasks;
using System.Windows;

namespace OpenFlier.Desktop.View
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
