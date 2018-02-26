using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using dbutils.Models;

using static dbutils.Logger;

namespace DatabasePrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// DENOTE EACH QUERY IN CODE WITH A //QUERY {For what/Question} HEADER
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            

            //Load Functionality for Employees
            InitializeEmployees();




        }


        /// <summary>
        /// This is the method that inits the employee functionaility
        /// </summary>
        private void InitializeEmployees()
        {
            var SearchBy = EmployeesSearchByComboBox;

            SearchBy.SelectionChanged += (obj, sender) =>
            {
                //Set on change to enable search button, if filterby is untouched
                if (!EmployeesFilterOptionBar.IsEnabled)
                    EmployeesRunButton.IsEnabled = true;
                else
                {
                    EmployeesRunButton.IsEnabled = false;
                }
            };

            var FilterBy = EmployeesFilterByComboBox;

            FilterBy.SelectionChanged += (obj, sender) =>
            {
                //Set on change to enable filter box if not enabled
                if (!EmployeesFilterOptionBar.IsEnabled)
                    EmployeesFilterOptionBar.IsEnabled = true;

                if (EmployeesRunButton.IsEnabled)
                    EmployeesRunButton.IsEnabled = false;
            };

            var RunButton = EmployeesRunButton;

            //Remember to set default button for each home tab, so that enter will trigger.
            RunButton.IsDefault = true;
            RunButton.Click += (obj, sender) =>
            {
                //The run button for each tab is responsible for sanitizing input, building the query and launching the result tab
                //First case is when there is no filter by
                if (!EmployeesFilterOptionBar.IsEnabled)
                {
                    string searchByChoice = SearchBy.Text;
                    //Remove spaces
                    searchByChoice = searchByChoice.Replace(" ", "");
                    
                    //Build Query
                    switch (searchByChoice.ToLower())
                    {
                        case "eid":
                            //QUERY Get Employee By Eid
                            break;
                    }
                }
            }




        }
    

        //Event Handlers
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

       

        private void EmployeesSearchBar_OnMouseEnter(object sender, MouseEventArgs e)
        {
            EmployeesSearchBar.Text = "";
        }
    }
}
