using Neuro.ActivationFunctions;
using Neuro.Layers;
using Neuro.Learning;
using Neuro.Networks;
using Network = Neuro.Networks.Network;

namespace Sobel.Neronet
{
    public class BackPropoginationNew
    {
        public ActivationNetwork Network;

        public int Iterations = 2000;
        public double LearningRate = 0.05;
        public double ResultError = 0;
        public int Iteration { get; set; }

        private bool _needToStop = false;

        public BackPropoginationNew()
        {
            var activation = new BipolarSigmoidFunction();
            Network = new ActivationNetwork(activation, 400, 15, 15, 15, 15, 1);

            //Network = new ActivationNetwork(
            //    new ActivationLayer(50, 400, activation),
            //    new ActivationLayer(25, 50, activation),
            //    new ActivationLayer(1, 25, activation)
            //    );

            Network.Randomize();

//            Network = new Network(
//                new ConvolutionalLayer(new ReluFunction(), 2, 20, 20, 3),
//                new ActivationLayer(15, 70, activation),
//                new ActivationLayer(6, 15, activation),
//                new ActivationLayer(1, 6, activation)
//                );
            
            Network.Randomize();
        }

        public double[] Compute(double[] inputs)
        {
            return Network.Compute(inputs);
        }

        public void SearchSolution(double[] inputs, double[] outputs)
        {
            var teacher = new BackPropagationLearning(Network)
            {
                LearningRate = LearningRate
            };

            teacher.Run(inputs, outputs);
        }

        public void SearchSolutionStop()
        {
            _needToStop = true;
        }
    }
}
