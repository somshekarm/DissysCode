using FileScanner.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            
            if (sb.VerticalOffset == 0)
            {
                return;
            }            
            else if (sb.VerticalOffset == sb.ScrollableHeight)
            {   
                var vm = ((MainViewModel)DataContext);
                if (vm.EndOfFile)
                {
                    MessageBox.Show("No more Customer to display");
                    return;
                }
                var count = vm.Customers.Count();
                _ = vm.LoadMoreData();
                sb.ScrollToVerticalOffset(count - 100);
            }           
        }
    }
}
