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
using System.Windows.Shapes;
using DatabasePrototype.Models;

namespace DatabasePrototype.Windows
{

    /// <summary>
    /// Interaction logic for OrderInformationWindow.xaml
    /// </summary>
    public partial class OrderInformationWindow : Window
    {
        private OrderDataRecord finalRecord;


        private Func<OrderDataRecord, bool> closeCallback;

        /// <summary>
        /// This will only take OrderDataRecords due to complexity.
        /// </summary>
        /// <param name="record"></param>
        public OrderInformationWindow(OrderDataRecord record, Func<OrderDataRecord,bool> callback)
        {
            InitializeComponent();

            finalRecord = record;

            closeCallback = callback;
            //Load Item list
            var list = record.GetLineItems();
            //Will show results in this
            var resultContainer = OrderLineList;
            //The actual container
            var resultGrid = new GridView();

            //Create columns, and bind them
            var nameCol = new GridViewColumn();
            //This sets the field this column will get data from
            nameCol.DisplayMemberBinding = new Binding("ItemName");
            nameCol.Header = "Item";

            var quantityCol = new GridViewColumn();
            //This sets the field this column will get data from
            quantityCol.DisplayMemberBinding = new Binding("Quantity");
            quantityCol.Header = "Quantity";

            var priceCol = new GridViewColumn();
            //This sets the field this column will get data from
            priceCol.DisplayMemberBinding = new Binding("ItemPrice");
            priceCol.Header = "Item Price";

            var returnCol = new GridViewColumn();
            //This sets the field this column will get data from
            returnCol.DisplayMemberBinding = new Binding("IsReturned");
            returnCol.Header = "Returned?";



            //Add all binded columns to grid view.
            resultGrid.Columns.Add(nameCol);
            resultGrid.Columns.Add(quantityCol);
            resultGrid.Columns.Add(priceCol);
            resultGrid.Columns.Add(returnCol);

            //fill the data.
            foreach (var item in list)
            {
                resultContainer.Items.Add(item);
            }

            //Add gridView to list
            resultContainer.View = resultGrid;


            //Set onclick
            resultContainer.MouseDoubleClick += (sender, args) =>
            {
                var item = (LineItem)resultContainer.SelectedItem;
                //Open a connection to the DB
                var connection = ConnectionManager.OpenLast();
                try
                {
                    //Set selected item data
                    OrderItemDesc.Text = item.GetField("Description");
                    OrderItemID.Text = item.GetField("ItemID");
                }
                catch (NullReferenceException nre)
                {
                    //The system will throw a nre if the user clicks on the header, issue that should be addressed

                }


            };










        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            closeCallback.Invoke(finalRecord);
        }
    }
}
