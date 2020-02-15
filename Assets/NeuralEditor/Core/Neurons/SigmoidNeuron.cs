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
            float weight_delta = rate * delta * Value * (1 - Value);

            foreach (var link in input_weights)
            {
                link.weight += link.source.Value * link.weight * weight_delta;

                link.source.Lern(rate, link.weight * delta);
            }
        }
    }
}
