using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatabasePrototype.Models
{
    /// <summary>
    /// Represents a single row of a query.
    /// This should be implemented for each major category.
    /// Unfortunately there is no easy one-size-fits-all solution without getting into much to complicated things
    /// Or using LINQ, but I've done my best to keep the 'recipies' closely related so it at least becomes simple grunt work.
    /// </summary>
    public interface IDataRecord
    {
        /// <summary>
        /// Used to pull the update query string from a DataRecord
        /// </summary>
        /// <returns>The string update query.</returns>
        string UpdateQuery();
        /// <summary>
        /// Internally parses and runs an update query on the backend!
        /// </summary>
        void Update();
        /// <summary>
        /// Sets a record to be updated, all types should be string castable.
        /// Types are handled during the query transaction!
        /// </summary>
        void SetField(string field, string value);

        string Get(string key);

        /// <summary>
        /// Builds the internal query for this Record instance, you generally don't need to call this manually.
        /// </summary>
        void BuildQuery();
    }
}
