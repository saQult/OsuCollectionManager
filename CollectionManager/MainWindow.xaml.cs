using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CollectionManager.Core;

namespace CollectionManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Config.Load();

        _homePage = new HomePage();
        _settingsPage = new SettingsPage();
        _osuCollectorPage = new OsuCollectorPage();

        InitializeComponent();
        MainFrame.Navigate(_homePage);
    }
    private HomePage _homePage;
    private SettingsPage _settingsPage;
    private OsuCollectorPage _osuCollectorPage;
    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(_homePage);
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(_settingsPage);
    }

    private void OsuCollectorButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(_osuCollectorPage);
    }
}