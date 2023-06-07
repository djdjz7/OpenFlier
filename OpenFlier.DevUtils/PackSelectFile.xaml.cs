using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace OpenFlier.DevUtils;
/// <summary>
/// PackSelectFile.xaml 的交互逻辑
/// </summary>
public partial class PackSelectFile : Page
{
    public ObservableCollection<PluginFileModel> PluginFiles
    {
        get; set;
    } = new ObservableCollection<PluginFileModel>();
    public PackSelectFile()
    {
        InitializeComponent();
        PluginFilesList.ItemsSource = PluginFiles;
    }
    public PackSelectFile(ObservableCollection<PluginFileModel> pluginFiles)
    {
        InitializeComponent();
        PluginFiles = pluginFiles;
        PluginFilesList.ItemsSource = PluginFiles;
    }

    private void RemoveFileButton_Click(object sender, RoutedEventArgs e)
    {
        PluginFiles.Remove(PluginFilesList.SelectedItem as PluginFileModel ?? new PluginFileModel());

    }

    private void AddFileButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog ofd = new OpenFileDialog
        {
            Multiselect = true,
            Title = "Select plugin files"
        };

        if (ofd.ShowDialog() == true)
        {
            foreach (var file in ofd.FileNames)
            {
                var fileInfo = new FileInfo(file);
                PluginFiles.Add(new PluginFileModel
                {
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                });
            }
        }

    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        if (PluginFiles.Count <= 0)
        {
            MessageBox.Show("No files selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        bool flag = false;
        foreach (var file in PluginFiles)
        {
            if (file.FilePath?.EndsWith(".dll") == true)
                flag = true;
        }
        if (!flag)
        {
            MessageBox.Show("No .dll files selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        PackSelectMain packSelectMain = new(PluginFiles);
        NavigationService.Navigate(packSelectMain);
    }
}
