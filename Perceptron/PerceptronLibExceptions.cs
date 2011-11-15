using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerceptronLib
{
    public class ExampleException : Exception
    {
        public ExampleException() : base() { }
        public ExampleException(string message) : base(message) { }
    }

    public class ExampleListException : ExampleException
    {
        private List<LearningExample> list;
        public ExampleListException() : base() {}
        public ExampleListException(string message, List<LearningExample> list = null) : base(message) 
        {
            this.list = list;
        }

        public List<LearningExample> Examples
        {
            get
            {
                return list;
            }
        }
    }
}
