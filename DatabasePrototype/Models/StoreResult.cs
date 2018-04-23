using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable ConvertToAutoProperty

namespace DatabasePrototype.Models
{
    public class StoreResult : IResult
    {
        //data holders;
        private string _idm, _pm, _sm;

        //Only worry about these, as per the interface.
        public string IdentifyingMember => _idm;

        public string PrimaryMember => _pm;
        public string SecondaryMember => _sm;
        public string Table => "StoreInfo";


        /// <summary>
        /// Creates a new Employee Result.
        /// Accepts Objects, although it is expecting STRINGS ONLY.
        /// </summary>
        /// <param name="memberStrings">Must be size 3, Format for now! {eid info,FirstName, LastName}</param>
        public StoreResult(params object[] memberStrings)
        {
            if (memberStrings.Length != 3)
                throw new ArgumentException("Input must contain three fields. See docs.");
            //Set fields
            _idm = memberStrings[0] +"";
            _pm = memberStrings[1] + "";
            _sm = memberStrings[2] +"";

        }
        /// <summary>
        /// Gets the Id Column Name.
        /// </summary>
        /// <returns>EID</returns>
        public string IdHeader() => "STRID";
        /// <summary>
        /// Returns first name.
        /// </summary>
        /// <returns></returns>
        public string PrimaryHeader() => "SState";
        /// <summary>
        /// Returns last name.
        /// </summary>
        /// <returns></returns>
        public string SecondaryHeader() => "SZip";
    }
}
