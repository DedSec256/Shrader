using Shrader.IDE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ShaderBuilder;

namespace Shrader.IDE.View
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public MainWindow()
		{
            OpenTK.Toolkit.Init();
            InitializeComponent();

            DataContext = new MainWindowViewModel();
		}

        #region Highlight part

        private void CodeEditSpace_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange documentRange = new TextRange(CodeEditSpace.Document.ContentStart, CodeEditSpace.Document.ContentEnd);
            //documentRange.ClearAllProperties();
            var text = documentRange.Text;
        }

        #endregion

        #region Render part

        ShaderBuilder.ShaderBuilder shaderBuilder;

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            RenderCanvas.MakeCurrent();
        }

        private void RenderCanvas_Load(object sender, EventArgs e)
        {
            shaderBuilder = new ShaderBuilder.ShaderBuilder();

        }

        private void RenderCanvas_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            shaderBuilder.Paint(RenderCanvas);
        }

        #endregion
    }
}
