using System;

namespace Neuro.Extensions
{
    public static class FloatExtensions
    {
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble(); // range 0.0 to 1.0
        }
    }
}
