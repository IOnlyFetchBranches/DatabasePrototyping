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
using SystemException = System.SystemException;

namespace DatabasePrototype.Windows
{
    /// <summary>
    /// Interaction logic for EmployeeDataWindow.xaml
    /// </summary>
    public partial class EmployeeDataWindow : Window
    {
        public EmployeeDataWindow(IDataRecord record)
        {
            InitializeComponent();

            if(!(record is EmployeeDataRecord))
                throw new SystemException("Employee data window needs corresponding record!");

            //TODO: Add more fields to update!
            EmpEID.Text = record.Get("EID");
            EmpEmail.Text = record.Get("PrimaryEmail");
            EmpFirstName.Text = record.Get("FirstName");
            EmpLastName.Text = record.Get("LastName");
            EmpPosition.Text = record.Get("Position");

            switch (record.Get("CurrentStatus"))
            {
                case "Active":
                    EmpStatus.SelectedIndex = 0;
                    break;
                case "Suspended":
                    EmpStatus.SelectedIndex = 1;
                    break;
                case "Terminated":
                    EmpStatus.SelectedIndex = 2;
                    break;
            }


            EmpUpdateButton.Click += (sender, args) =>
            {
                //TODO:Sanitize later!
                record.SetField("FirstName", EmpFirstName.Text);
                record.SetField("LastName", EmpLastName.Text);
                record.SetField("CurrentStatus", EmpStatus.Text);
                record.SetField("PrimaryEmail", EmpEmail.Text);
                record.SetField("Position", EmpPosition.Text);
                //run update routine.
                record.Update();


            };




        }
    }
}
