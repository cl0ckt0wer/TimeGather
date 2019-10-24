using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TimeGatherCore
{
    /// <summary>
    /// set wether we are going to filter by computername prefix, and which prefixes those are.
    /// </summary>
    public partial class Settings : Window
    {
        public string AddThisPrefix { get; set; }
        public ObservableCollection<string> Filters { get; set; }
        
        public static Prop<bool> FilterEnable { get; set; } = new Prop<bool> { Value = true };

        public Settings()
        {
            InitializeComponent();
            var s = new MyUserSettings();
            Filters = new ObservableCollection<string>(JsonSerializer.Deserialize<string[]>(s.Filters));
            FilterEnable.Value = s.Isprefixfilterenabled;
            DataContext = this;
        }

        private void DataWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var s = new MyUserSettings();
            s.Isprefixfilterenabled = FilterEnable.Value;
            s.Filters = JsonSerializer.Serialize(Filters.ToArray());
            s.Save();
            MessageBox.Show("You must restart the application for changes to take effect.", "FYI", MessageBoxButton.OK, MessageBoxImage.Asterisk);
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
