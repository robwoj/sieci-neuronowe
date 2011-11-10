using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerceptronLib
{
    /// <summary>
    /// Wyjątek odpowiadający przekazaniu obiektów o złych wymiarach
    /// </summary>
    public class SizeException : Exception
    {
        public SizeException(string message = "") : base(message) { }

    }
}
