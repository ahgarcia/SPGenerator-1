using System;
using System.Windows.Input;
using System.Diagnostics;

namespace SPGenerator.UI.Commands
{
    /// <summary>
    /// RelayCommand Classs provice custom command feature
    /// </summary>
    public class RelayCommand : ICommand
    {

        /// <summary>
        /// Delegate to execute
        /// </summary>
        readonly Action<object> execute;

        /// <summary>
        /// Predicate  to check canExecute
        /// </summary>
        readonly Predicate<object> canExecute;


        int WaitInMileSeconds = 200;

        /// <summary>
        /// Overload constructer of Realy Command
        /// </summary>
        /// <param name="_execute">Delegate to execute</param>
        /// <param name="_canExecute">Predicate to check can execute</param>
        public RelayCommand(Action<object> _execute, Predicate<object> _canExecute)
        {
            if (_execute == null)
                throw new ArgumentNullException("_execute");

            execute = _execute;
            canExecute = _canExecute;
        }
        /// <summary>
        /// Overload constructer of Realy Command
        /// </summary>
        /// <param name="_execute">Delegate to execute</param>
        /// <param name="_canExecute">Predicate to check can execute</param>
        public RelayCommand(Action<object> _execute, Predicate<object> _canExecute, int waitInMileSeconds)
        {
            if (_execute == null)
                throw new ArgumentNullException("_execute");

            execute = _execute;
            canExecute = _canExecute;
            WaitInMileSeconds = waitInMileSeconds;
        }
        /// <summary>
        /// Overload constructer of Relay Command
        /// </summary>
        /// <param name="_execute">Delegate to execute</param>
        public RelayCommand(Action<object> _execute)
            : this(_execute, null)
        {

        }
        /// <summary>
        /// Overload constructer of Realy Command
        /// </summary>
        /// <param name="_execute">Delegate to execute</param>
        public RelayCommand(Action<object> _execute, int waitInMileSeconds)
            : this(_execute, null, waitInMileSeconds)
        {

        }
        // Summary:
        //     Defines the method that determines whether the command can execute in its
        //     current state.
        //
        // Parameters:
        //   parameter:
        //     Data used by the command. If the command does not require data to be passed,
        //     this object can be set to null.
        //
        // Returns:
        //     true if this command can be executed; otherwise, false.
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }
        private static DateTime? LastClickTime
        {
            get;
            set;
        }
        //
        // Summary:
        //     Defines the method to be called when the command is invoked.
        //
        // Parameters:
        //   parameter:
        //     Data used by the command. If the command does not require data to be passed,
        //     this object can be set to null.
        public void Execute(object parameter)
        {
            try
            {
                if (LastClickTime == null || WaitInMileSeconds == 0 || System.DateTime.Now.Subtract(LastClickTime.Value).TotalMilliseconds > WaitInMileSeconds)
                {
                    lock (this)
                    {
                        execute(parameter);
                        LastClickTime = System.DateTime.Now;
                    }
                }
                else
                {
                }
            }
            finally
            {
                LastClickTime = System.DateTime.Now;
            }
        }
        /// <summary>
        /// Event handler to add/ remove event
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
