using GASS.CUDA;

namespace Neuro.GPU
{
    public class Class2
    {
        public void Test()
        {
            CUDA cuda = new CUDA( true );
            var devices = cuda.Devices;

            var context = cuda.CreateContext(0);
            cuda.SetCurrentContext(context);

            var dev_Value = cuda.Allocate<double>(new double[] {3});
            var dev_Value2 = cuda.Allocate<double>(new double[]{2});

            var r = dev_Value + dev_Value2;
            double[] data = new double[1];
            cuda.CopyDeviceToHost<double>(dev_Value, data);
            cuda.CopyHostToDevice(dev_Value2, data);
            
            var a = 1;
        }
    }
}