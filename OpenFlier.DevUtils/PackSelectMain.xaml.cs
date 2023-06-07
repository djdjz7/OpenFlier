using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace OpenFlier.DevUtils;
/// <summary>
/// PackSelectMain.xaml 的交互逻辑
/// </summary>
public partial class PackSelectMain : Page
{
    ObservableCollection<PluginFileModel> PluginFiles;
    public PackSelectMain(ObservableCollection<PluginFileModel> pluginFiles)
    {
        InitializeComponent();
        PluginFiles = pluginFiles;
        PluginFilesList.ItemsSource = PluginFiles;
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        if (PluginFilesList.SelectedItem is not PluginFileModel file)
            return;
        if (file.FilePath?.EndsWith(".dll") != true)
        {
            MessageBox.Show("This is not a .dll file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        file.IsPluginMain = true;
        PackSummary packSummary = new(PluginFiles);
        NavigationService.Navigate(packSummary);
    }

    private void Previous_Click(object sender, RoutedEventArgs e)
    {
        PackSelectFile packSelectFile = new(PluginFiles);
        NavigationService.Navigate(packSelectFile);
    }
}
