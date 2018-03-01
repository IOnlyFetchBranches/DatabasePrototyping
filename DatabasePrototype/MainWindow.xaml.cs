using System;
using System.Collections.Generic;
using System.Data;
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
using DatabasePrototype.Models;
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
        /// <summary>
        /// Represents our database connection.
        /// You'll attach your SqlCommands to this object.
        /// Use ConnectionManager to open, rather than instantiating a new one every time.
        /// </summary>
        private static SqlConnection db;
        

        public MainWindow()
        {

            InitializeComponent();

           
            //Open Db Connection
            db = ConnectionManager.Open(ConnectionStrings.Marcus);

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
                if (true)
                {

                    var sanitizedText = EmployeesSearchBar.Text;

                    int spaces = 0; //TrackSpaces 

                    //Must sanitize to prevent SQL Injection.
                    foreach (char c in sanitizedText)
                    {
                        if (!char.IsLetterOrDigit(c))
                        {

                            MessageBox.Show("Invalid Input!");
                            return;

                        }
                        if (c == ' ')
                            spaces++;
                        if (spaces > 1)
                        {
                            MessageBox.Show("Too Many Spaces!");
                            return;
                        }
                    }

                    string searchByChoice = SearchBy.Text;
                    string filterByChoice = FilterBy.Text;

                    
                    //Remove spaces
                    searchByChoice = searchByChoice.Replace(" ", "");
                    filterByChoice = filterByChoice.Replace(" ", "");
                    
                    //Build Query

                    string filterStatement = ""; //triggered if filter by is on

                    if (EmployeesFilterOptionBar.IsEnabled && EmployeesFilterOptionBar.Text.Length > 0)
                    {
                        filterStatement = " And " + filterByChoice + " = '" + EmployeesFilterOptionBar.Text + "'";
                    }
                    
                     //QUERY Get Employee By Eid
                    SqlCommand GetEmployeeByEid = new SqlCommand(
                                //Do not forget to space after each statement
                                "Select EID, FirstName, LastName From Employees " +
                                //Yes for inserted strings, that you want to be evaluated in sql as string
                                //You still have to concat the ' before AND after!
                                "Where "+ searchByChoice + " = '" + sanitizedText+"' " + filterStatement
                                );
                    //You must link the connection, maybe we can create a connection wrapper that does this for us.
                    //You could also pass as second param in constructor but the idea would be to have it link automatically
                    //It could grab the db connection from the Connection Manager.
                    GetEmployeeByEid.Connection = db;

                    //Launch Command, Returns a Reader on the result table
                    var results = GetEmployeeByEid.ExecuteReader();

                    //Spawn a new results tab;
                    var resultsTab = new ResultsTab();

                    //Compile results here for testing
                    var sb = new StringBuilder();
                    
                    
                    
                    while (results.Read())
                    {
                        sb.AppendLine(results[0] + " " + results[1] + " " + results[2]);
                        
                    }

                    results.Close(); //Gotta close the reader to recycle the command.

                    MessageBox.Show(sb.ToString(), "Results", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                

            };

            var FilterOptions = EmployeesFilterOptionBar;

            EmployeesFilterOptionBar.TextChanged += (obj, sender) =>
            {
                if (FilterOptions.Text.Length > 1)
                {
                    //enable button
                    RunButton.IsEnabled = true;
                }
                else
                {
                    RunButton.IsEnabled = false;
                }
            };




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
