using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DatabasePrototype.Exceptions;

namespace DatabasePrototype.Models
{
    public abstract class ConnectionManager
    {
        //Stores connections for faster load later.
        private static IDictionary<int, SqlConnection> connections = new ConcurrentDictionary<int, SqlConnection>();

        private static String _default;

        /// <summary>
        /// Opens or returns a connection, given a Connection String.
        /// </summary>
        /// <param name="connectionString">A valid connection string.</param>
        /// <returns>DB Connection</returns>
        public static SqlConnection Open(string connectionString)
        {

            if (connections.ContainsKey(connectionString.GetHashCode()))
                return connections[connectionString.GetHashCode()];

            if (_default == null)
                _default = connectionString;


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

        public static SqlConnection OpenLast()
        {
            if (_default == null)
            {
                throw new IllegalStateException(
                    "Connection Manager cannnot retrieve unset connection! Call Open() First!");
            }

            return connections[_default.GetHashCode()];
        }


        /// <summary>
        /// Returns new Opened Connection
        /// </summary>
        public static SqlConnection OpenNew()
        {
            var c = new SqlConnection(_default);
            c.Open();
            return c ;
        }

        /// <summary>
        /// Returns new Opened Connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static SqlConnection OpenNew(string connectionString)
        {
            var c = new SqlConnection(connectionString);
            c.Open();
            return c;
        }
    }
}

