using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shrader.IDE.ViewModel.Base
{
    public class RelayCommand : ICommand
    {
        #region Private members

        private Action<object> execute;
        private Func<object, bool> canExecute;

        #endregion

        #region Public event

#pragma warning disable CS0067 // Событие "RelayCommand.CanExecuteChanged" никогда не используется.
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // Событие "RelayCommand.CanExecuteChanged" никогда не используется.

        #endregion

        #region Constructor

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region ICommand methods

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        #endregion
    }
}
