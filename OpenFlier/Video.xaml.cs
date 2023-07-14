using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Services;

namespace OpenFlier;
/// <summary>
/// Video.xaml 的交互逻辑
/// </summary>
public partial class Video : Window, INotifyPropertyChanged
{
    public List<string> files = new();
    public DispatcherTimer timer = new();
    private int _flag = 0;
    public int Flag
    {
        get => _flag;
        set
        {
            _flag = value;
            OnPropertyChanged("Flag");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Video()
    {
        Microsoft.Win32.OpenFileDialog ofd = new()
        {
            Multiselect = true,
        };
        if (ofd.ShowDialog() != true)
            return;
        foreach (var fileName in ofd.FileNames)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            File.Copy(fileName, Path.Combine("Screenshots", fileInfo.Name));
            files.Add(fileInfo.Name);
        }
        InitializeComponent();
        DataContext = this;
        foreach (var user in MqttService.Users)
        {
            if (!string.IsNullOrEmpty(user.CurrentClientID))
            {
                UserSelector.Items.Add(user.CurrentClientID);
            }
        }
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        if (UserSelector.SelectedItem == null) return;
        timer.Stop();
        timer = new();
        timer.Interval = TimeSpan.FromMilliseconds(Convert.ToInt32(Interval.Text));
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private async void Timer_Tick(object? sender, EventArgs e)
    {
        if (Flag < files.Count)
        {
            string s5 = JsonConvert.SerializeObject(new
            {
                type = MqttMessageType.ScreenCaptureResp,
                data = new
                {
                    name = files[Flag],
                    deviceCode = LocalStorage.MachineIdentifier,
                    versionCode = LocalStorage.Version
                }
            });
            await MqttService.MqttServer.PublishAsync(new MQTTnet.MqttApplicationMessage
            {
                Topic = UserSelector.SelectedItem + "/REQUEST_SCREEN_CAPTURE",
                Payload = Encoding.Default.GetBytes(s5),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce
            });
            Flag++;
        }
        else
        {
            Flag = files.Count - 1;
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        timer.Stop();
    }
}
