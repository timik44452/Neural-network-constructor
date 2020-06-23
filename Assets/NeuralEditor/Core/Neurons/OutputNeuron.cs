using System.Linq;
using UnityEngine;

namespace Core.Neurons
{
    public class OutputNeuron : Neuron
    {
        public OutputNeuron() : base()
        {

        }

        protected override void CounterInvokeInput()
        {
            Value = ActivationFunction(input_synapse.Sum(x => x.weight * x.source.Value));

            Propagate();
        }

        protected override void CounterInvokeLern(float rate)
        {
            input_synapse.ForEach(x =>
            {
                // Not used target
                x.source.CalculateGradient(0);
                x.source.Lern(rate);
            });
        }

        public override void CalculateGradient(float target)
        {
            float error = target - Value;

            Gradient = error * Value * (1 - Value);
        }

        private float ActivationFunction(float value)
        {
            return value < -45.0F ? 0.0F : value > 45.0F ? 1.0F : 1.0F / (1.0F + Mathf.Exp(-value));
        }

        private float Derivative(float x)
        {
            return x * (1 - x);
        }
    }
}