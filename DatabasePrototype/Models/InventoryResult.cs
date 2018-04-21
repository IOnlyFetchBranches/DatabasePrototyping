using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable ConvertToAutoProperty

namespace DatabasePrototype.Models
{
    public class InventoryResult : IResult
    {
        //DONTCOPY THIS ONE, THE _idm and _pm are switched!
        //data holders;
        private string _idm, _pm, _sm;

        //Only worry about these, as per the interface.
        public string IdentifyingMember => _idm;

        public string PrimaryMember => _pm;
        public string SecondaryMember => _sm;
        public string Table => "InventoryInfo";


        /// <summary>
        /// Creates a new Customer Result.
        /// Accepts Objects, although it is expecting STRINGS ONLY.
        /// </summary>
        /// <param name="memberStrings">Must be size 3, Format for now! {eid info,FirstName, LastName}</param>
        public InventoryResult(params object[] memberStrings)
        {
            if (memberStrings.Length != 3)
                throw new ArgumentException("Input must contain three fields. See docs.");
            //Set fields, swap for itemid to be ontop.
            _idm = memberStrings[1] +"";
            _pm = memberStrings[0] + "";
            _sm = memberStrings[2] + "";

         
        }
        /// <summary>
        /// Gets the Id Column Name.
        /// </summary>
        /// <returns>CID</returns>
        public string IdHeader() => "ItemId";
        /// <summary>
        /// Returns first name.
        /// </summary>
        /// <returns></returns>
        public string PrimaryHeader() => "InvID";
        /// <summary>
        /// Returns last name.
        /// </summary>
        /// <returns></returns>
        public string SecondaryHeader() => "Quantity";
    }
}
