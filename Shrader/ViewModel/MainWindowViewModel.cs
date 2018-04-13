using Shrader.IDE.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrader.IDE.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {

        #region Properties
        /// <summary>
        /// Text on RichTextBox
        /// </summary>
        public string Text { get; set; }

        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
        }

        #endregion
    }
}
