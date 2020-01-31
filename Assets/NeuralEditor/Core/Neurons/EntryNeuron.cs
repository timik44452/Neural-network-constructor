namespace Core.Neurons
{
    public class EntryNeuron : Neuron
    {
        public EntryNeuron() : base()
        {

        }

        protected override void CounterInvokeInput(float value)
        {
            Value = value;

            Propagate(value);
        }

        protected override void CounterInvokeLern(float rate, float value)
        {
            
        }
    }
}