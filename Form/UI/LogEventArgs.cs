using System;

namespace Sobel.UI
{
    public class LogEventArgs : EventArgs
    {
        public long I { get; set; }
        public int Success { get; set; }
        public double Time { get; set; }
        public double Error { get; set; }
        
        public LogEventArgs(long i, int success, double time, double error)
        {
            I = i;
            Success = success;
            Time = time;
            Error = error;
        }
    }
}
