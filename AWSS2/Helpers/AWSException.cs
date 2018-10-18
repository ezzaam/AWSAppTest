using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSS2.Helpers
{
    public class AWSException : Exception
    {
        public AWSException(string message, Exception ex)  :
            base(String.Format(message))
        {

        }

        public AWSException(string message)
        {

        }
    }
}
