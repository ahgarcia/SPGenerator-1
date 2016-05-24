using SPGenerator.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD = SPGenerator.DataModel;

namespace SPGenerator.UI.Comman
{
    internal static class Settings
    {
        internal static void SaveSettings(SD.Settings settings)
        {
            var filePath = Path.GetTempPath() + "\\" + Constants.settingTempFileName;
            Serializer.Serialize<SD.Settings>(settings, filePath);
        }
        internal static SPGenerator.DataModel.Settings GetSettings()
        {
            var filePath = Path.GetTempPath() + "\\" + Constants.settingTempFileName;
            var settings = Serializer.Deserialize<SD.Settings>(new SD.Settings(), filePath);
            if (settings == null)
                settings = new SD.Settings();
            return settings;
        }
    }
}
