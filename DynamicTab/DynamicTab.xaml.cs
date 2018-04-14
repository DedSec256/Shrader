using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicTab
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class CustomDynamicTab : UserControl
    {

        #region Custom properties
        public static readonly DependencyProperty TabItemsProperty = 
            DependencyProperty.Register("TabItems", typeof(ObservableCollection<TabItem>), typeof(CustomDynamicTab));


        public ObservableCollection<TabItem> TabItems
        {
            get
            {
                return GetValue(TabItemsProperty) as ObservableCollection<TabItem>;
            }
            set
            {
                SetValue(TabItemsProperty, value);
            }
        }

       

        #endregion


        public event EventHandler<EventArgs> TextChangedRichTextBoxEvent;

        public CustomDynamicTab()
        {
            try
            {
                InitializeComponent();

                // bind tab control
                TabItems = new ObservableCollection<TabItem>();
                TabItems.CollectionChanged += TabItems_CollectionChanged;
                tabDynamic.DataContext = TabItems;

                tabDynamic.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TabItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;
            foreach (var item in e.NewItems)
            {
                // add controls to tab item, this case I added just a textbox
                var rtb = new System.Windows.Forms.RichTextBox();
	            rtb.DetectUrls = false;
	            rtb.AutoWordSelection = true;
	            rtb.ReadOnly = false;
                rtb.TextChanged += Rtb_TextChanged;
                var tab = (item as TabItem);
                tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;
	            var formsHost = new WindowsFormsHost();
	            formsHost.Child = rtb;
	            tab.Content = formsHost;				
            }
        }

        private void Rtb_TextChanged(object sender, EventArgs e)
        {
            TextChangedRichTextBoxEvent?.Invoke(sender, e);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabDynamic.Items.Cast<TabItem>().SingleOrDefault(i => i.Name.Equals(tabName));

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                if (MessageBox.Show($"Are you sure you want to remove the tab '{tab.Header.ToString()}'?",
                    "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // get selected tab
                    TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

                    TabItems.Remove(tab);

                    tabDynamic.SelectedItem = selectedTab;
                }
            }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CustomDynamicTab));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        private void TabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = tabDynamic.SelectedItem;
        }
    }
}
