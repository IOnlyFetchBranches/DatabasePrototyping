using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using dbutils;

using static dbutils.Logger;

namespace DatabasePrototype.Models
{
    public class InventoryDataRecord : IDataRecord
    {
        //TODO: Let's bring the TAG system back. Learned it from Android. Denote a TAG static final [readonly] variable to make logging easier!
        private static readonly string TAG = "InventoryDataRecord";

        //TODO: Fix ItemId to ItemID

        /// <summary>
        /// Dictionary whose keys are column names and values are row specific values
        /// </summary>
        private Dictionary<string, string> data = new Dictionary<string, string>();
        //Contains a customer's matched card info data, if any.
        private Dictionary<string, string> itemData = new Dictionary<string, string>();

        /// <summary>
        /// The global query.
        /// </summary>
        private string _query = "";
        /// <summary>
        /// This instance's Sql Connection.
        /// </summary>
        private SqlConnection _connection;




        public InventoryDataRecord(SqlDataReader rowData, SqlConnection connection)
        {
            if (!rowData.HasRows)
                throw new Exception("No items to add to record!");
            //Save connection for reusability
            _connection = connection;
            //The default position of the SqlDataReader is before the first record. So we call read()
            rowData.Read();
            //Now that the reader is over a row, read all the columns into the data map saving them by the name.
            Logger.LogG(TAG, "Reading Inventory record.");
            int x = 0;
            for (x = 0; x < rowData.FieldCount; x++)
            {
                var colName = rowData.GetName(x);
                var val = rowData.GetFieldValue<Object>(x);
                //Convert datetimes to strings.
                if (val is DateTime)
                    val = ((DateTime)val).ToString("yyyy-MM-dd");
                else
                {
                    val = val + "";
                }
                data.Add(colName, (String)val);
            }

            //Close the reader, to free the calling SqlCommand that gave us the reader.
            rowData.Close();
            x = 0;//reset x for reuse!

            //TODO: Implement the tables that have data we need to get

            string invId = data["InvID"];
            string itemId = data["ItemId"];

            //Query
            SqlCommand getItems = new SqlCommand("Select * From Items Where ItemID = '" + itemId + "'", _connection);

            //if you get an error here, check your database setup! Ensure you have the latest script pulled from the repo.
            rowData = getItems.ExecuteReader();
            rowData.Read(); //Again, we must push it forward one.
            Logger.LogG(TAG, "Reading Item info for " + itemId + " in inventory " + invId );
            for (x = 0; x < rowData.FieldCount; x++)
            {
                var colName = rowData.GetName(x);
                //This should not happen, but just incase of accidental duplicates, we skip them.
                if (data.ContainsKey(colName))
                    continue;

                var val = rowData.GetFieldValue<Object>(x);
                if (val is DateTime)
                    val = ((DateTime)val).ToString("yyyy-MM-dd");
                else
                {
                    val = val + ""; 
                    //Why the + "" you might ask?  Quicker way to ensure val is treated as a string. This however should be noticed as BADPRACTICE, however due to time we'll fix it later. (Generates extra SB)
                }


                itemData.Add(colName, (String)val);
            }
            //TODO: Also add the ability to get DID.
            //Close the reader, to free the calling SqlCommand that gave us the reader.
            rowData.Close();


            //We would continue this pattern for the other related tables.
            //For now I just want to finish with the update() implementation
            BuildQuery(); //Build the base query, which should update nothing.





        }


        /// <summary>
        /// Returns the record's currently generated query.
        /// </summary>
        /// <returns></returns>
        public string UpdateQuery()
        {
            return _query;
        }

        public void Update()
        {
            BuildQuery(); //Finalize changes
            SqlCommand updateRecord = new SqlCommand(_query, _connection);
            updateRecord.ExecuteNonQuery();

        }
        /// <summary>
        /// Sets a field to be updated.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void SetField(string field, String value)
        {
            if (!data.ContainsKey(field) & !itemData.ContainsKey(field))
                throw new InvalidOperationException("Can only set existing values!");
            if (data.ContainsKey(field))
            {
                data.Remove(field);
                data.Add(field, value);
            }
            else if (itemData.ContainsKey(field))
            {
                itemData.Remove(field);
                itemData.Add(field, value);
            }

        }

        public string Get(string key)
        {
            if (data.ContainsKey(key))
                return data[key];
            else if (itemData.ContainsKey(key))
                return itemData[key];
            //We would continue this pattern here.
            else
            {
                return null;
            }
        }

        public void BuildQuery()
        {

            //TODO:Always change these, to match the record's tables!
            var _tables = new[] { "InventoryInfo", "Items" };

            //So let's begin with the final part, building the base query. 
            //For generification reasons, the query will pull fields from the map, which can be updated from the GUI via SetField()
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("Update " + _tables[0] + " Set ");



            foreach (KeyValuePair<string, string> pair in data)
            {
                queryBuilder.Append((string)(pair.Key + " = '" + pair.Value + "' , ")); //Don't forget the ' 
            }

            //Add where
            queryBuilder.Append("Where InvID = '" + data["InvID"] + "'"); //Don't FoRgEt the ' !

            //Generate first query
            var _rawQuery = queryBuilder.ToString();

            //Don't forget to remove that straggling comma :)
            _rawQuery = _rawQuery.Remove(_rawQuery.LastIndexOf(",", StringComparison.Ordinal), 1);

            queryBuilder.Clear(); //clear the strBuilder

            //Build second query, to update contacts
            queryBuilder.Append("Update " + _tables[1] + " Set ");

            //parse the dictionary into sql.
            foreach (KeyValuePair<string, string> pair in itemData)
            {
                queryBuilder.Append((string)(pair.Key + " = '" + pair.Value + "' , ")); //Don't forget the ' 
            }

            //Add where
            queryBuilder.Append("Where ItemId = '" + data["ItemId"] + "'"); //Don't FoRgEt the ' !





            //Build final query by adding on the second half.
            _query = _rawQuery + Environment.NewLine + queryBuilder;


            //Don't forget to remove that straggling comma :) then we'll have our fully-parsed query.
            _query = _query.Remove(_query.LastIndexOf(",", StringComparison.Ordinal), 1);

            //Additional Sanitization here, we need to change Name => [Name] as the former is a keyword...
            _query = _query.Replace("Name", "[Name]");


            //DEBUG
            //MessageBox.Show("Built query:\n" + _query);
            Logger.LogG(TAG, "Built Query:\n" + _query);
        }
    }
}
