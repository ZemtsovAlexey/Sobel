using System;

namespace Sobel.UI
{
    public class LogEventArgs : EventArgs
    {
        public long I { get; set; }
        public int Success { get; set; }
        
        public LogEventArgs(long i, int success)
        {
            I = i;
            Success = success;            
        }
    }
}
