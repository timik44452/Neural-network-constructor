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
            float delta = value - Value;
            float weight_delta = rate * delta * Value * (1 - Value);

            foreach (var link in input_weights)
            {
                link.weight += link.source.Value * link.weight * weight_delta;

                link.source.Lern(rate, link.weight * delta);
            }
        }
    }
}