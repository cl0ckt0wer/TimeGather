using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
namespace TimeGatherFramework
{

   
    public class MyUserSettings : ApplicationSettingsBase
    {
        public MyUserSettings()
        {
            if (Filters.Length == 0)
            {
                Filters = JsonConvert.SerializeObject(new ObservableCollection<string>() { "IGT" , "ADV", "SBX"});
                Save();
            }
        }
       
        [UserScopedSetting()]
        [DefaultSettingValue("true")]
        public bool Isprefixfilterenabled
        {
            get
            {
                return ((bool)this["Isprefixfilterenabled"]);
            }
            set
            {
                this["Isprefixfilterenabled"] = (bool)value;
            }
        }
        [UserScopedSetting()]
        [DefaultSettingValue("")]
        //provides the list of filters for computernames
        public string Filters
        {
            get
            {
                return (string)this["Filters"];
            }

            set
            {
                this["Filters"] = value;
                this.Save();
            }
        }
    }
}
