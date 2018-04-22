using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using dbutils;

namespace DatabasePrototype.Models
{
    public class EmployeeDataRecord : IDataRecord
    {
        //TODO: Let's bring the TAG system back. Learned it from Android. Denote a TAG static final [readonly] variable to make logging easier!
        private static readonly string TAG = "EmployeeDataRecord";

        /// <summary>
        /// Dictionary whose keys are column names and values are row specific values
        /// </summary>
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private Dictionary<string, string> contactData = new Dictionary<string, string>();

        /// <summary>
        /// The global query.
        /// </summary>
        private string _query = "";
        /// <summary>
        /// This instance's Sql Connection.
        /// </summary>
        private SqlConnection _connection;

        


        public EmployeeDataRecord(SqlDataReader rowData, SqlConnection connection)
        {
            if (!rowData.HasRows)
                throw new Exception("No items to add to record!");
            //Save connection for reusability
            _connection = connection;
            //The default position of the SqlDataReader is before the first record. So we call read()
            rowData.Read();
            //Now that the reader is over a row, read all the columns into the data map saving them by the name.
            Logger.LogG(TAG,"Reading Employee record.");
            int x = 0;
            for(x = 0;x<rowData.FieldCount;x++)
            {
                var colName = rowData.GetName(x);
                var val = rowData.GetFieldValue<Object>(x);
                if (val is DateTime)
                    val = ((DateTime) val).ToString("yyyy-MM-dd");
                else
                {
                    val = val + "";
                }
                data.Add(colName,(String) val);
            }
            //Close the reader, to free the calling SqlCommand that gave us the reader.
            rowData.Close();
            x = 0;//reset x for reuse!
            
            //By here it should have pulled the EID, we know Employees also have contact info so let's get that as well
            //TODO: Implement the other two tables EmployeeWages, EmployeeDirectDepositInfo; Later

            string _eid = data["EID"];
            //Query
            SqlCommand getContacts = new SqlCommand("Select * From EmployeeContacts Where EID = '" +_eid +"'", _connection);
            //We should be able to assert this exists otherwise there would be no employee record at all.
            //Each employee must have a contacts, so to find an employee asserts there is a contact. They are added together
            //if you get an error here, check your database setup! Ensure you have the latest script pulled from the repo.
            rowData = getContacts.ExecuteReader();
            rowData.Read(); //Again, we must push it forward one.
            Logger.LogG(TAG, "Reading Employee Contact info for " + _eid);
            for (x = 0; x < rowData.FieldCount; x++)
            {
                var colName = rowData.GetName(x);
                if(data.ContainsKey(colName))
                    continue;
                var val = rowData.GetFieldValue<Object>(x);
                if (val is DateTime)
                    val = ((DateTime)val).ToString("yyyy-MM-dd");
                else
                {
                    val = val + "";
                }

                
                contactData.Add(colName, (String)val);
            }
            //Close the reader, to free the calling SqlCommand that gave us the reader.
            rowData.Close();

        
            //We would continue this pattern for the other related tables.
            //For now I just want to finish with the update() implementation
            BuildQuery(); //Build the base query, which should update nothing.





        }


    
        public string UpdateQuery()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            BuildQuery(); //Finalize changes
            SqlCommand updateRecord = new SqlCommand(_query,_connection);
            updateRecord.ExecuteNonQuery();

        }
        /// <summary>
        /// Sets a field to be updated.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void SetField(string field, String value)
        {
            if (!data.ContainsKey(field) & !contactData.ContainsKey(field))
                throw new InvalidOperationException("Can only set existing values!");
            if (data.ContainsKey(field))
            {
                data.Remove(field);
                data.Add(field, value);
            }
            else if (contactData.ContainsKey(field))
            {
                contactData.Remove(field);
                contactData.Add(field, value);
            }
        
        }

        public string Get(string key)
        {
            if (data.ContainsKey(key))
                return data[key];
            else if (contactData.ContainsKey(key))
                return contactData[key];
            else
            {
                return null;
            }
        }

        public void BuildQuery()
        {

            var _tables = new []{"Employees","EmployeeContacts"};

            //So let's begin with the final part, building the base query. 
            //For generification reasons, the query will pull fields from the map, which can be updated from the GUI via SetField()
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("Update " + _tables[0] + " Set ");



            foreach (KeyValuePair<string, string> pair in data)
            {
                queryBuilder.Append((string)(pair.Key + " = '" + pair.Value + "' , ")); //Don't forget the ' 
            }

            //Add where
            queryBuilder.Append("Where EID = '" + data["EID"]+"'"); //Don't FoRgEt the ' !

            //Generate first query
            var _rawQuery = queryBuilder.ToString();

            //Don't forget to remove that straggling comma :)
            _rawQuery = _rawQuery.Remove(_rawQuery.LastIndexOf(",", StringComparison.Ordinal), 1);

            queryBuilder.Clear(); //clear the strBuilder

            //Build second query, to update contacts
            queryBuilder.Append("Update " + _tables[1] + " Set ");

            foreach (KeyValuePair<string, string> pair in contactData)
            {
                queryBuilder.Append((string)(pair.Key + " = '" + pair.Value + "' , ")); //Don't forget the ' 
            }

            //Add where
            queryBuilder.Append("Where EID = '" + data["EID"] + "'"); //Don't FoRgEt the ' !




            
            //Build final query by adding on the second half.
            _query = _rawQuery +Environment.NewLine + queryBuilder;

            //Don't forget to remove that straggling comma :)
            _query = _query.Remove(_query.LastIndexOf(",", StringComparison.Ordinal), 1);


            //DEBUG
            //MessageBox.Show("Built query:\n" + _query);
            Logger.LogG(TAG,"Built Query:\n" + _query);
        }
    }
}
