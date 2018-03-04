using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable ConvertToAutoProperty

namespace DatabasePrototype.Exceptions
{
    public sealed class IllegalStateException : SystemException
    {

        private readonly string _message;
        public override string Message => _message;

        //Throw when program has reached a place it should not have
        public IllegalStateException(string message)
        {
            _message = message; //Set message.
        }

        //Throw when program has reached a place it should not have
        public IllegalStateException()
        {
            _message = "Program reached unexpected state."; //Set message.
        }
    }
}
