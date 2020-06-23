using System.Linq;

namespace Core.Neurons
{
    public class EntryNeuron : Neuron
    {
        public EntryNeuron() : base()
        {

        }

        public override void CalculateGradient(float target)
        {
            Gradient = out_synapse.Sum(a => a.destination.Gradient * a.weight) * Derivative(Value);
        }

        protected override void CounterInvokeInput()
        {
            Propagate();
        }

        protected override void CounterInvokeLern(float rate)
        {
            
        }

        private float Derivative(float x)
        {
            return x * (1 - x);
        }
    }
}