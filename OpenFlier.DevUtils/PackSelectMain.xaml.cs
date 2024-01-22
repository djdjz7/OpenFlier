using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace OpenFlier.DevUtils;
/// <summary>
/// PackSelectMain.xaml 的交互逻辑
/// </summary>
public partial class PackSelectMain : Page
{
    public ObservableCollection<FileSystemEntry> DisplayingFiles { get; set; } = new ObservableCollection<FileSystemEntry>();
    private string _zipArchivePath;
    private string _zipExtractedDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenFlier", "dev-utils-zip-temp");
    private DirectoryInfo _currentDirectory;
    public PackSelectMain(string filePath)
    {
        InitializeComponent();
        if (Directory.Exists(_zipExtractedDir))
            Directory.Delete(_zipExtractedDir, true);
        _zipArchivePath = filePath;
        using (var stream = new FileStream(_zipArchivePath, FileMode.Open))
        {
            using (var zip = new ZipArchive(stream))
            {
                zip.ExtractToDirectory(_zipExtractedDir, true);
            }
        }
        _currentDirectory = new DirectoryInfo(_zipExtractedDir);
        UpdateFilesList();
        DisplayingFileList.ItemsSource = DisplayingFiles;
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        var selectedValue = DisplayingFileList.SelectedValue as FileSystemEntry;
        if (selectedValue is null)
            return;
        if (selectedValue.IsDirectory == true)
            return;
        var fileInfo = new FileInfo(selectedValue.FullName);
        if (fileInfo.Extension.ToLower() != ".dll")
        {
            MessageBox.Show("You must select a *.dll file!");
            return;
        }
        NavigationService.Navigate(new PackSummary(_zipArchivePath, _zipExtractedDir, selectedValue));
    }

    private void Previous_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void UpdateFilesList()
    {
        DisplayingFiles.Clear();
        foreach (var dir in _currentDirectory.GetDirectories())
        {
            DisplayingFiles.Add(new FileSystemEntry
            {
                Name = dir.Name,
                FullName = dir.FullName,
                IsDirectory = true,
            });
        }
        foreach (var file in _currentDirectory.GetFiles())
        {
            DisplayingFiles.Add(new FileSystemEntry
            {
                Name = file.Name,
                FullName = file.FullName,
                IsDirectory = false,
            });
        }
    }

    private void EntryPanel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var selectedValue = DisplayingFileList.SelectedValue as FileSystemEntry;
        if (selectedValue is null)
            return;
        if (!selectedValue.IsDirectory)
            return;
        _currentDirectory = new DirectoryInfo(selectedValue.FullName);
        UpdateFilesList();
    }
}
