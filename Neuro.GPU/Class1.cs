using System;

namespace Neuro.GPU
{
    public class Class1
    {
        public void Test()
        {
            int n = 100000;
            int[] x = new int[n];
            Campy.Parallel.For(n, i => x[i] = i);
            var a = 1;
        }
    }
}
