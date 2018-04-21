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
using dbutils;
using DatabasePrototype.Exceptions;
using DatabasePrototype.Models;

namespace DatabasePrototype.Windows
{
    /// <summary>
    /// Interaction logic for InventoryDataWindow.xaml
    /// </summary>
    public partial class InventoryDataWindow : Window
    {

        private InventoryDataRecord finalRecord;

        public InventoryDataWindow(IDataRecord record)
        {
            InitializeComponent();


            //Check to make sure the correct version of the DataRecord is passed
            if (!(record is InventoryDataRecord))
            {
                throw new IllegalStateException("Cannot pass this type of Data Record to this window, expected InventoryDataRecord.");
            }

            finalRecord = (InventoryDataRecord) record;
            //Load all the fields

            InvID.Text = finalRecord.Get("InvID");
            ItemQuantity.Text = finalRecord.Get("Quantity");
            ItemName.Text = finalRecord.Get("Name");
            LastRestock.Text = finalRecord.Get("LastRestocked");
            ItemPrice.Text = finalRecord.Get("Price");

            //Set buttons
            ItemDetailsButton.Click += (s, a) => { new ItemDataWindow(this, finalRecord, onClose).Show(); };

            UpdateButton.Click += (s, a) =>
            {
                finalRecord.SetField("InvID", InvID.Text);
                finalRecord.SetField("Quantity", ItemQuantity.Text);
                finalRecord.SetField("Name", ItemName.Text);
                finalRecord.SetField("LastRestocked", LastRestock.Text);
                finalRecord.SetField("Price", ItemPrice.Text);

                //Call update routine on final record to finalize in database
                finalRecord.Update();

                Close(); //Close window!
            };


        }


        private bool onClose(InventoryDataRecord updatedRecord)
        {
            finalRecord = updatedRecord;
            Logger.LogG("Item Info closed...");

            return true;
        }
    }

}
