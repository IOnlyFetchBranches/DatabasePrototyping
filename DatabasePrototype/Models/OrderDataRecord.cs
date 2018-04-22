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
    public class OrderDataRecord : IDataRecord
    {
        //TODO: Let's bring the TAG system back. Learned it from Android. Denote a TAG static final [readonly] variable to make logging easier!
        private static readonly string TAG = "OrderDataRecord";

        /// <summary>
        /// Dictionary whose keys are column names and values are row specific values
        /// </summary>
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private Dictionary<string, string> orderInfoData = new Dictionary<string, string>();




        //Contains Customer data
        private Dictionary<string, string> custData = new Dictionary<string, string>();
        

        /// <summary>
        /// The global query.
        /// </summary>
        private string _query = "";
        /// <summary>
        /// This instance's Sql Connection.
        /// </summary>
        private SqlConnection _connection;






        public OrderDataRecord(SqlDataReader rowData, SqlConnection connection)
        {
            if (!rowData.HasRows)
                throw new Exception("No items to add to record!");
            //Save connection for reusability
            _connection = connection;



            //The default position of the SqlDataReader is before the first record. So we call read()
            rowData.Read();
            //Now that the reader is over a row, read all the columns into the data map saving them by the name.
            Logger.LogG(TAG, "Reading Order record.");
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

            string custId = data["CID"];
            string ordID = data["OID"];
            
        
            //Query
            SqlCommand getCust = new SqlCommand("Select * From Customers Where CID = '" + custId + "'", _connection);

            //if you get an error here, check your database setup! Ensure you have the latest script pulled from the repo.
            rowData = getCust.ExecuteReader();
            rowData.Read(); //Again, we must push it forward one.
            Logger.LogG(TAG, "Reading Customer info for " + custId);
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
                }


                custData.Add(colName, (String)val);
            }
            //Close the reader, to free the calling SqlCommand that gave us the reader.
            rowData.Close();


            //Query
            SqlCommand getOrdInfo = new SqlCommand("Select * From OrderInformation Where OID = '" + ordID + "'", _connection);

            //if you get an error here, check your database setup! Ensure you have the latest script pulled from the repo.
            rowData = getOrdInfo.ExecuteReader();
            rowData.Read(); //Again, we must push it forward one.
            Logger.LogG(TAG, "Reading Order info for " + ordID);
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
                }


                orderInfoData.Add(colName, (String)val);
            }
            //Close the reader, to free the calling SqlCommand that gave us the reader.
            rowData.Close();






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
            if (!data.ContainsKey(field) & !custData.ContainsKey(field))
                throw new InvalidOperationException("Can only set existing values!");
            if (data.ContainsKey(field))
            {
                data.Remove(field);
                data.Add(field, value);
            }
            else if (custData.ContainsKey(field))
            {
                custData.Remove(field);
                custData.Add(field, value);
            }

        }

        public string Get(string key)
        {
            if (data.ContainsKey(key))
                return data[key];
            else if (custData.ContainsKey(key))
                return custData[key];
            //We would continue this pattern here.
            else
            {
                return null;
            }
        }

        public void BuildQuery()
        {

            //TODO:Always change these, to match the record's tables!
            var _tables = new[] { "Orders","Customers", "OrderInformation" };

            //So let's begin with the final part, building the base query. 
            //For generification reasons, the query will pull fields from the map, which can be updated from the GUI via SetField()
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("Update " + _tables[0] + " Set ");



            foreach (KeyValuePair<string, string> pair in data)
            {
                queryBuilder.Append((string)(pair.Key + " = '" + pair.Value + "' , ")); //Don't forget the ' 
            }

            //Add where
            queryBuilder.Append("Where OID = '" + data["OID"] + "'"); //Don't FoRgEt the ' !

            //Generate first query
            var _rawQuery = queryBuilder.ToString();

            //Don't forget to remove that straggling comma :)
            _rawQuery = _rawQuery.Remove(_rawQuery.LastIndexOf(",", StringComparison.Ordinal), 1);

            queryBuilder.Clear(); //clear the strBuilder

            //Build second query, to update Customer
            queryBuilder.Append("Update " + _tables[1] + " Set ");

            //parse the dictionary into sql.
            foreach (KeyValuePair<string, string> pair in custData)
            {
                queryBuilder.Append((string)(pair.Key + " = '" + pair.Value + "' , ")); //Don't forget the ' 
            }

            //Add where
            queryBuilder.Append("Where CID = '" + data["CID"] + "'"); //Don't FoRgEt the ' !

            _rawQuery = _rawQuery + Environment.NewLine + queryBuilder;
            _rawQuery = _rawQuery.Remove(_rawQuery.LastIndexOf(",", StringComparison.Ordinal), 1);
            queryBuilder.Clear();

            //Build third query, to update orderInformation
            queryBuilder.Append("Update " + _tables[2] + " Set ");

            //parse the dictionary into sql.
            foreach (KeyValuePair<string, string> pair in orderInfoData)
            {
                queryBuilder.Append((string)(pair.Key + " = '" + pair.Value + "' , ")); //Don't forget the ' 
            }

            //Add where
            queryBuilder.Append("Where OID = '" + data["OID"] + "' And ItemID = '" + orderInfoData["ItemID"] +"' "  ); //Don't FoRgEt the ' !

            _rawQuery = _rawQuery + Environment.NewLine + queryBuilder;
            _rawQuery = _rawQuery.Remove(_rawQuery.LastIndexOf(",", StringComparison.Ordinal), 1);
            queryBuilder.Clear();




            _query = _rawQuery;
            
            //DEBUG
            MessageBox.Show("Built query:\n" + _query);
            Logger.LogG(TAG, "Built Query:\n" + _query);
        }
    }
}
