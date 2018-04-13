using Shrader.IDE.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shrader.IDE.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {

        #region Properties
        /// <summary>
        /// ErrorText on Render
        /// </summary>
        public string ErrorText { get; set; }

        #endregion

        #region Commands
        public ICommand RunCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            
        }

        #endregion
    }
}
