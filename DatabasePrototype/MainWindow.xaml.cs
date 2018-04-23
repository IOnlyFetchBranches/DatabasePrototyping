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
    /*
        Triple slashes [///] denote Summary comments, and you can see them when calling a method in the summary box
        Use these for DOCUMENTATION of methods/class/variables.
        Don't Forget:
        DENOTE EACH QUERY IN CODE WITH A //QUERY {For what/Question} HEADER
        So That we can search the code efficiently!
        Also Don't forget to mark the question answered with a G, in the questions file
        This is a partial class and can be split across multiple files, if we need too.
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
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
           
            //TODO:Set your connection string here!
            //Open Db Connection, only call this once then you can call OpenLast()
            db = ConnectionManager.OpenLast();

            //Load Functionality for Employees
            InitializeEmployees();

            //Load Functionality for Customers
            InitializeCustomers();

        
            //Load Functionality for Orders
            InitializeOrders();

            //Load Functionality for the Inventories
            InitializeInventory();

            //Load Functionality for Stores
            InitializeStores();
            




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


            //One way of doing dynamic joins?
            //The keys in this dictionary represent a selection value, such as Zipcode
            //The values are the table that would be needed to retireve this value, For zip thats EmployeeContacts
            //We could check contains<Selection value> and them automatically append the correct table to the join statement?
            Dictionary<string, string> joinList = new Dictionary<string, string>();

            //Add conditions, remeber these are just column names that you need to account for.
            //Good example, even though i show ZipCode to the user, We sanitize that to a proper column name "Zip"
            //THEN we query the joinList for the correct table to join to.
            //ALL this is taken care of, or should be, by the rest of the logic so just ensure you have the proper columns here
            //Then Go down to the "Sanitization Section" and  ensure you're sanitizing any "Facade Names" to a proper column name where you need to
            //OTHERWISE it wont match on the join list, and you WONT get a join!
            //Of course if we have time we can refine the system to make it more intelligent, but I'd rather implement that once we get all
            //50 Questions handled
        
            joinList.Add("Zip", "EmployeeContacts");    

            //Declare Controls to a generic name;
            //That way we can resuse most of this logic by just assigning the proper control here.
            var SearchBar = EmployeesSearchBar;
            //We do the same for every other control
            var SearchBy = EmployeesSearchByComboBox;
            
            var OrderBy = EmployeesSortByComboBox;

            var FilterBy = EmployeesFilterByComboBox;

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
                    //SANITIZATION SECTION
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

                    //If you have any 'facade names' that is , inputs that don't share the same screen name as the column name, handle that here
                    if (filterByChoice == "ZipCode")
                        filterByChoice = "Zip"; //Change to the shortened form

                    //Build Query
                    //Hopefully the only part we'll have to hardcode
                    
                    //STATEMENT SECTION

                    string joinStatement = "";
                    //now join statement needs to be empty by default
                    //how do we sovle the question of knowing WHENwe need a join, being  least hardcody as possible?

                    //All joins will be done by some sort of ID

                    //conditionals for all entries that will require touching another table and making sure that table is accounted for
                    if (joinList.ContainsKey(filterByChoice))
                    {
                        //On <jtable>.<idvalue> = <maintable>.<idvalue>
                        var onStatement = " On " + joinList[filterByChoice] + "." + id + " = " + mainTable + "." + id;
                        // <maintabl> , <jtable> + [OnStatement]
                        joinStatement += " join " + joinList[filterByChoice] + " " + onStatement + " ";

                    }

                    string fromStatement = " From " + mainTable + " " + joinStatement;
                    //Explanation for +Maintable + "." + id, To prevent ambiguous column name in case of join
                    string selectionStatement = "Select "+mainTable+"."+id+","+primaryColumn+", "+secondaryColumn + " " + fromStatement;
                    //The "And" Part of the where OR the WHOLE where if there is no search by chosen.
                    string filterStatement = ""; //triggered if filter by is on

                    if (EmployeesFilterOptionBar.IsEnabled && EmployeesFilterOptionBar.Text.Length > 0)
                    {
                        if ((bool)EmployeesWildCardCheckBox.IsChecked)
                        {
                            filterStatement = " And " + filterByChoice + " like '" + EmployeesFilterOptionBar.Text + "%'";
                        }

                        else
                        {
                            filterStatement = " And " + filterByChoice + " = '" + EmployeesFilterOptionBar.Text + "'";
                        }
                    }


                    string whereStatement = ""; //We'll use conditional logic to formulate this value, then pass it through to our query.

                    if (sanitizedText.Length > 0)
                        //If the user has followed the default search logic
                        //(Type info in search bar, choose search by =>{Whatever else}
                    if ((bool)EmployeesWildCardCheckBox.IsChecked)
                    {
                        whereStatement = "Where " + searchByChoice + " like '" + sanitizedText + "%' " + filterStatement;
                    }

                        else
                    {
                        whereStatement = "Where " + searchByChoice + " = '" + sanitizedText + "' " + filterStatement;
                    }
                    else if (sanitizedText.Length == 0 && filterStatement != "")
                    {
                        //If the user wants all users that match a given filter, then type the filter in the option bar
                        whereStatement =
                            "Where " + filterStatement.Replace(" And ",
                                ""); //Its just the filter statement minus the And part;
                    }
                    else
                    {
                        MessageBox.Show("Enter a valid Employee query.");
                        return;
                      //throw new IllegalStateException("Error in Employee query generation conditional logic.");
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
                
                    //NOW WEBUILD THE QUERY
                    
                     //QUERY
                     SqlCommand GetEmployees = new SqlCommand(
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
                    //Debug ops, and for easy referencing later.
                    Logger.LogG("SqlCommand",
                        "Created query:" + Environment.NewLine + GetEmployees.CommandText + Environment.NewLine);

                    //You must link the connection, maybe we can create a connection wrapper that does this for us.
                    //You could also pass as second param in constructor but the idea would be to have it link automatically
                    //It could grab the db connection from the Connection Manager.
                    GetEmployees.Connection = db;

                    try
                    {
                        //Launch Command, Returns a Reader on the result table
                        var results = GetEmployees.ExecuteReader(); 
                        
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
                        MessageBox.Show("Error with Employee query.");
                       //Application.Current.Shutdown(1);
                    }

                   
                  
               
                }
                
            };
        }


        private void InitializeCustomers()
        {

                //Set generic table name
                string mainTable = "Customers";
                //Declare column names to a generic name
                string id = "CID";
                string primaryColumn = "FirstName";
                string secondaryColumn = "LastName";


                //One way of doing dynamic joins?
                //The keys in this dictionary represent a selection value, such as Zipcode
                //The values are the table that would be needed to retireve this value, For zip thats EmployeeContacts
                //We could check contains<Selection value> and them automatically append the correct table to the join statement?
                Dictionary<string, string> joinList = new Dictionary<string, string>();

                //Add conditions, remeber these are just column names that you need to account for.
                //Good example, even though i show ZipCode to the user, We sanitize that to a proper column name "Zip"
                //THEN we query the joinList for the correct table to join to.
                //ALL this is taken care of, or should be, by the rest of the logic so just ensure you have the proper columns here
                //Then Go down to the "Sanitization Section" and  ensure you're sanitizing any "Facade Names" to a proper column name where you need to
                //OTHERWISE it wont match on the join list, and you WONT get a join!
                //Of course if we have time we can refine the system to make it more intelligent, but I'd rather implement that once we get all
                //50 Questions handled

                joinList.Add("Total", "Orders");

                //Declare Controls to a generic name;
                //That way we can resuse most of this logic by just assigning the proper control here.
                var SearchBar = CustomersSearchBar;
                //We do the same for every other control
                var SearchBy = CustomersSearchBy;

                var OrderBy = CustomersSortBy;

                var FilterBy = CustomersFilterBy;

                var RunButton = CustomersRunButton;

                //Remember to set default button for each home tab, so that enter will trigger.
                RunButton.IsDefault = true;
                //The most important part, contains submission logic.
                RunButton.Click += (obj, sender) =>
                {
                    //The run button for each tab is responsible for sanitizing input, building the query and launching the result tab
                    //First case is when there is no filter by
                    if (true)
                    {
                        //SANITIZATION SECTION
                        var sanitizedText = CustomersSearchBar.Text;

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

                        //If you have any 'facade names' that is , inputs that don't share the same screen name as the column name, handle that here
                        if (filterByChoice == "Top Purchase")
                            filterByChoice = "Total"; //Change to the shortened form

                        //Build Query
                        //Hopefully the only part we'll have to hardcode

                        //STATEMENT SECTION

                        string joinStatement = "";
                        //now join statement needs to be empty by default
                        //how do we sovle the question of knowing WHENwe need a join, being  least hardcody as possible?

                        //All joins will be done by some sort of ID

                        //conditionals for all entries that will require touching another table and making sure that table is accounted for
                        if (joinList.ContainsKey(filterByChoice))
                        {
                            //On <jtable>.<idvalue> = <maintable>.<idvalue>
                            var onStatement = " On " + joinList[filterByChoice] + "." + id + " = " + mainTable + "." + id;
                            // <maintabl> , <jtable> + [OnStatement]
                            joinStatement += " join " + joinList[filterByChoice] + " " + onStatement + " ";

                        }

                        string fromStatement = " From " + mainTable + " " + joinStatement;
                        //Explanation for +Maintable + "." + id, To prevent ambiguous column name in case of join
                        string selectionStatement = "Select " + mainTable + "." + id + "," + primaryColumn + ", " + secondaryColumn + " " + fromStatement;
                        //The "And" Part of the where OR the WHOLE where if there is no search by chosen.
                        string filterStatement = ""; //triggered if filter by is on

                        if (CustomersFilterOptionBar.IsEnabled && CustomersFilterOptionBar.Text.Length > 0)
                        {
                            if ((bool)CustomersWildCardCheckBox.IsChecked)
                            {
                                filterStatement = " And " + filterByChoice + " like '" + EmployeesFilterOptionBar.Text + "%'";
                            }

                            else
                            {
                                filterStatement = " And " + filterByChoice + " = '" + EmployeesFilterOptionBar.Text + "'";
                            }
                        }


                        string whereStatement = ""; //We'll use conditional logic to formulate this value, then pass it through to our query.

                        if (sanitizedText.Length > 0)
                     
                        if ((bool)CustomersWildCardCheckBox.IsChecked)
                        {
                            whereStatement = "Where " + searchByChoice + " like '" + sanitizedText + "%' " + filterStatement;
                        }

                        else
                        {
                            whereStatement = "Where " + searchByChoice + " = '" + sanitizedText + "' " + filterStatement;
                        }
                        else if (sanitizedText.Length == 0 && filterStatement != "")
                        {
                            //If the user wants all users that match a given filter, then type the filter in the option bar
                            whereStatement =
                                "Where " + filterStatement.Replace(" And ",
                                    ""); //Its just the filter statement minus the And part;
                        }
                        else
                        {
                            MessageBox.Show("Enter a valid Customer query.");
                            return;
                            //throw new IllegalStateException("Error in Customer query generation conditional logic.");
                        }

                        //This is the final part of our query, the order statement
                        string orderStatement = "";


                        if (OrderBy.SelectedIndex == 1)
                        {
                            //This would be the full name;
                            if (CustomersCheckBoxIsDesc.IsChecked != null && (bool)CustomersCheckBoxIsDesc.IsChecked)
                            {
                                //If they want it desc
                                orderStatement = " Order By FirstName Desc, LastName Desc";
                            }
                            else
                            {
                                orderStatement = " Order By FirstName, LastName";
                            }

                        }
                        else if (OrderBy.SelectedIndex != -1)
                        {
                            //anything other than full name
                            if (CustomersCheckBoxIsDesc.IsChecked != null && (bool)CustomersCheckBoxIsDesc.IsChecked)
                            {
                                //If they want it desc
                                orderStatement = " Order By " + orderByChoice + " Desc";
                            }
                            else
                            {
                                orderStatement = " Order By " + orderByChoice; // Ascending order is default.
                            }
                        }

                        //NOW WEBUILD THE QUERY

                        //QUERY
                        SqlCommand GetCustomers = new SqlCommand(
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
                        //Debug ops, and for easy referencing later.
                        Logger.LogG("SqlCommand",
                            "Created query:" + Environment.NewLine + GetCustomers.CommandText + Environment.NewLine);

                        //You must link the connection, maybe we can create a connection wrapper that does this for us.
                        //You could also pass as second param in constructor but the idea would be to have it link automatically
                        //It could grab the db connection from the Connection Manager.
                        GetCustomers.Connection = db;

                        try
                        {
                            //Launch Command, Returns a Reader on the result table
                            var results = GetCustomers.ExecuteReader();

                            //Spawn a new results tab, besure to call Prepare() before adding to the master tab control for that Section!
                            var resultsTab = new ResultsTab();



                            while (results.Read())
                            {
                                var result = new CustomerResult(results[0], results[1], results[2]); //Create new result with an EMPLOYEE Context.
                                resultsTab.Add(result);
                            }

                            //CLOSE the sql command's reader, or subsequent calls to the sql command will fail!
                            results.Close();

                            //Handle naming...

                            //How many other result tabs are open?
                            int count = 1; //init the counter
                            foreach (object tab in CustomersTabControl.Items)
                            {
                                if (tab is ResultsTab)
                                    count++;
                            }

                            //PREPARE the tab BEFORE adding to the MasterTab's TabControl.
                            resultsTab.Prepare("Results " + count);
                            //Add to tabcontrol.
                            CustomersTabControl.Items.Add(resultsTab);

                            //Switch to new tab, for pazzaz really, but it makes sense.
                            //ie when a user searches, you expect to be brought to the results
                            //imagine if google opened it's results in a new tab and made you switch to it
                            //or when you rightclick and issue an  open in new tab command , you have to manually click the tab
                            //it wouldn't make any sense, so don't forget to always do this.
                            CustomersTabControl.SelectedIndex = CustomersTabControl.Items.Count - 1;


                        }
                        catch (SqlException sqe)
                        {
                            //Use EasyBox to handle errors.
                            EasyBox.ShowError(sqe);
                            MessageBox.Show("Error with Customer query.");
                            //Application.Current.Shutdown(1);
                        }




                    }

                };
                //Filter drop down
                var FilterOptions = CustomersFilterOptionBar;
   
        

            }


        private void InitializeOrders()
        {
            //Fix mystery tab.
            OrdersTabControl.Items.RemoveAt(1);
            //Set generic table name
            string mainTable = "Orders";
            //Declare column names to a generic name
            string id = "OID";
            string primaryColumn = "CID";
            string secondaryColumn = "Total";


            //One way of doing dynamic joins?
            //The keys in this dictionary represent a selection value, such as Zipcode
            //The values are the table that would be needed to retireve this value, For zip thats EmployeeContacts
            //We could check contains<Selection value> and them automatically append the correct table to the join statement?
            Dictionary<string, string[]> joinList = new Dictionary<string, string[]>();

            //Add conditions, remeber these are just column names that you need to account for.
            //Good example, even though i show ZipCode to the user, We sanitize that to a proper column name "Zip"
            //THEN we query the joinList for the correct table to join to.
            //ALL this is taken care of, or should be, by the rest of the logic so just ensure you have the proper columns here
            //Then Go down to the "Sanitization Section" and  ensure you're sanitizing any "Facade Names" to a proper column name where you need to
            //OTHERWISE it wont match on the join list, and you WONT get a join!
            //Of course if we have time we can refine the system to make it more intelligent, but I'd rather implement that once we get all
            //50 Questions handled

            joinList.Add("FirstName", new string[] {"CID","Customers"} );
            joinList.Add("LastName", new string[] { "CID", "Customers" });
            joinList.Add("Email", new string[] { "CID", "Customers" });
            joinList.Add("CellPhone", new string[] { "CID", "Customers" });
            joinList.Add("DaysVisitedThisMonth", new string[] { "CID", "Customers" });
            joinList.Add("ItemPrice", new string[] { "OID", "OrderInformation" });
            joinList.Add("isReturned", new string[] { "OID", "OrderInformation" });
            joinList.Add("Quantity", new string[] { "OID", "OrderInformation" });

            //Declare Controls to a generic name;
            //That way we can resuse most of this logic by just assigning the proper control here.
            var SearchBar = OrdersSearchBar;
            //We do the same for every other control
            var SearchBy = OrdersSearchBy;

            var OrderBy = OrdersSortBy;



            var FilterBy = OrdersFilterBy;

            var FilterOptionBar = OrdersFilterOptionBar;
            //Stores the bounds values
            double b1;
            double b2;
            var Bound1 = Orders_Bound1;
            var Bound2 = Orders_Bound2;
            var BoundBox1 = Orders_BoundBox1;
            var BoundBox2 = Orders_BoundBox2;

            //We check to make sure the bounds contains an input
            if (!String.IsNullOrEmpty(Orders_BoundBox1.Text) || !String.IsNullOrEmpty(Orders_BoundBox2.Text))
            {
                //Now we check to see if the input bounds are numeric
                if (!double.TryParse(Orders_BoundBox1.Text, out b1) || !double.TryParse(Orders_BoundBox2.Text, out b2))
                {
                    MessageBox.Show("Only numerical values are allowed in the bounds fields");
                }
            }

            var RunButton = OrdersRunButton;

            //Remember to set default button for each home tab, so that enter will trigger.
            RunButton.IsDefault = true;
            //The most important part, contains submission logic.
            RunButton.Click += (obj, sender) =>
            {
                //The run button for each tab is responsible for sanitizing input, building the query and launching the result tab
                //First case is when there is no filter by
                if (true)
                {
                    //SANITIZATION SECTION
                    var sanitizedText = OrdersSearchBar.Text;

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

                    //If you have any 'facade names' that is , inputs that don't share the same screen name as the column name, handle that here
                    
                    //Build Query
                    //Hopefully the only part we'll have to hardcode

                    //STATEMENT SECTION

                    string joinStatement = "";
                    //now join statement needs to be empty by default
                    //how do we sovle the question of knowing WHENwe need a join, being  least hardcody as possible?

                    //All joins will be done by some sort of ID

                    //conditionals for all entries that will require touching another table and making sure that table is accounted for

                    string onStatement = "";
                    if (joinList.ContainsKey(searchByChoice))
                    {
                        //On <jtable>.<idvalue> = <maintable>.<idvalue>
                        onStatement = " On " + joinList[searchByChoice][1] + "." + joinList[searchByChoice][0] + " = " + mainTable + "." + joinList[searchByChoice][0];
                        // <maintabl> , <jtable> + [OnStatement]
                        joinStatement += " join " + joinList[searchByChoice][1] + " " + onStatement + " ";

                    }


                    if (joinList.ContainsKey(filterByChoice))
                    {

                        //On <jtable>.<idvalue> = <maintable>.<idvalue>
                        var nOnStatement = " On " + joinList[filterByChoice][1] + "." + joinList[filterByChoice][0] + " = " + mainTable + "." + joinList[filterByChoice][0];
                        //Check if we need to add the join
                        if (onStatement != nOnStatement && !onStatement.Contains(nOnStatement))
                        {
                            onStatement += nOnStatement;
                            // <maintabl> , <jtable> + [OnStatement]
                            joinStatement += " join " + joinList[filterByChoice][1] + " " + onStatement + " ";
                        }
                        

                    }

                

                    string fromStatement = " From " + mainTable + " " + joinStatement;
                    //Explanation for +Maintable + "." + id, To prevent ambiguous column name in case of join
                    string selectionStatement = "Select " + mainTable + "." + id + "," + mainTable + "." + primaryColumn + ", " + secondaryColumn + " " + fromStatement;
                    //The "And" Part of the where OR the WHOLE where if there is no search by chosen.
                    string filterStatement = ""; //triggered if filter by is on

                    if (FilterOptionBar.IsEnabled && FilterOptionBar.Text.Length > 0)
                    {
                        if ((bool)OrdersWildCardCheckBox.IsChecked)
                        {
                            filterStatement = " And " + filterByChoice + " like '" + EmployeesFilterOptionBar.Text + "%'";
                        }

                        else
                        {
                            filterStatement = " And " + filterByChoice + " = '" + EmployeesFilterOptionBar.Text + "'";
                        }
                    }

                    if (FilterOptionBar.IsEnabled && FilterOptionBar.Text.Length > 0
                                                           && Bound1.IsEnabled == false)
                    {
                        filterStatement = " And " + filterByChoice + " = '" + FilterOptionBar.Text + "'";
                    }
                    //User has chosen something that requires or can have a bound
                    else if( Bound1.IsEnabled && BoundBox1.Text.Length > 0)
                    {
                        //If a box hax been selected that requires a bound
                        filterStatement = " And " + filterByChoice + " " + Bound1.Text + " '" +
                                          BoundBox1.Text + " '";

                       

                    } //Check if a secondary bound has been set
                   else if (BoundBox2.IsEnabled && BoundBox2.Text.Length > 0)
                    {
                        //Attach the secondary bound
                        filterStatement += " And " + filterByChoice + " " + Bound2.Text + " '" +
                                           BoundBox2.Text + " '";
                    }

                    string whereStatement = ""; //We'll use conditional logic to formulate this value, then pass it through to our query.

                    if (sanitizedText.Length > 0)
                    {

                        if ((bool) OrdersWildCardCheckBox.IsChecked)
                        {
                            whereStatement = "Where " + searchByChoice + " like '" + sanitizedText + "%' " +
                                             filterStatement;
                        }

                        else
                        {
                            whereStatement = "Where " + searchByChoice + " = '" + sanitizedText + "' " +
                                             filterStatement;
                        }
                    }
                    else if (sanitizedText.Length == 0 && filterStatement != "")
                    {
                        //If the user wants all users that match a given filter, then type the filter in the option bar
                        whereStatement =
                            "Where " + filterStatement.Replace(" And ",
                                ""); //Its just the filter statement minus the And part;
                    }
                    else if(sanitizedText.Length ==0)
                    {

                        whereStatement =
                            "Where " + filterStatement.Replace(" And ",
                                ""); //Its just the filter statement minus the And part;
                    }
                    else
                    {
                        EasyBox.ShowError(new IllegalStateException("Could not generate query!"));
                    }

                    //This is the final part of our query, the order statement
                    string orderStatement = "";

                    if (OrderBy.SelectedIndex != -1)
                    {
                        //anything other than full name
                        if (OrdersCheckBoxIsDesc.IsChecked != null && (bool)OrdersCheckBoxIsDesc.IsChecked)
                        {
                            //If they want it desc
                            orderStatement = " Order By " + orderByChoice + " Desc";
                        }
                        else
                        {
                            orderStatement = " Order By " + orderByChoice; // Ascending order is default.
                        }
                    }

                    //NOW WEBUILD THE QUERY

                    //QUERY
                    SqlCommand GetOrders = new SqlCommand(
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
                    //Debug ops, and for easy referencing later.
                    Logger.LogG("SqlCommand",
                        "Created query:" + Environment.NewLine + GetOrders.CommandText + Environment.NewLine);

                    //You must link the connection, maybe we can create a connection wrapper that does this for us.
                    //You could also pass as second param in constructor but the idea would be to have it link automatically
                    //It could grab the db connection from the Connection Manager.
                    GetOrders.Connection = db;

                    try
                    {
                        //Launch Command, Returns a Reader on the result table
                        var results = GetOrders.ExecuteReader();

                        //Spawn a new results tab, besure to call Prepare() before adding to the master tab control for that Section!
                        var resultsTab = new ResultsTab();



                        while (results.Read())
                        {
                            var result = new OrderResult(results[0], results[1], results[2]); //Create new result with an EMPLOYEE Context.
                            resultsTab.Add(result);
                        }

                        //CLOSE the sql command's reader, or subsequent calls to the sql command will fail!
                        results.Close();

                        //Handle naming...

                        //How many other result tabs are open?
                        int count = 1; //init the counter
                        foreach (object tab in OrdersTabControl.Items)
                        {
                            if (tab is ResultsTab)
                                count++;
                        }

                        //PREPARE the tab BEFORE adding to the MasterTab's TabControl.
                        resultsTab.Prepare("Results " + count);
                        //Add to tabcontrol.
                        OrdersTabControl.Items.Add(resultsTab);

                        //Switch to new tab, for pazzaz really, but it makes sense.
                        //ie when a user searches, you expect to be brought to the results
                        //imagine if google opened it's results in a new tab and made you switch to it
                        //or when you rightclick and issue an  open in new tab command , you have to manually click the tab
                        //it wouldn't make any sense, so don't forget to always do this.
                        OrdersTabControl.SelectedIndex = OrdersTabControl.Items.Count - 1;


                    }
                    catch (SqlException sqe)
                    {
                        //Use EasyBox to handle errors.
                        EasyBox.ShowError(sqe);
                        MessageBox.Show("Error with Orders Query.");
                        //TODO: Remove all these!
                        //Application.Current.Shutdown(1);
                    }




                }

            };
            //Filter drop down
            var FilterOptions = CustomersFilterOptionBar;

        }
        /// <summary>
        /// Setup the base logic for the Inventory tab.
        /// </summary>
        private void InitializeInventory() {

            //mystery tab patch
            InventoryTabControl.Items.RemoveAt(1);


            //Set generic table name
            string mainTable = "InventoryInfo";
            //Declare column names to a generic name
            string id = "InvID";
            string primaryColumn = "ItemId";
            string secondaryColumn = "[Name]";

            //One way for doing bounds, keep a list of values that can be bounded
            List<string> BoundedVals = new List<string>();

            BoundedVals.Add("Quantity");
            BoundedVals.Add("Price");

            //One way of doing dynamic joins?
            //The keys in this dictionary represent a selection value, such as Zipcode
            //The values are the table that would be needed to retireve this value, For zip thats EmployeeContacts
            //We could check contains<Selection value> and them automatically append the correct table to the join statement?
            Dictionary<string, string[]> joinList = new Dictionary<string, string[]>();

            //Add conditions, remeber these are just column names that you need to account for.
            //Good example, even though i show ZipCode to the user, We sanitize that to a proper column name "Zip"
            //THEN we query the joinList for the correct table to join to.
            //ALL this is taken care of, or should be, by the rest of the logic so just ensure you have the proper columns here
            //Then Go down to the "Sanitization Section" and  ensure you're sanitizing any "Facade Names" to a proper column name where you need to
            //OTHERWISE it wont match on the join list, and you WONT get a join!
            //Of course if we have time we can refine the system to make it more intelligent, but I'd rather implement that once we get all
            //50 Questions handled

            joinList.Add(secondaryColumn,new string[]{"ItemId","Items"});
            

          



            //TODO: Add logic to enable/disable fields to help prevent incorrect queries.

            //Declare Controls to a generic name;
            //That way we can resuse most of this logic by just assigning the proper control here.
            var SearchBar = InventorySearchBar;
            //We do the same for every other control
            var SearchBy = InventorySearchBy;

            var OrderBy = InventorySortBy;

            var FilterBy = InventoryFilterBy;

            //Handle our bounds here.
            double b1;
            double b2;
            var Bound1 = Inventory_Bound1;
            var Bound2 = Inventory_Bound2;

            var BoundBox1 = Inventory_BoundBox1;
            var BoundBox2 = Inventory_BoundBox2;

            //Define button
            var RunButton = InventoryRunButton;

            //We check to make sure the bounds contains an input
            if (!String.IsNullOrEmpty(Inventory_BoundBox1.Text) || !String.IsNullOrEmpty(Inventory_BoundBox2.Text))
            {
                //Now we check to see if the input bounds are numeric
                if (!double.TryParse(Inventory_BoundBox2.Text, out b1) || !double.TryParse(Inventory_BoundBox2.Text, out b2))
                {
                    MessageBox.Show("Only numerical values are allowed in the bounds fields");
                }
            }


          







            //Set logic for 


            //Remember to set default button for each home tab, so that enter will trigger.
            RunButton.IsDefault = true;
            //The most important part, contains submission logic.
            RunButton.Click += (obj, sender) =>
            {
                
                //The run button for each tab is responsible for sanitizing input, building the query and launching the result tab
                //First case is when there is no filter by
                if (true)
                {
                    //SANITIZATION SECTION
                    var sanitizedText = InventorySearchBar.Text;

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


                    //Facade names
                    if (orderByChoice.Equals("ItemName"))
                    {
                        orderByChoice = "Name";
                    }

                    //Add brackets for keyworded names
                    filterByChoice = "[" + filterByChoice + "]";

                    searchByChoice = "[" + searchByChoice + "]";

                    orderByChoice = "[" + orderByChoice + "]";

                    //If you have any 'facade names' that is , inputs that don't share the same screen name as the column name, handle that here
                   
                    //Build Query
                    //Hopefully the only part we'll have to hardcode

                    //STATEMENT SECTION
                    //header section


                


                    string joinStatement = "";
                    //now join statement needs to be empty by default
                    //how do we sovle the question of knowing WHENwe need a join, being  least hardcody as possible?

                    //All joins will be done by some sort of ID

                    //conditionals for all entries that will require touching another table and making sure that table is accounted for

                    //On <jtable>.<idvalue> = <maintable>.<idvalue>
                    var onStatement = " "; //Leave empty by default
                    if (joinList.ContainsKey(filterByChoice))
                    {
                       onStatement = " On " + joinList[filterByChoice][1] + "." + joinList[filterByChoice][0] + " = " + mainTable + "." + joinList[filterByChoice][0];
                        // <maintabl> , <jtable> + [OnStatement]
                        joinStatement += " join " + joinList[filterByChoice][1] + " " + onStatement + " ";

                    }

                    if (joinList.ContainsKey(searchByChoice))
                    {
                        onStatement = " On " + joinList[searchByChoice][1] + "." + joinList[searchByChoice][0] + " = " + mainTable + "." + joinList[searchByChoice][0];
                        // <maintabl> , <jtable> + [OnStatement]
                        joinStatement += " join " + joinList[searchByChoice][1] + " " + onStatement + " ";

                    }


                    if (joinList.ContainsKey(secondaryColumn))
                    {
                        onStatement = " On " + joinList[secondaryColumn][1] + "." + joinList[secondaryColumn][0] + " = " + mainTable + "." + joinList[secondaryColumn][0];
                        // <maintabl> , <jtable> + [OnStatement]
                        joinStatement += " join " + joinList[secondaryColumn][1] + " " + onStatement + " ";

                    }






                    string fromStatement = " From " + mainTable + " " + joinStatement;
                    //Explanation for +Maintable + "." + id, To prevent ambiguous column name in case of join
                    //IMPORTANTEXCEPTION ->no mt.column for primaryColumn as it's a joined column
                    string selectionStatement = "Select " + mainTable + "." + id + ", " +  mainTable + "."+primaryColumn  + ", " + secondaryColumn + " " + fromStatement;
                    //The "And" Part of the where OR the WHOLE where if there is no search by chosen.
                    string filterStatement = ""; //triggered if filter by is on

                    if (InventoryFilterOptionBar.IsEnabled && InventoryFilterOptionBar.Text.Length > 0 
                            && Inventory_Bound1.IsEnabled == false)
                    {
                        filterStatement = " And " + filterByChoice + " = '" + InventoryFilterOptionBar.Text + "'";
                    }

                    //User has chosen something that requires or can have a bound
                    if (Inventory_Bound1.IsEnabled && Inventory_BoundBox1.Text.Length >0)
                    {
                        //If a box hax been selected that requires a bound
                        filterStatement += "  " + filterByChoice + " " + Inventory_Bound1.Text + " '" +
                                          Inventory_BoundBox1.Text + " '";

                       
                       
                    }

                    //Check if a secondary bound has been set
                    if (Inventory_BoundBox2.IsEnabled && Inventory_BoundBox2.Text.Length > 0)
                    {
                        //Attach the secondary bound
                        filterStatement += " And " + filterByChoice + " " + Inventory_Bound2.Text + " '" +
                                           Inventory_BoundBox2.Text + " '";
                    }


                    string whereStatement = ""; //We'll use conditional logic to formulate this value, then pass it through to our query.

                    if (sanitizedText.Length > 0)
                        //If the user has followed the default search logic
                        //(Type info in search bar, choose search by =>{Whatever else}
                        whereStatement = "Where " + searchByChoice + " = '" + sanitizedText + "' " + filterStatement;
                    else if (sanitizedText.Length == 0 && InventoryFilterOptionBar.Text.Length > 0)
                    {
                        //If the user wants all users that match a given filter, then type the filter in the option bar
                        whereStatement =
                            "Where " + filterStatement.Replace(" And ",
                                ""); //Its just the filter statement minus the And part;
                    }
                    else if (Bound1.SelectedIndex > 0 && Bound2.SelectedIndex < 1)
                    {
                        
                        whereStatement = "Where " + filterStatement;
                        whereStatement = whereStatement.Replace("And" , "");
                    }
                    else if (Bound1.SelectedIndex > 0 && Bound2.SelectedIndex > 0)
                    {

                        whereStatement = "Where " + filterStatement;
                        whereStatement = "";

                    }
                    else
                    {
                        MessageBox.Show("Enter a valid Inventory query.");
                        return;
                       // throw new IllegalStateException("Error in Inventory query generation conditional logic.");
                    }

                    //This is the final part of our query, the order statement
                    string orderStatement = "";



        
                    if (OrderBy.SelectedIndex != -1)
                    {
                        //anything other than full name
                        if (InventoryCheckBoxIsDesc.IsChecked != null && (bool)InventoryCheckBoxIsDesc.IsChecked)
                        {
                            //If they want it desc
                            orderStatement = " Order By " + orderByChoice + " Desc";
                        }
                        else
                        {
                            orderStatement = " Order By " + orderByChoice; // Ascending order is default.
                        }
                    }

                    //NOW WEBUILD THE QUERY

                    //QUERY
                    SqlCommand GetInventory = new SqlCommand(
                               //Do not forget to space after each statement "if hardcoding" statements should already be properly spaced!
                               //Data we will be returning
                               selectionStatement + 
                               //Yes for inserted strings, that you want to be evaluated in sql as string
                               //You still have to concat the ' before AND after!
                               whereStatement +
                               //Now we just concat our formulated order statement
                               orderStatement
                               //That's it.
                               );
                    //Debug ops, and for easy referencing later.
                    Logger.LogG("SqlCommand",
                        "Created query:" + Environment.NewLine + GetInventory.CommandText + Environment.NewLine);

                    //You must link the connection, maybe we can create a connection wrapper that does this for us.
                    //You could also pass as second param in constructor but the idea would be to have it link automatically
                    //It could grab the db connection from the Connection Manager.
                    GetInventory.Connection = db;

                    try
                    {
                        //Launch Command, Returns a Reader on the result table
                        var results = GetInventory.ExecuteReader();

                        //Spawn a new results tab, besure to call Prepare() before adding to the master tab control for that Section!
                        var resultsTab = new ResultsTab();


                        while (results.Read())
                        {
                            var result = new InventoryResult(results[0], results[1], results[2]); //Create new result with an INVENTORY Context.
                            resultsTab.Add(result);
                        }

                        //CLOSE the sql command's reader, or subsequent calls to the sql command will fail!
                        results.Close();

                        //Handle naming...

                        //How many other result tabs are open?
                        int count = 1; //init the counter
                        foreach (object tab in InventoryTabControl.Items)
                        {
                            if (tab is ResultsTab)
                                count++;
                        }

                        //PREPARE the tab BEFORE adding to the MasterTab's TabControl.
                        resultsTab.Prepare("Results " + count);
                        //Add to tabcontrol.
                        InventoryTabControl.Items.Add(resultsTab);

                        //Switch to new tab, for pazzaz really, but it makes sense.
                        //ie when a user searches, you expect to be brought to the results
                        //imagine if google opened it's results in a new tab and made you switch to it
                        //or when you rightclick and issue an  open in new tab command , you have to manually click the tab
                        //it wouldn't make any sense, so don't forget to always do this.
                        InventoryTabControl.SelectedIndex = InventoryTabControl.Items.Count - 1;


                    }
                    catch (SqlException sqe)
                    {
                        //Use EasyBox to handle errors.
                        EasyBox.ShowError(sqe);
                        MessageBox.Show("Error with Inventory Query");
                        //Application.Current.Shutdown(1);
                    }




                }

            };
            //Filter drop down
            var FilterOptions = InventoryFilterOptionBar;


        }

        private void InitializeStores()
        {

            //Set generic table name
            string mainTable = "StoreInfo";
            //Declare column names to a generic name
            string id = "STRID";
            string primaryColumn = "SState";
            string secondaryColumn = "SZip";


            //One way of doing dynamic joins?
            //The keys in this dictionary represent a selection value, such as Zipcode
            //The values are the table that would be needed to retireve this value, For zip thats EmployeeContacts
            //We could check contains<Selection value> and them automatically append the correct table to the join statement?
            Dictionary<string, string> joinList = new Dictionary<string, string>();

            //Add conditions, remeber these are just column names that you need to account for.
            //Good example, even though i show ZipCode to the user, We sanitize that to a proper column name "Zip"
            //THEN we query the joinList for the correct table to join to.
            //ALL this is taken care of, or should be, by the rest of the logic so just ensure you have the proper columns here
            //Then Go down to the "Sanitization Section" and  ensure you're sanitizing any "Facade Names" to a proper column name where you need to
            //OTHERWISE it wont match on the join list, and you WONT get a join!
            //Of course if we have time we can refine the system to make it more intelligent, but I'd rather implement that once we get all
            //50 Questions handled



            //Declare Controls to a generic name;
            //That way we can resuse most of this logic by just assigning the proper control here.
            var SearchBar = StoresSearchBar;
            //We do the same for every other control
            var SearchBy = StoresSearchBy;

           

            var OrderBy = StoresSortBy;



            var FilterBy = StoresFilterBy;

            var RunButton = StoresRunButton;

            //Remember to set default button for each home tab, so that enter will trigger.
            RunButton.IsDefault = true;
            //The most important part, contains submission logic.
            RunButton.Click += (obj, sender) =>
            {
                //The run button for each tab is responsible for sanitizing input, building the query and launching the result tab
                //First case is when there is no filter by
                if (true)
                {
                    //SANITIZATION SECTION
                    var sanitizedText = StoresSearchBar.Text;

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

                    //If you have any 'facade names' that is , inputs that don't share the same screen name as the column name, handle that here


                    //Build Query
                    //Hopefully the only part we'll have to hardcode

                    //STATEMENT SECTION

                    string joinStatement = "";
                    //now join statement needs to be empty by default
                    //how do we sovle the question of knowing WHENwe need a join, being  least hardcody as possible?

                    //All joins will be done by some sort of ID

                    //conditionals for all entries that will require touching another table and making sure that table is accounted for
                    if (joinList.ContainsKey(filterByChoice))
                    {
                        //On <jtable>.<idvalue> = <maintable>.<idvalue>
                        var onStatement = " On " + joinList[filterByChoice] + "." + id + " = " + mainTable + "." + id;
                        // <maintabl> , <jtable> + [OnStatement]
                        joinStatement += " join " + joinList[filterByChoice] + " " + onStatement + " ";

                    }

                    string fromStatement = " From " + mainTable + " " + joinStatement;
                    //Explanation for +Maintable + "." + id, To prevent ambiguous column name in case of join
                    string selectionStatement = "Select " + mainTable + "." + id + "," + primaryColumn + ", " + secondaryColumn + " " + fromStatement;
                    //The "And" Part of the where OR the WHOLE where if there is no search by chosen.
                    string filterStatement = ""; //triggered if filter by is on

                    if (StoresFilterOptionBar.IsEnabled && StoresFilterOptionBar.Text.Length > 0)
                    {
                        filterStatement = " And " + filterByChoice + " = '" + StoresFilterOptionBar.Text + "'";
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
                        throw new IllegalStateException("Error in Store query generation conditional logic.");
                    }

                    //This is the final part of our query, the order statement
                    string orderStatement = "";


                    if (OrderBy.SelectedIndex != -1)
                    {
                        //anything other than full name
                        if (StoresCheckBoxIsDesc.IsChecked != null && (bool)StoresCheckBoxIsDesc.IsChecked)
                        {
                            //If they want it desc
                            orderStatement = " Order By " + orderByChoice + " Desc";
                        }
                        else
                        {
                            orderStatement = " Order By " + orderByChoice; // Ascending order is default.
                        }
                    }

                    //NOW WEBUILD THE QUERY

                    //QUERY
                    SqlCommand GetStores = new SqlCommand(
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
                    //Debug ops, and for easy referencing later.
                    Logger.LogG("SqlCommand",
                        "Created query:" + Environment.NewLine + GetStores.CommandText + Environment.NewLine);

                    //You must link the connection, maybe we can create a connection wrapper that does this for us.
                    //You could also pass as second param in constructor but the idea would be to have it link automatically
                    //It could grab the db connection from the Connection Manager.
                    GetStores.Connection = db;

                    try
                    {
                        //Launch Command, Returns a Reader on the result table
                        var results = GetStores.ExecuteReader();

                        //Spawn a new results tab, besure to call Prepare() before adding to the master tab control for that Section!
                        var resultsTab = new ResultsTab();



                        while (results.Read())
                        {
                            var result = new StoreResult(results[0], results[1], results[2]); //Create new result with an EMPLOYEE Context.
                            resultsTab.Add(result);
                        }

                        //CLOSE the sql command's reader, or subsequent calls to the sql command will fail!
                        results.Close();

                        //Handle naming...

                        //How many other result tabs are open?
                        int count = 1; //init the counter
                        foreach (object tab in StoresTabControl.Items)
                        {
                            if (tab is ResultsTab)
                                count++;
                        }

                        //PREPARE the tab BEFORE adding to the MasterTab's TabControl.
                        resultsTab.Prepare("Results " + count);
                        //Add to tabcontrol.
                        StoresTabControl.Items.Add(resultsTab);

                        //Switch to new tab, for pazzaz really, but it makes sense.
                        //ie when a user searches, you expect to be brought to the results
                        //imagine if google opened it's results in a new tab and made you switch to it
                        //or when you rightclick and issue an  open in new tab command , you have to manually click the tab
                        //it wouldn't make any sense, so don't forget to always do this.
                        StoresTabControl.SelectedIndex = StoresTabControl.Items.Count - 1;


                    }
                    catch (SqlException sqe)
                    {
                        //Use EasyBox to handle errors.
                        EasyBox.ShowError(sqe);
                        Application.Current.Shutdown(1);
                    }




                }

            };
            //Filter drop down
            var FilterOptions = StoresFilterOptionBar;




        }






        ///////////////////////EVENT HANDLERS//////////////////////////////////////////////////////////////////


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

        private void CustomersSearchBar_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //Check for default text or this becomes very annoying!
            if (CustomersSearchBar.Text.Contains("Type Here Or Choose Filter!"))
                CustomersSearchBar.Text = "";
        }

        private void CustomersSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OrdersFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void EmployeesSearchBar_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}
