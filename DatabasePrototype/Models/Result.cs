using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasePrototype.Models
{
    public interface IResult
    {
        //Represents a result from the database, binding depends on this
        /// <summary>
        /// Returns the implentation specific root table.
        /// </summary>
        string Table { get; }

        string IdentifyingMember { get; }
        //Top Two most important pieces of info.
        string PrimaryMember { get; }
        string SecondaryMember { get; }

        //Get headers [For column naming]
        string IdHeader();
        string PrimaryHeader();
        string SecondaryHeader();


    }
}
