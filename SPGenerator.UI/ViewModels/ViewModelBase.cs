using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.UI.ViewModels
{
    /// <summary>
    /// Abstart class for ViewModel to raise Property Change Event
    /// </summary>
    abstract class ViewModelBase : System.ComponentModel.INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Property Change Event Handler
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method to raise Property Change Event
        /// </summary>
        /// <param name="propertyName">Name of Property which Changed</param>
        internal void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when class disposed
        /// </summary>
        public void Dispose()
        {
            PropertyChanged = null;
        }
    }
}
