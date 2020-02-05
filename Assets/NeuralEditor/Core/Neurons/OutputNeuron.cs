namespace Core.Neurons
{
    public class OutputNeuron : Neuron
    {
        public OutputNeuron() : base()
        {

        }

        protected override void CounterInvokeInput(float value)
        {
            Value = (float)(1.0 / (1.0 + System.Math.Exp(-value)));
        }

        protected override void CounterInvokeLern(float rate, float value)
        {
            float delta = Value * (1 - Value) * (value - Value);

            foreach (var link in input_weights)
            {
                float weight_delta = delta * link.source.Value;

                link.weight += rate * weight_delta;

                link.source.Lern(rate, weight_delta);
            }
        }
    }
}