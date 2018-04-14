using OpenTK;
using Shrader.IDE.DataModels;
using Shrader.IDE.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shrader.IDE.ViewModel
{
    public class FullScreenRenderViewModel : BaseViewModel
    {
        /// <summary>
        /// Command for exit by esc pressing
        /// </summary>
        public ICommand ExitCommand { get; set; }

        public FullScreenRenderViewModel()
        {         
            
            ExitCommand = new RelayCommand((obj) =>
            {
                IoC.IoC.MainWindowViewModel.GoToPage(ApplicationPage.Main);
            });
        }
    }
}
