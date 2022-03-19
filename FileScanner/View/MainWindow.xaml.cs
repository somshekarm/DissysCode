using FileScanner.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileScanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void OnScrollChange(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sb = e.OriginalSource as ScrollViewer;
            if(sb.VerticalOffset == 0)
            {
                return;
            }

            if (sb.VerticalOffset == sb.ScrollableHeight)
            {
                var vm = ((MainViewModel)DataContext);
                var count = vm.Customers.Count();
                _ = vm.LoadMoreData();
                sb.ScrollToVerticalOffset(count - 100);                                
            }
        }
    }
}
