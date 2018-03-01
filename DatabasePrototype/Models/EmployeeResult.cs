using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable ConvertToAutoProperty

namespace DatabasePrototype.Models
{
    public class EmployeeResult : IResult
    {
        //data holders;
        private string _idm, _pm, _sm;

        //Only worry about these, as per the interface.
        public string IdentifyingMember => _idm;

        public string PrimaryMember => _pm;
        public string SecondaryMember => _sm;


        /// <summary>
        /// Creates a new Employee Result;
        /// </summary>
        /// <param name="memberStrings">Must be size 3, Format for now! {eid info,FirstName, LastName} F</param>
        public EmployeeResult(params string[] memberStrings)
        {
            if (memberStrings.Length != 3)
                throw new ArgumentException("Input must contain three fields. See docs.");
            //Set fields
            _idm = memberStrings[0];
            _pm = memberStrings[1];
            _sm = memberStrings[2];

        }

        public string IdHeader() => "EID";

        public string PrimaryHeader() => "FirstName";
        public string SecondaryHeader() => "LastName";
    }
}
