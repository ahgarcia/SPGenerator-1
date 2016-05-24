using SPGenerator.UI.Commands;
using SPGenerator.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SPGenerator.UI.ViewModels
{
    class SettingsVM : ViewModelBase
    {
        SettingsModel model;
        public SettingsVM()
        {
            model = new SettingsModel();
            LoadSettings();
        }

        #region Properties
        string prefixWhereParameter;
        public string PrefixWhereParameter
        {
            get
            {
                return prefixWhereParameter;
            }
            set
            {
                prefixWhereParameter = value;
                NotifyPropertyChanged("PrefixWhereParameter");
            }
        }

        string prefixInputParameter;
        public string PrefixInputParameter
        {
            get
            {
                return prefixInputParameter;
            }
            set
            {
                prefixInputParameter = value;
                NotifyPropertyChanged("PrefixInputParameter");
            }
        }

        string prefixInsertSp;
        public string PrefixInsertSp
        {
            get
            {
                return prefixInsertSp;
            }
            set
            {
                prefixInsertSp = value;
                NotifyPropertyChanged("PrefixInsertSp");
            }
        }

        string prefixUpdateSp;
        public string PrefixUpdateSp
        {
            get
            {
                return prefixUpdateSp;
            }
            set
            {
                prefixUpdateSp = value;
                NotifyPropertyChanged("PrefixUpdateSp");
            }
        }

        string errorHandling;
        public string ErrorHandling
        {
            get
            {
                return errorHandling;
            }
            set
            {
                errorHandling = value;
                NotifyPropertyChanged("ErrorHandling");
            }
        }

        public string[] ErrorHandlingOptions
        {
            get
            {
                return new string[] { "Yes", "No" };

            }
        }


        #endregion

        #region Commands
        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null) saveCommand = new RelayCommand(param => this.Save(param));
                return saveCommand;
            }
        }

        private RelayCommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null) cancelCommand = new RelayCommand(param => this.Cancel(param));
                return cancelCommand;
            }
        }

        private void Cancel(object param)
        {
            ((Window)param).Close();
        }

        private void Save(object param)
        {
            var settings = new SPGenerator.DataModel.Settings();
            settings.prefixInputParameter = prefixInputParameter;
            settings.prefixInsertSp = prefixInsertSp;
            settings.prefixUpdateSp = prefixUpdateSp;
            settings.prefixWhereParameter = prefixWhereParameter;
            settings.errorHandling = ErrorHandling;
            model.SaveSettings(settings);
            ((Window)param).Close();
        }


        #endregion

        private void LoadSettings()
        {
            var settings = model.GetSettings();
            PrefixInputParameter = settings.prefixInputParameter;
            PrefixInsertSp = settings.prefixInsertSp;
            PrefixUpdateSp = settings.prefixUpdateSp;
            PrefixWhereParameter = settings.prefixWhereParameter;
            ErrorHandling = settings.errorHandling;
        }
    }
}
