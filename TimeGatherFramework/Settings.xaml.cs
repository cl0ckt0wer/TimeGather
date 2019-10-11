using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Shapes;

namespace TimeGatherFramework
{
    /// <summary>
    /// set wether we are going to filter by computername prefix, and which prefixes those are.
    /// </summary>
    public partial class Settings : Window
    {
        public string AddThisPrefix { get; set; }
        public ObservableCollection<string> Filters {get;set;}
        public bool FilterEnable { get; set; }
        public Settings()
        {
            InitializeComponent();
            var s = new MyUserSettings();
            Filters = JsonConvert.DeserializeObject<ObservableCollection<string>>(s.Filters);
            FilterEnable = s.Isprefixfilterenabled;
            DataContext = this;
        }

        private void DataWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var s = new MyUserSettings();
            s.Isprefixfilterenabled = FilterEnable;
            s.Filters = JsonConvert.SerializeObject(Filters);
            s.Save();
            MessageBox.Show("You must restart the application for changes to take effect.", "FYI", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

       

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AddFilterPrefix_Click(object sender, RoutedEventArgs e)
        {
            Filters.Add(AddThisPrefix);
        }

        private void RemoveSelectedFromList_Click(object sender, RoutedEventArgs e)
        {
            Filters.Remove((string)Filterbox.SelectedItem);
        }
    }
}
