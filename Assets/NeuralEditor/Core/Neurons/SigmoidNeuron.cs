namespace Core.Neurons
{
    public class SigmoidNeuron : Neuron
    {
        public SigmoidNeuron() : base()
        {

        }

        protected override void CounterInvokeInput(float value)
        {
            Value = (float)(1.0 / (1.0 + System.Math.Exp(-value)));

            Propagate(Value);
        }

        protected override void CounterInvokeLern(float rate, float delta)
        {
            float local_delta = delta * Value * (1 - Value);

            foreach (var link in input_weights)
            {
                link.weight += rate * link.weight * local_delta;

                link.source.Lern(rate, local_delta);
            }

        }
    }
}