using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using dbutils.Models;
using DatabasePrototype.Windows;

namespace DatabasePrototype.Models
{
    /// <summary>
    /// Wrapper Class for displaying results.
    /// This class is sealed. Do not extend this class.
    /// </summary>
    public sealed class ResultsTab : TabItem
    {
        //Globals
        private Grid rootGrid;

        private ListView resultContainer;

        private GridView resultGrid;

        private DataTable resultData;

        public ResultsTab()
        {
            //Setup default   
            
            //Create Base Grid
            rootGrid = new Grid();
            //This included for readability.
            rootGrid.Width = this.Width;
            rootGrid.Height = this.Height;


            //Will show results in this
            resultContainer = new ListView();
            //The actual container
            resultGrid = new GridView();

            //Create columns, and bind them
            var idColumn = new GridViewColumn();
            //This sets the field this column will get data from
            idColumn.DisplayMemberBinding = new Binding("IdentifyingMember");

            var primaryColumn = new GridViewColumn();
            //This sets the field this column will get data from
            primaryColumn.DisplayMemberBinding = new Binding("PrimaryMember");

            var secondaryColumn = new GridViewColumn();
            //This sets the field this column will get data from
            secondaryColumn.DisplayMemberBinding = new Binding("SecondaryMember");

            //Add all binded columns to grid view.
            resultGrid.Columns.Add(idColumn);
            resultGrid.Columns.Add(primaryColumn);
            resultGrid.Columns.Add(secondaryColumn);

            

            //Set onclick
            resultContainer.MouseDoubleClick += (sender, args) =>
            {
                var item = (IResult) resultContainer.SelectedItem;
                //Open a connection to the DB
                var connection = ConnectionManager.OpenLast();
                try
                {
                    //Run query to retrieve row data
                   
                    IDataRecord record;

                    //TODO: Implement DataRecord For each of the five major categories.
                    SqlCommand command;
                    SqlDataReader results;
                    switch (item.Table)
                    {
                        case "Employees":
                            command = new SqlCommand(
                                "Select * From " + item.Table
                                                 + " Where " + item.IdHeader() + " = '" + item.IdentifyingMember + "'" //Dont forget your '
                                , connection);

                            results = command.ExecuteReader();
                            //In this case the item was an employee record
                            record = new EmployeeDataRecord(results,connection);
                            //TODO:Create a window that accepts Employee Data
                            new EmployeeDataWindow(record).Show();
                            break;

                        case "Customers":
                             command = new SqlCommand(
                                "Select * From " + item.Table
                                                 + " Where " + item.IdHeader() + " = '" + item.IdentifyingMember + "'" //Dont forget your '
                                , connection);

                             results = command.ExecuteReader();
                            record = new CustomerDataRecord(results, connection);
                            new CustomerDataWindow(record).Show();
                            break;

                        case "InventoryInfo":
                            command = new SqlCommand(
                                "Select * From " + item.Table
                                                 + " Where " + item.IdHeader() + " = '" + item.IdentifyingMember + "'" //Dont forget your '
                                , connection);

                            results = command.ExecuteReader();
                            record = new InventoryDataRecord(results, connection);
                            new InventoryDataWindow(record).Show();
                            break;
                    }


                }
                catch (NullReferenceException nre)
                {
                    //The system will throw a nre if the user clicks on the header, issue that should be addressed

                }


            };



        }
        /// <summary>
        /// Adds a result to a result tab.
        /// When finished adding to a ResultTab, call Prepare()!
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <returns>True if add is successful, false if otherwise.</returns>
        public bool Add(IResult item)
        {
            try
            {

                //the ? acts as a safety net to nulls, so this may not be neccesary but just in case...
                if (resultGrid.Columns[0].Header == null || (string) resultGrid.Columns[0]?.Header != item.IdHeader())
                {
                    //Name columns dynamically.
                    resultGrid.Columns[0].Header = item.IdHeader();
                    resultGrid.Columns[1].Header = item.PrimaryHeader();
                    resultGrid.Columns[2].Header = item.SecondaryHeader();

                    MessageBox.Show("Added " + item.IdentifyingMember + " " + item.PrimaryMember + " " +
                                    item.SecondaryMember);
                }

                resultContainer.Items.Add(item);
               

                //Return success
                return true;
            }
            catch (NullReferenceException nre)
            {
                MessageBox.Show("NullReferenceException:\n" + nre.Message, "An exception has occurred!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }





        /// <summary>
        /// Call this method to prepare a tab's results, once you are done adding, and name the tab using Name();
        /// ONLY CALL THIS ONCE PER RESULT TAB.
        /// </summary>
        public void Prepare(string headingTitle)
        {

            if (Content != null)
                throw new ThreadStateException("Cannot Prepare Twice.");

            //Set as data mold
            resultContainer.View = resultGrid;

            //Set Header to add close button
            var heading = new StackPanel();
            heading.Orientation = Orientation.Horizontal; //Display in line

            var headingLabel = new Label();
            headingLabel.Content = headingTitle + "\t"; //Set title
            heading.Children.Add(headingLabel); //Add to grid

            //Add closeing label
            var closeLabel = new Label();
            closeLabel.Content = "x";
            //Format it correctly
            closeLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
            closeLabel.VerticalContentAlignment = VerticalAlignment.Center;

            //Set onclick, to close tab
            closeLabel.MouseDown += (sender, args) =>
            {
                var parent = (TabControl) this.Parent;
                int index = 0;
                foreach (var tab in parent.Items)
                {
                    if (tab.GetHashCode() == this.GetHashCode())
                    {
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
                //Before you remove send user to the immediate left tab.
                parent.SelectedIndex = index - 1;

                //remove tab
                parent.Items.RemoveAt(index);
            };
            //Add Close Label
            heading.Children.Add(closeLabel);
            //Set heading
            this.Header = heading;
            


            //Set content basing
            rootGrid.Children.Add(resultContainer);
            Content = rootGrid;
            //DEBUG
              MessageBox.Show("Header "+this.Header.GetType() + " grid Items: " + resultContainer.Items.Count );
        }
    }
}
