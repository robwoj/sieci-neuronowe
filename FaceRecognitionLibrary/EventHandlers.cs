using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceRecognitionLibrary
{
    public class FaceRecognitionEngineEventArgs : EventArgs
    {
        public FaceRecognitionEngineEventArgs() : base() { }
    }

    public delegate void FaceRecogintionEventHandler(object sender, FaceRecognitionEngineEventArgs e);

    public class DataBaseEventArgs : FaceRecognitionEngineEventArgs
    {
        private string path;

        public string Path
        {
            get
            {
                return path;
            }
        }
        public DataBaseEventArgs(string path) : base() { this.path = path;  }
    }

    public delegate void DataBaseEventHandler(object sender, DataBaseEventArgs e);


    public class FaceRecognitionSuccessEventArgs : FaceRecognitionEngineEventArgs
    {
        private IUserInfo userInfo;

        public IUserInfo UserInfo
        {
            get
            {
                return userInfo;
            }
        }

        public FaceRecognitionSuccessEventArgs(IUserInfo userInfo) : base()
        {
            this.userInfo = userInfo;
        }
    }

    public delegate void FaceRecognitionSuccessEventHandler(object sender, 
        FaceRecognitionSuccessEventArgs e);


    public class FaceRecognitionFailEventArgs : FaceRecognitionEngineEventArgs
    {
        public FaceRecognitionFailEventArgs() : base() { }
    }

    public delegate void FaceRecognitionFailEventHandler(object sender, FaceRecognitionFailEventArgs e);
}
