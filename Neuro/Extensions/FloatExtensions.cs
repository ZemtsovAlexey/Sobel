using System;

namespace Neuro.Extensions
{
    public static class FloatExtensions
    {
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble(); // range 0.0 to 1.0
        }

        public static float[,] Sum(this float[][,] inputs)
        {
            if (inputs == null || inputs.Length < 1)
                return null;
            
            var inputHeight = inputs[0].GetLength(0);
            var inputWidth = inputs[0].GetLength(1);
            var sum = new float[inputHeight,inputWidth];
            
            foreach (var input in inputs)
            {
                for (var y = 0; y < inputHeight; y++)
                {
                    for (var x = 0; x < inputWidth; x++)
                    {
                        sum[y, x] += input[y, x];
                    }
                }
            }

            return sum;
        }
    }
}
