using Ninject;
using OpenTK;
using Shrader.IDE.View;
using Shrader.IDE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrader.IDE.IoC
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
        #region Public Properties

        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// A shortcut to access the <see cref="MainWindowViewModel"/>
        /// </summary>
        public static MainWindowViewModel MainWindowViewModel => IoC.Get<MainWindowViewModel>();
        /// <summary>
        /// A shortcut to access the <see cref="MainPageViewModel"/>
        /// </summary>
        public static MainPage MainPage => IoC.Get<MainPage>();

        #endregion

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all 
        ///       services can be found
        /// </summary>
        public static void SetupWindow()
        {
            // Bind window view model
            BindWindowViewModel();
        }

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called after login
        /// </summary>
        public static void SetupMainPage()
        {
            // Bind main page view model
            BindMainPageViewModel();
        }

        /// <summary>
        /// Binds all singleton window view model
        /// </summary>
        private static void BindWindowViewModel()
        {
            // Bind to a single instance of window view model
            Kernel.Bind<MainWindowViewModel>().ToConstant(new MainWindowViewModel());
        }
        /// <summary>
        /// Binds all singleton main page view model
        /// </summary>
        private static void BindMainPageViewModel()
        {
            //Bind to a single instance of main view model
            Kernel.Bind<MainPage>().ToConstant(new MainPage());
        }
        /// <summary>
        /// Get's a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
