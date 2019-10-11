using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace TimeGatherCore
{
    public class ServerModel
    {
        public string Name { get; set; }
        public string OS { get; set; }
        public int SecondsOffset { get; set; }
        public string ErrorMessage { get; set; }
        public string Source { get; set; }
        public static ServerModel Updatethesecondsoffset(ServerModel comp)
        {
            try
            {
                var t = (TimeSpan)(DateTime.Now - RemoteTOD.GetNow(comp.Name, true));
                comp.SecondsOffset = (int)t.Seconds;
                comp.SecondsOffset = Math.Abs(comp.SecondsOffset);
            }
            catch (Exception ex)
            {
                comp.ErrorMessage = ex.Message;
            }
            return comp;

        }
    }
    public class ServerModels : INotifyCollectionChanged
    {
        public ObservableCollection<ServerModel> ServerModelList { get; set; }


        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                ((INotifyCollectionChanged)ServerModelList).CollectionChanged += value;
            }

            remove
            {
                ((INotifyCollectionChanged)ServerModelList).CollectionChanged -= value;
            }
        }
    }
   
}

