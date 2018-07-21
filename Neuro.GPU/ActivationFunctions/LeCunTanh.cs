using System;

namespace Neuro.ActivationFunctions
{
    public class LeCunTanh : IActivationFunction
    {
        public double Alpha { get; set; }
        public double MinRange { get; set; } = -1;
        public double MaxRange { get; set; } = 1;
        
        public double Activation(double x)
        {
            const double divX = 2f / 3;
            const double scale = 1.7159f;
            double e2x = Math.Exp(2 * divX * x);
            
            return scale * (e2x - 1) / (e2x + 1);
        }

        public double Derivative(double x)
        {
            const double numerator = 4.57573f;
            double
                exp = 2 * x / 3,
                ePlus = Math.Exp(exp),
                eMinus = Math.Exp(-exp),
                sum = ePlus + eMinus,
                square = sum * sum;
            
            return numerator / square;
        }
    }
}