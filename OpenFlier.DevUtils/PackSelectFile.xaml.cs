using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OpenFlier.DevUtils;

/// <summary>
/// PackSelectFile.xaml 的交互逻辑
/// </summary>
public partial class PackSelectFile : Page
{
    public PackSelectFile()
    {
        InitializeComponent();
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        var filePath = FilePathTextBox.Text;
        if (!File.Exists(filePath))
        {
            MessageBox.Show("No such file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        PackSelectMain packSelectMain = new(filePath);
        NavigationService.Navigate(packSelectMain);
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog ofd = new OpenFileDialog()
        {
            Filter = "Zip archive (*.zip)|*.zip",
            CheckFileExists = true,
        };
        var result = ofd.ShowDialog();
        if (result == true)
        {
            FilePathTextBox.Text = ofd.FileName;
        }
    }
}
