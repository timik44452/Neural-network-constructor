using System.Collections.Generic;


namespace Core.Neurons
{
    [System.Serializable]
    public abstract class Neuron
    {
        public int ID;

        public int InputCount { get => input_weights.Count; }
        public int OutputCount { get => out_weights.Count; }

        public float Value;

        public List<NeuronLink> out_weights;
        public List<NeuronLink> input_weights;

        private int counter = 0;
        private float summ = 0;


        public Neuron()
        {
            out_weights = new List<NeuronLink>();
            input_weights = new List<NeuronLink>();
        }

        protected abstract void CounterInvokeInput(float value);
        protected abstract void CounterInvokeLern(float rate, float value);
        protected void Propagate(float value)
        {
            out_weights.ForEach(link => link.destination.Input(value * link.weight));
        }

        public void Input(float value)
        {
            counter++;
            summ += value;

            if (counter >= InputCount)
            {
                CounterInvokeInput(summ);
                Reset();
            }
        }

        public void Lern(float rate, float value)
        {
            counter++;
            summ += value;

            if (counter >= OutputCount)
            {
                CounterInvokeLern(rate, summ);
                Reset();
            }
        }

        public void Reset()
        {
            counter = 0;
            summ = 0;
        }
    }
}