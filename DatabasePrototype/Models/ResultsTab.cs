using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            //Add all binded column to grid view.
            resultGrid.Columns.Add(idColumn);
            resultGrid.Columns.Add(primaryColumn);
            resultGrid.Columns.Add(secondaryColumn);


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
                }


                //Push to container
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
        /// Call this method to prepare a tab's results, once you are done adding.
        /// ONLY CALL THIS ONCE PER RESULT TAB.
        /// </summary>
        public void Prepare()
        {
            if (Content != null)
                throw new ThreadStateException("Cannot Prepare Twice.");

            //Set as data mold
            resultContainer.View = resultGrid;

            //Set content base;
            Content = rootGrid;
        }
    }
}
