using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MLPNetworkLib
{
    public class NetworkException : Exception
    {
        public NetworkException() : base() { }
        public NetworkException(string message)
            : base(message)
        {

        }
    }

    public class NetworkDimensionException : NetworkException
    {
        private List<int> dimensions;
        private int wrongIndex;
        private int badDimension;
        
        public NetworkDimensionException() : base() { }
        public NetworkDimensionException(string message) : base(message) { }
        public NetworkDimensionException(string message, List<int> dims, int wrongDimensionIndex = -1)
            : base(message)
        {
            dimensions = dims;
            wrongIndex = wrongDimensionIndex;
            badDimension = -1;
        }

        public NetworkDimensionException(string message, int badDim)
            : base(message)
        {
            badDimension = badDim;
            dimensions = null;
            wrongIndex = -1;
        }

        public int WrongIndex 
        {
            get
            {
                return wrongIndex;
            }
        }

        public List<int> Dimensions
        {
            get
            {
                return dimensions;
            }
        }

        public int BadDimension
        {
            get
            {
                return badDimension;
            }
        }
    }
}
