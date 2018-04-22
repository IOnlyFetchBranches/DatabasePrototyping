using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable ConvertToAutoProperty

namespace DatabasePrototype.Models
{
    public class OrderResult : IResult
    {
        //data holders;
        private string _idm, _pm, _sm;

        //Only worry about these, as per the interface.
        public string IdentifyingMember => _idm;

        public string PrimaryMember => _pm;
        public string SecondaryMember => _sm;
        public string Table => "Orders";


        /// <summary>
        /// Creates a new Order Result.
        /// Accepts Objects, although it is expecting STRINGS ONLY.
        /// It should be in this order (Id,PrimaryMember,SecondaryMember}
        /// </summary>
        /// <param name="memberStrings">Must be size 3, Format for now! {eid info,FirstName, LastName}</param>
        public OrderResult(params object[] memberStrings)
        {
            if (memberStrings.Length != 3)
                throw new ArgumentException("Input must contain three fields. See docs.");
            //Set fields
            _idm =memberStrings[0] + "";
            _pm = memberStrings[1] + "";
            _sm = memberStrings[2] + "";

        }
        /// <summary>
        /// Gets the Id Column Name.
        /// </summary>
        /// <returns>OID</returns>
        public string IdHeader() => "OID";
        /// <summary>
        /// Returns customer id attached to the order.
        /// </summary>
        /// <returns>CID</returns>
        public string PrimaryHeader() => "CID";
        /// <summary>
        /// Returns total.
        /// </summary>
        /// <returns></returns>
        public string SecondaryHeader() => "Total";
    }
}
