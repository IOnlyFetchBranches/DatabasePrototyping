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
    /// Interaction logic for StoreCardDataWindow.xaml
    /// </summary>
    public partial class StoreCardDataWindow : Window
    {
        //C# uses delegates to act as method points where items taking values use Func<> voids use Actions
        private Func<IDataRecord, bool> closeCallback;

        private IDataRecord finalRecord;

        
        public StoreCardDataWindow(Window callingWindow, IDataRecord record, Func<IDataRecord, bool> closeCallback)
        {
            InitializeComponent();

            if (!(record is CustomerDataRecord) && !(record is OrderDataRecord))
            {
                throw new IllegalStateException("Cannot pass this record to this window! Expecting Customer Data Record or Order Data Record.");
            }
            //Set owner
            Owner = callingWindow;

            //Set callback
            this.closeCallback = closeCallback;

            finalRecord = record;

            Logger.LogG("Opening customer card for " + record.Get("FirstName") + " "+ record.Get("LastName"));

            //Fill all the fields
            CardID.Text = record.Get("CardID");
            CardPoints.Text = record.Get("Points");
            CardBalance.Text = record.Get("Balance");

            //Set on save
            CardSaveButton.Click += (s, e) =>
            {
                //Gather all the fields update them.
                finalRecord.SetField("CardID", CardID.Text);
                finalRecord.SetField("Points", CardPoints.Text);
                finalRecord.SetField("Balance", CardBalance.Text);
                Logger.LogG("Updating card info for " + record.Get("FirstName") + " " + record.Get("LastName"));
                
            
                //close window
                Close();

            };





        }

        /// <summary>
        /// Close the window.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);
            //invoke the callback to pass the new record back to the mother window.
           closeCallback.Invoke(finalRecord);
        }
    }
}
