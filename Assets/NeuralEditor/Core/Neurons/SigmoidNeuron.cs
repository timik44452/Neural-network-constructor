namespace Core.Neurons
{
    public class SigmoidNeuron : Neuron
    {
        public SigmoidNeuron() : base()
        {

        }

        protected override void CounterInvokeInput(float value)
        {
            Value = (float)(1.0 / (1.0 + System.Math.Exp(-2 * value)));

            Propagate(Value);
        }

        protected override void CounterInvokeLern(float rate, float value)
        {
            float delta = value * Value * (1 - Value);

            foreach (var link in input_weights)
            {
                float weight_delta = delta * link.source.Value;

                link.weight += rate * weight_delta;

                link.source.Lern(rate, weight_delta);
            }
        }
    }
}