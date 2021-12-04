using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AetherSenseRedux.Pattern
{
    public class PatternException : Exception
    {
        public PatternException() { }
        public PatternException(string message) : base(message) { }
        public PatternException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class PatternExpiredException : PatternException
    {
        public PatternExpiredException() { }
        public PatternExpiredException(string message) : base(message) { }
        public PatternExpiredException(string message, Exception innerException) : base(message, innerException) { }

    }
}
