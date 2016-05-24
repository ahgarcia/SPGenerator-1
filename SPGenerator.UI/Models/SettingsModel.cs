using SPGenerator.UI.Comman;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD = SPGenerator.DataModel;

namespace SPGenerator.UI.Models
{
    internal class SettingsModel
    {
        internal void SaveSettings(SD.Settings settings)
        {
            Comman.Settings.SaveSettings(settings);
        }

        internal SPGenerator.DataModel.Settings GetSettings()
        {
            return Comman.Settings.GetSettings();
        }
    }
}
