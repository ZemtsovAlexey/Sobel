using System;

namespace Neuro.ActivationFunctions
{
    public class LeCunTanh : IActivationFunction
    {
        public float Alpha { get; set; }
        public float MinRange { get; set; } = -1;
        public float MaxRange { get; set; } = 1;
        
        public float Activation(float x)
        {
            const float divX = 2f / 3;
            const float scale = 1.7159f;
            float e2x = (float)Math.Exp(2 * divX * x);
            
            return scale * (e2x - 1) / (e2x + 1);
        }

        public float Derivative(float x)
        {
            const float numerator = 4.57573f;
            float
                exp = 2 * x / 3,
                ePlus = (float)Math.Exp(exp),
                eMinus = (float)Math.Exp(-exp),
                sum = ePlus + eMinus,
                square = sum * sum;
            
            return numerator / square;
        }
    }
}