using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
            if (comp is null)
            {
                throw new ArgumentNullException("comp", "Cannot be null");
            }
            try
            {
                comp.SecondsOffset = (int)((DateTime)RemoteTOD.GetNow(comp.Name, true) - DateTime.Now).TotalSeconds;
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

