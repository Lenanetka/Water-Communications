using System;

namespace WaterCommunications
{
    public class NonCriticalErrorEventArgs : EventArgs
    {
        private readonly String message;
        public NonCriticalErrorEventArgs(String message)
        {
            this.message = message;
        }
        public String Message { get { return message; } }
    }
}
