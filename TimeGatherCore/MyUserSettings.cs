using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
namespace TimeGatherCore
{



    public class MyUserSettings : ApplicationSettingsBase
    {
        public MyUserSettings()
        {
            if (Filters.Length == 0)
            {
                Filters = JsonSerializer.Serialize(new string [ ] {"IGT", "ADV", "SBX" });
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
