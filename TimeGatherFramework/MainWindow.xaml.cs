using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace TimeGatherFramework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var myoption = new MyUserSettings();
            InitializeComponent();
            Mainwin.Title = "TimeGather is collecting data...";
            Servermodelsdatacontext = new ServerModels();
            Servermodelsdatacontext.ServerModelList = new ObservableCollection<ServerModel>();
            bool enableprefixfilter = myoption.Isprefixfilterenabled;

            //StringCollection prefixes = myoption.Filters;
            //if (prefixes == null || prefixes.Count == 0)
            //{
            //    prefixes = new StringCollection() { "prefix1", "prefix2" };
            //    Properties.Settings.Default["prefixes"] = prefixes;
            //    Properties.Settings.Default.Save();
            //}

            //Loading = new ServerModel() { Name = "Loading..." };
            ServerModelsDG.DataContext = Servermodelsdatacontext.ServerModelList;
            //_servermodelsdatacontext.ServerModelList.Add(Loading);
            var computers = GetComputer.GetComputers();

            TimeQueryActionBlock = new ActionBlock<ServerModel>(sm =>
          {
              ServerModel temp = ServerModel.Updatethesecondsoffset(sm);
              var tq = new TimeQuery();
              temp.Source = tq.GetTimeSource(temp.Name);
              Application.Current.Dispatcher.BeginInvoke(new Action(() => this.Servermodelsdatacontext.ServerModelList.Add(temp)));
          }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = ExecutionDataflowBlockOptions.Unbounded }
           );

            if (enableprefixfilter)
            {
                var f = JsonConvert.DeserializeObject<ObservableCollection<string>>(myoption.Filters);
                computers = (from pc in computers
                             where FilterByPrefix(pc, f) == true
                             select pc).ToList();
            }
            foreach (var s in computers)
            {
                TimeQueryActionBlock.Post(s);
            }
            TimeQueryActionBlock.Complete();

            //servermodelsdatacontext.ServerModelList = (from comp in computers.AsParallel()
            //                 select (ServerModel)Updatethesecondsoffset(comp)).ToList();
            //ServerModelsDG.DataContext = servermodelsdatacontext.ServerModelList;
        }

        private bool FilterByPrefix(ServerModel pc, IEnumerable prefixes)
        {
            foreach (string s in prefixes)
            {
                if (pc.Name.IndexOf(s, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public ServerModels Servermodelsdatacontext { get; set; }
        public ActionBlock<ServerModel> TimeQueryActionBlock { get; set; }

        //public ServerModel Loading { get; set; }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await TimeQueryActionBlock.Completion;
            Mainwin.Title = "TimeGather is done collecting data!";
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }
    }
}