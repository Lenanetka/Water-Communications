using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
