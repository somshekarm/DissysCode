using FileScanner.Commands;
using FileScanner.Model;
using Ookii.Dialogs.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileScanner.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Customer> _customers;
        private List<Customer> _customerList;
        private List<FileInfo> fileInfos = new List<FileInfo>();        
        private ICommand _selectFolderCommand;
        private int currentCount = 0;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SelectFolderCommand
        {
            get
            {
                return _selectFolderCommand ?? (_selectFolderCommand = new CommandHandler(() => SelectFolder(), () => CanExecute));
            }
        }
        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                return true;
            }
        }

        public ObservableCollection<Customer> Customers 
        { 
            get
            {                
                {
                    return _customers;
                }
            }
            private set
            {
                _customers = value;                
                OnPropertyChanged("Customers");
                
            }                
        }

        public bool EndOfFile { get; private set; }

        public MainViewModel()
        {
            _customers = new ObservableCollection<Customer>();
            _customerList = new List<Customer>();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task SelectFolder()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true;   
            if ((bool)dialog.ShowDialog())
            {
                _customers.Clear();
                _customerList.Clear();
                currentCount = 0;
                EndOfFile = false;
                Customers = new ObservableCollection<Customer>(_customerList);
                var path = dialog.SelectedPath;
                var dirInfo = new DirectoryInfo(path);
                fileInfos = dirInfo.GetFiles("*.txt", SearchOption.AllDirectories).ToList();
                await LoadMoreData();
            }
        }

        public async Task LoadMoreData()
        {            
            var listToprocess = fileInfos.Skip(currentCount).Take(100);
            currentCount += listToprocess.Count();
            if(listToprocess.Count() == 0)
            {
                EndOfFile = true;
            }

            foreach (var file in listToprocess)
            {
                var customersFound = await GetFileText(file.FullName);
                if (customersFound.Count() > 0)
                {                    
                    _customerList.AddRange(customersFound);
                    Customers = new ObservableCollection<Customer>(_customerList);
                }
            }
        }

        private async Task<List<Customer>> GetFileText(string filePath)
        {
            var fileContents = new List<string>();
            string customerInfo = @"CustomerInfo:";
            string nameString = @"Name:";
            string phoneString = @"PhoneNumber:";
            string addressString = @"Address:";
            var customers = new List<Customer>();
            
            if (File.Exists(filePath))
            {
                var content = await File.ReadAllLinesAsync(filePath);
                fileContents = content.ToList();
                var customerValue = fileContents.Where(x => x.StartsWith(customerInfo)).Select(x => x.Remove(0, customerInfo.Length));   
                foreach(var customerData in customerValue) 
                {
                    var values = customerData.Split(';').ToList();
                    var nameValue = values.Where(x => x.StartsWith(nameString)).Select(x => x.Remove(0, nameString.Length)).ToList();
                    var addressValue = values.Where(x => x.StartsWith(addressString)).Select(x => x.Remove(0, addressString.Length)).ToList();
                    var phoneValue = values.Where(x => x.StartsWith(phoneString)).Select(x => x.Remove(0, phoneString.Length)).ToList();
                    var customer = new Customer { Id = this.Customers.Count() + customers.Count() + 1, Name = nameValue[0], Address = addressValue[0], PhoneNumber = phoneValue[0] };
                    customers.Add(customer);
                }
            }
            return customers;
        }
    }
}
