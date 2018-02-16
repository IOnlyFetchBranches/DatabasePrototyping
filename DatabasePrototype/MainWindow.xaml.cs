using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using static dbutils.Logger;

namespace DatabasePrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeTabs();
        }



        private void InitializeTabs()
        {
            foreach (TabItem mainTab in MainTabControl.Items)
            {
                //Debug
                LogG(mainTab.Name);

                switch (mainTab.Name)
                {
                    
                    case "TabStores":
                        //Example
                        StoresHomeTab.Content = new Label()
                            .Content = "This is an example of assigning content to a TabItem..." +
                                       "\nThis can be any Control at all, including containers!" +
                                       "\nSo You can imagine how creative you can get."+
                                       "\nBesure to Follow the naming conventions, it really helps later on." +
                                       "\nWhen you guys get a working layout, tell me we can run through it and I can wire it up." +
                                       "\nAlso note that any utility methods can be found in the attached dll I've started [dbutils]" +
                                       "\nIf you create a method/helper class/data structure you think could be used universally, create it there." +
                                       "\nDon't forget to rebuild! [Or build it] ";

                        break;
                    case "TabEmployees":
                        break;
                    case "TabCustomers":
                        break;
                    case "TabOrders":
                        break;
                    case "TabInventory":
                        break;
                    default:
                        //We should never reach here, unless you modify the name in the XAML
                        LogErr("Main", "Unable to match a Main Tab Name!");
                        break;
                }


            }
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
