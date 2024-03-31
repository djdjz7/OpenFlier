using System.Diagnostics;
using System.Windows;

namespace OpenFlier.DevUtils;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GoToPackTabButton_Click(object sender, RoutedEventArgs e)
    {
        MainTabControl.SelectedIndex = 1;
    }

    private void GoToSubmitTabButton_Click(object sender, RoutedEventArgs e)
    {
        MainTabControl.SelectedIndex = 2;
    }

    private void OpenDocsButton_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer", "https://openflier.ml/docs");
    }
}
