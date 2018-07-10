
namespace Neuro.GPU
{
    public class Class2
    {
        public void Test()
        {
            int n = 4;
            int[] x = new int[n];
            Campy.Parallel.For(n, i => x[i] = i);
            Campy.Parallel.For(n, i =>
            {
                
            });
        }
    }
}