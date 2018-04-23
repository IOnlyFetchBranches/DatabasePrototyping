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
    /// Interaction logic for ShippingDataWindow.xaml
    /// </summary>
    public partial class ShippingDataWindow : Window
    {

        private IDataRecord finalRecord;

        private readonly Func<IDataRecord, bool> onClose;


        public ShippingDataWindow(Window callingWindow, OrderDataRecord record, Func<IDataRecord,bool> closeCallback)
        {
            InitializeComponent();

            onClose = closeCallback; //Set callback

            Owner = callingWindow; //Set owner

            //Load fields
            ShipCity.Text = record.Get("City");
            ShipState.Text = record.Get("State");
            ShipStreet.Text = record.Get("Street");
            ShipZip.Text = record.Get("Zip");

            finalRecord = record;

            //Set on clicks
            ShipButtonSave.Click += (s, a) =>
            {
                finalRecord.SetField("City",ShipCity.Text);
                finalRecord.SetField("State", ShipState.Text);
                finalRecord.SetField("Street", ShipStreet.Text);
                finalRecord.SetField("Zip", ShipZip.Text);
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
