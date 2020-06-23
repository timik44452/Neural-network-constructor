using System.Linq;
using UnityEngine;

namespace Core.Neurons
{
    public class SigmoidNeuron : Neuron
    {
        public SigmoidNeuron() : base()
        {

        }

        public override void CalculateGradient(float target)
        {
            Gradient = out_synapse.Sum(a => a.destination.Gradient * a.weight) * Derivative(Value);
        }

        protected override void CounterInvokeInput()
        {
            Value = ActivationFunction(input_synapse.Sum(x => x.weight * x.source.Value));

            Propagate();
        }

        protected override void CounterInvokeLern(float rate)
        {
            
        }


        private float ActivationFunction(float value)
        {
            return value < -45.0F ? 0.0F : value > 45.0F ? 1.0F : 1.0F / (1.0F + Mathf.Exp((float)-value));
        }

        private float Derivative(float x)
        {
            return x * (1 - x);
        }
    }
}