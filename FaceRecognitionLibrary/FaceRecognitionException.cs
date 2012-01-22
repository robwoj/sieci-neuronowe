using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceRecognitionLibrary
{
    public class FaceRecognitionEngineException : Exception
    {
        public FaceRecognitionEngineException() : base() { }
        public FaceRecognitionEngineException(string message) : base(message) { }
        public FaceRecognitionEngineException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class DimensionException : FaceRecognitionEngineException
    {
        public DimensionException() : base() { }
        public DimensionException(string message) : base(message) { }
    }

    public class FileException : FaceRecognitionEngineException
    {
        public FileException() : base() { }
        public FileException(string message) : base() { }
        public FileException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

}
