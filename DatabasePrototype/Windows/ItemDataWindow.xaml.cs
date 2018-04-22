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
using DatabasePrototype.Exceptions;
using DatabasePrototype.Models;

namespace DatabasePrototype.Windows
{
    /// <summary>
    /// Interaction logic for ItemDataWindow.xaml
    /// </summary>
    public partial class ItemDataWindow : Window
    {

        //This window's working final draft of the new record.
        private InventoryDataRecord finalRecord;

        //The onClose Callback
        private Func<InventoryDataRecord, bool> onClose;


        public ItemDataWindow(Window callingWindow, IDataRecord record, Func<InventoryDataRecord,bool> closeCallback)
        {
            InitializeComponent();

            Owner = callingWindow;

            onClose = closeCallback;

            //Check to make sure proper record is passed into the constructor
            if (!(record is InventoryDataRecord))
            {
                throw new IllegalStateException(
                    "Cannot pass this record type into this window, expected InventoryDataRecord!");
            }

            finalRecord = (InventoryDataRecord) record;
            
            //Load all the fields
            ItemID.Text = record.Get("ItemId");
            CatID.Text = record.Get("CatID");
            Description.Text = record.Get("Description");

            //Set save button
            ItemSaveButton.Click += (s, e) =>
            {
                //Gather all the fields, and update them in our final record;
                finalRecord.SetField("ItemId", ItemID.Text);
                finalRecord.SetField("CatID", CatID.Text);
                finalRecord.SetField("Description", Description.Text);

                Close();
    

            };


        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            onClose.Invoke(finalRecord);
        }
    }
}
