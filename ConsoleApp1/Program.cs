using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 100;
            int[] x = new int[n];
            Campy.Parallel.For(n, i => x[i] = i);
            var a = 1;
        }
    }
}
