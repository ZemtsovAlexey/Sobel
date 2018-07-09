using System;

namespace Sobel.UI
{
    public class LogEventArgs : EventArgs
    {
        public long I { get; set; }
        public int Success { get; set; }
        public double Time { get; set; }
        
        public LogEventArgs(long i, int success, double time)
        {
            I = i;
            Success = success;
            Time = time;
        }
    }
}
