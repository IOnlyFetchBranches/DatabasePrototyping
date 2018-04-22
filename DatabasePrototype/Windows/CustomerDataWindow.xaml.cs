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
    /// Interaction logic for CustomerDataWindow.xaml
    /// </summary>
    public partial class CustomerDataWindow : Window
    {

        internal static bool windowOpen = false; //True when window open.

        private IDataRecord finalRecord;

       
        public CustomerDataWindow(IDataRecord record)
        {
            InitializeComponent();

            if (!(record is CustomerDataRecord))
            {
                throw new IllegalStateException("Cannot pass this record to this window! Expecting Customer Data Record or OrderDataRecord.");
            }

            //Set global record
            finalRecord =  record;

            //Set default button first ( so that enter will trigger it. )
            CustUpdateButton.IsDefault = true;

            //Check the record for a storecard
            var cardID = finalRecord.Get("CardID");
            if (!string.IsNullOrEmpty(cardID))
            {

                //Callback method
                Func<IDataRecord,bool> CloseCallback = onCardInfoClosed;
                //Enable the card info button and set it's oncick
                CustButtonViewCard.IsEnabled = true;
                //We open the new window and pass it a reference to the record
                CustButtonViewCard.Click += (s, e) =>
                {
                    new StoreCardDataWindow(this,finalRecord, CloseCallback).Show();
                    windowOpen = true;
                };
            }
            else
            {
                //disable the button
                CustButtonViewCard.IsEnabled = false;
            }




            //Load all the fields
            CustCID.Text = finalRecord.Get("CID");
            CustFirstName.Text = finalRecord.Get("FirstName");
            CustLastName.Text = finalRecord.Get("LastName");
            CustEmail.Text = finalRecord.Get("Email");
            CustCellPhone.Text = finalRecord.Get("CellPhone");
            CustLastVisit.Text = finalRecord.Get("LastVisited");


            //Set update button

            CustUpdateButton.Click += (s, e) =>
            {
                finalRecord.SetField("CID", CustCID.Text);
                finalRecord.SetField("FirstName", CustFirstName.Text);
                finalRecord.SetField("LastName", CustLastName.Text);
                finalRecord.SetField("Email", CustEmail.Text);
                finalRecord.SetField("CellPhone", CustCellPhone.Text);
                finalRecord.SetField("LastVisited", CustLastVisit.Text);

                //Run update process
                finalRecord.Update();

                //Close the window
                Close();

            };



        }



        //Wrapped by a Func Object to be passed to any child windows
        bool onCardInfoClosed(IDataRecord record)
        {
            if (record is CustomerDataRecord || record is OrderDataRecord)
            {
                finalRecord = record;
                return true;
            }
            else
            {
                throw new IllegalStateException("Cannot pass this record to this window, expected Customer Data Record or Order Data Record");
            }
        }



    }


  

}
