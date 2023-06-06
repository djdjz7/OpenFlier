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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Installer;

/// <summary>
/// FontTest.xaml 的交互逻辑
/// </summary>
public partial class FontTestScreen : Page
{
    public FontTestScreen()
    {
        InitializeComponent();
    }

    private void AbnormalButton_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe", "https://aka.ms/SegoeFluentIcons");
    }

    private void DismissButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new Uri("WelcomeScreen.xaml", UriKind.Relative));
    }
}
