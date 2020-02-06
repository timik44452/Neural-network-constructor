namespace Core.Neurons
{
    public class BiasNeuron : Neuron
    {
        public BiasNeuron() : base()
        {
            Value = 0.5F;
        }

        protected override void CounterInvokeInput(float value)
        {
            
        }

        protected override void CounterInvokeLern(float rate, float delta)
        {
            
        }
    }
}
