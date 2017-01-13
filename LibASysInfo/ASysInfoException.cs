using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibASysInfo
{
    public class ASysInfoException : Exception
    {
        public ASysInfoException()
        {
        }

        public ASysInfoException(string message) : base(message)
        {
        }

        public ASysInfoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
