using System;

namespace i1Sharp
{
    public class I1ProException : Exception 
    {
        public I1Pro.Result Result { get; private set; }

        
        public I1ProException (I1Pro.Result result, string message)
        : base (message)
        {
            Result = result;
        }

        public override string Message 
        { 
            get
            {
                return "Erro " + (int) Result + " (" + Result + "): " + base.Message;
            }
        }
    }
}
