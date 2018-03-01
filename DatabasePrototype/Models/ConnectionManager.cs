using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabasePrototype.Models
{
    public abstract class ConnectionManager
    {
        //Stores connections for faster load later.
        private static IDictionary<int, SqlConnection> connections = new ConcurrentDictionary<int, SqlConnection>();

        /// <summary>
        /// Opens or returns a connection, given a Connection String.
        /// </summary>
        /// <param name="connectionString">A valid connection string.</param>
        /// <returns>DB Connection</returns>
        public static SqlConnection Open(string connectionString)
        {

            if (connections.ContainsKey(connectionString.GetHashCode()))
                return connections[connectionString.GetHashCode()];


            try
            {
                //Get connection
                var connection = new SqlConnection(connectionString);
                connection.Open();
                connections.Add(connectionString.GetHashCode(), connection);
                return connections[connectionString.GetHashCode()];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error connecting to database!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(); //Exit
                return null;
            }
        }
    }
}
