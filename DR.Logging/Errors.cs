using System;
using System.Text;
using System.Collections.Generic;

namespace DR.Logging
{
    public class Errors
    {
        /// <summary>
        /// Throw exception if provided color isn't HEX or RGB.
        /// </summary>
        [Serializable]
        public class InvalidColorException : Exception
        {
            public InvalidColorException() { }

            public InvalidColorException(string message) : base(message) { }
        }

        /// <summary>
        /// Throw exception if path/directory doesn't exist
        /// </summary>
        [Serializable]
        public class InvalidPathException : Exception
        {
            public InvalidPathException() { }

            public InvalidPathException(string message) : base(message) { }
        }
    }
}
