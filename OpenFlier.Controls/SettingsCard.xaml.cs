using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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

namespace OpenFlier.Controls
{
    /// <summary>
    /// SettingsCard.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsCard : UserControl
    {
        public SettingsCard()
        {
            InitializeComponent();
        }



        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SettingsCard), new PropertyMetadata("Placeholder_Header"));



        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SettingsCard), new PropertyMetadata("PlaceHolder_Description"));



        public bool IsClickEnabled
        {
            get { return (bool)GetValue(IsClickEnabledProperty); }
            set { SetValue(IsClickEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsClickEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClickEnabledProperty =
            DependencyProperty.Register("IsClickEnabled", typeof(bool), typeof(SettingsCard), new PropertyMetadata(false));



        public UIElement ActionIcon
        {
            get { return (UIElement)GetValue(ActionIconProperty); }
            set { SetValue(ActionIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActionIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionIconProperty =
            DependencyProperty.Register("ActionIcon", typeof(UIElement), typeof(SettingsCard));



        public UIElement HeaderIcon
        {
            get { return (UIElement)GetValue(HeaderIconProperty); }
            set { SetValue(HeaderIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderIconProperty =
            DependencyProperty.Register("HeaderIcon", typeof(UIElement), typeof(SettingsCard));


    }
}
