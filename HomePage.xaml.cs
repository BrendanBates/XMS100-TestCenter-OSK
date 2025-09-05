using System;
using System.Collections.Generic;
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
using TestCenter.TestUI;

namespace TestCenter
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage() {
            InitializeComponent();
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductList.SelectedItem != null) {
                App.deviceID = (uint)ProductList.SelectedIndex;
            }
            else
            {
                //throw exception
            }
            Switcher.Switch(new TestsPage());
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (unitSelect.SelectedIndex == 0)
            {
                var newItem = new ListBoxItem
                {
                    Name = "INT",
                    Margin = new Thickness(0),
                    FontWeight = FontWeights.Bold,
                    //FontFamily = FontFamily.Source{Segoe UI},
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(40, 0, 0, 0),
                    Height = 30
                };
            }
        }
    }
}

