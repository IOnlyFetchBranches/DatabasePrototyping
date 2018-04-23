using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
using dbutils.Models;
using DatabasePrototype.Models;

namespace DatabasePrototype
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        //Store login password during testing
        public IDictionary<string, string> users = new Dictionary<string, string>();

        private SqlConnection _connection;

        public Login()
        {
            InitializeComponent();


            _connection = ConnectionManager.Open(ConnectionStrings.Marcus);


            //MAKE sure that the password entry is a PasswordBox
            //Also Set the max text to 16
            LoginUserPassword.MaxLength = 16;
            LoginUserName.MaxLength = 16;

            //Set login button as default
            LoginButton.IsDefault = true;






        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var unameBox = LoginUserName;
            var passBox = LoginUserPassword;

            //Sanitize Entries
            foreach (char c in unameBox.Text)
            {
                if (!Char.IsLetterOrDigit(c))
                {
                    //Prompt user
                    MessageBox.Show("Invalid Username/Password", "Login Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    unameBox.Text = ""; //Reset text
                    passBox.Password = "";
                    return;

                }
            }
            foreach (char c in passBox.Password)
            {
                if (!Char.IsLetterOrDigit(c))
                {

                    //Prompt user
                    MessageBox.Show("Invalid Username/Password", "Login Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    unameBox.Text = ""; //Reset text
                    passBox.Password = "";
                    return;

                }
            }

            //DoLogin
            DoLogin(unameBox.Text, passBox.Password);



        }


        private void DoLogin(string uname, string pass)
        {
            //Generate hash
            var hash = Convert.ToBase64String(
                        new SHA256Managed().ComputeHash(
                            Encoding.ASCII.GetBytes(pass)));
            //Check, in the future this should query the DB
            var getUsers = new SqlCommand("Select * From Logins Where [Password] = '" + hash + "' And Username = '" + uname + "'", _connection);

            var results = getUsers.ExecuteReader();

            if (results.Read())
            {
               
                
                //Launch Main
                new MainWindow().Show();

                //Close this one
                this.Close();
            }
            else
            {
                //Prompt user
                MessageBox.Show("Invalid Username/Password", "Login Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                LoginUserName.Text = ""; //Reset text
                LoginUserPassword.Password = ""; //Reset text
            
            }


            results.Close();



        }
    }
}
