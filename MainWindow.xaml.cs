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
using TestCenter;
using TestCenter.TestUI;

namespace TestCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new HomePage());
        }
        public void Navigate(Page nextPage)
        {
            this.Content = nextPage;
        }

        public void Navigate(Page nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }

        private void NavigateToTests(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new TestsPage());
        }

        private void NavigateToViewTests(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ViewTestsPage());
        }

        private void NavigateToMockupPdf(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new PDFPage());
        }
    }

    public interface ISwitchable
    {
        void UtilizeState(object state);
    }

    public static class Switcher
    {
        public static MainWindow pageSwitcher;

        public static void Switch(Page newPage)
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(Page newPage, object state)
        {
            pageSwitcher.Navigate(newPage, state);
        }

    }
}
