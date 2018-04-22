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
    public partial class OrderDataWindow : Window
    {

        internal static bool windowOpen = false; //True when window open.

    
        private IDataRecord finalRecord;


        public OrderDataWindow(IDataRecord record)
        {
            InitializeComponent();

            if (!(record is OrderDataRecord))
            {
                throw new IllegalStateException("Cannot pass this record to this window! Expecting Customer Data Record.");
            }

            //Set global record, this is again the one we modify
            finalRecord = record;

            //Define closecallback for child windows
            Func<IDataRecord, bool> closeCallback = onChildClosed;

            //Set fields









        }



        //Wrapped by a Func Object to be passed to any child windows
        bool onChildClosed(IDataRecord record)
        {

            finalRecord = record;
            return true;
        }

        p
    }




}
