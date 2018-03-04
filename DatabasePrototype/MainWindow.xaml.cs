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
using dbutils;
using dbutils.Models;
using DatabasePrototype.Exceptions;
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

            //Init Logger
            string path = Logger.NewFile(null, true);
            //Test
            Logger.LogG("Opened log file at " +path);
            
            //We'll display this log location to the user using a label on our status bar;
            MainStatusLabelLogPath.Content = "Logging To: " + path;
            //Open Db Connection
            db = ConnectionManager.Open(ConnectionStrings.Marcus);

            //Load Functionality for Employees
            InitializeEmployees();





        }


        /// <summary>
        /// This is the method that inits the employee tab functions.
        /// Be prepared to replicate this logic for the other tab init functions.
        /// If we do this right we can reuse a good bit of this logic for all the other classes
        /// This is a segmentation based approach to formulating query logic
        /// The goal is to do minimum hardcoding, unfortunately this means relying on direct input
        /// I've included sanitizing handlers on all text entry for ensuring we don't get malicious input
        /// We can split this up across muliple partial classes if needed, you can only access controls directly
        /// from the main class thats linked to the window.
        /// There may be other ways to split it up among completely different classes, let me [Marcus] know.
        /// One class per initializer, if things get too hectic.
        /// </summary>
        private void InitializeEmployees()
        {

            //Set generic table name
            string mainTable = "Employees";
            //Declare column names to a generic name
            string id = "EID";
            string primaryColumn = "FirstName";
            string secondaryColumn = "LastName";

            //Declare Controls to a generic name;
            //That way we can resuse most of this logic by just assigning the proper control here.
            var SearchBar = EmployeesSearchBar;
            //We do the same for every other control
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

            var OrderBy = EmployeesSortByComboBox;



            var FilterBy = EmployeesFilterByComboBox;

            //Acts as a filter
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
            //The most important part, contains submission logic.
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
                    string orderByChoice = OrderBy.Text;

                    
                    //Remove spaces
                    searchByChoice = searchByChoice.Replace(" ", "");
                    filterByChoice = filterByChoice.Replace(" ", "");
                    orderByChoice = orderByChoice.Replace(" ", "");
                    
                    //Build Query
                    //Hopefully the only part we'll have to hardcode

                    string joinStatement = "";
                    //now join statement needs to be empty by default
                    //how do we sovle the question of knowing WHENwe need a join, being  least hardcody as possible?

                    


                    string selectionStatement = "Select "+id+","+primaryColumn+", "+secondaryColumn+" From " +mainTable + " " + joinStatement;

                    string filterStatement = ""; //triggered if filter by is on

                    if (EmployeesFilterOptionBar.IsEnabled && EmployeesFilterOptionBar.Text.Length > 0)
                    {
                        filterStatement = " And " + filterByChoice + " = '" + EmployeesFilterOptionBar.Text + "'";
                    }


                    string whereStatement = ""; //We'll use conditional logic to formulate this value, then pass it through to our query.

                    if (sanitizedText.Length > 0)
                        //If the user has followed the default search logic
                        //(Type info in search bar, choose search by =>{Whatever else}
                        whereStatement = "Where " + searchByChoice + " = '" + sanitizedText + "' " + filterStatement;
                    else if (sanitizedText.Length == 0 && filterStatement != "")
                    {
                        //If the user wants all users that match a given filter, then type the filter in the option bar
                        whereStatement =
                            "Where " + filterStatement.Replace(" And ",
                                ""); //Its just the filter statement minus the And part;
                    }
                    else
                    {
                        throw new IllegalStateException("Error in Employee query generation conditional logic.");
                    }

                    //This is the final part of our query, the order statement
                    string orderStatement = "";


                    if (OrderBy.SelectedIndex == 1)
                    {
                        //This would be the full name;
                        if (EmployeesCheckBoxIsDesc.IsChecked != null && (bool) EmployeesCheckBoxIsDesc.IsChecked)
                        {
                            //If they want it desc
                            orderStatement = " Order By FirstName Desc, LastName Desc";
                        }
                        else
                        {
                            orderStatement = " Order By FirstName, LastName";
                        }

                    }
                    else if(OrderBy.SelectedIndex != -1)
                    {
                        //anything other than full name
                        if (EmployeesCheckBoxIsDesc.IsChecked != null && (bool)EmployeesCheckBoxIsDesc.IsChecked)
                        {
                            //If they want it desc
                            orderStatement = " Order By "+ orderByChoice +" Desc";
                        }
                        else
                        {
                            orderStatement = " Order By " + orderByChoice; // Ascending order is default.
                        }
                    }
                
                    

                     //QUERY Get Employee By Eid
                    SqlCommand GetEmployeeByEid = new SqlCommand(
                                //Do not forget to space after each statement
                                //Although I've already added proper spacing in the statements themselves
                                //Data we will be returning
                                selectionStatement +
                                //Yes for inserted strings, that you want to be evaluated in sql as string
                                //You still have to concat the ' before AND after!
                                whereStatement +
                                //Now we just concat our formulated order statement
                                orderStatement
                                //That's it.
                                );
                    //Debug ops
                    Logger.LogG("SqlCommand", "Created query:\n" + GetEmployeeByEid.CommandText);

                    //You must link the connection, maybe we can create a connection wrapper that does this for us.
                    //You could also pass as second param in constructor but the idea would be to have it link automatically
                    //It could grab the db connection from the Connection Manager.
                    GetEmployeeByEid.Connection = db;

                    try
                    {
                        //Launch Command, Returns a Reader on the result table
                        var results = GetEmployeeByEid.ExecuteReader(); 
                        
                        //Spawn a new results tab, besure to call Prepare() before adding to the master tab control for that Section!
                        var resultsTab = new ResultsTab();
                    
                    
                    
                        while (results.Read())
                    {
                        var result = new EmployeeResult(results[0], results[1], results[2]); //Create new result with an EMPLOYEE Context.
                        resultsTab.Add(result);
                    }
            
                    //CLOSE the sql command's reader, or subsequent calls to the sql command will fail!
                    results.Close();
                    
                    //Handle naming...
                    
                    //How many other result tabs are open?
                    int count = 1; //init the counter
                    foreach (object tab in EmployeesTabControl.Items)
                    {
                        if (tab is ResultsTab)
                            count++;
                    }

                    //PREPARE the tab BEFORE adding to the MasterTab's TabControl.
                    resultsTab.Prepare("Results " + count);
                    //Add to tabcontrol.
                    EmployeesTabControl.Items.Add(resultsTab);

                    //Switch to new tab, for pazzaz really, but it makes sense.
                    //ie when a user searches, you expect to be brought to the results
                    //imagine if google opened it's results in a new tab and made you switch to it
                    //or when you rightclick and issue an  open in new tab command , you have to manually click the tab
                    //it wouldn't make any sense, so don't forget to always do this.
                    EmployeesTabControl.SelectedIndex = EmployeesTabControl.Items.Count - 1;


                    }
                    catch (SqlException sqe)
                    {
                        //Use EasyBox to handle errors.
                        EasyBox.ShowError(sqe);
                    }

                   
                  
               
                }
                
            };
            //Filter drop down
            var FilterOptions = EmployeesFilterOptionBar;
            //Enables/Disables submit button
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



            //Handler that disables search button if box is empty.
            SearchBar.TextChanged += (sender, args) =>
            {
                if (SearchBar.Text.Length == 0)
                    EmployeesRunButton.IsEnabled = false;
                //If there is text AND search by is filled AND there's no filter
                else if (SearchBy.SelectedItem != null && !EmployeesFilterOptionBar.IsEnabled)
                {
                    RunButton.IsEnabled = true;
                }
                //If there is text AND search by is filled AND the filter contains text
                else if (SearchBy.SelectedItem != null && EmployeesFilterOptionBar.IsEnabled && EmployeesFilterOptionBar?.Text.Length >0 )
                {
                    RunButton.IsEnabled = true;
                }

            };




        }
    

        //Event Handlers
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

       
        /// <summary>
        /// Will clear any default text.
        /// We Don;t have to have this but it makes life easier.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmployeesSearchBar_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //Check for default text or this becomes very annoying!
            if(EmployeesSearchBar.Text.Contains("Type Here Or Choose Filter!"))
                EmployeesSearchBar.Text = "";
        }

        private void EmployeesSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
