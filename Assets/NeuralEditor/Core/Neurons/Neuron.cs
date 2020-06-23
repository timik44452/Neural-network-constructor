using System.Collections.Generic;


namespace Core.Neurons
{
    [System.Serializable]
    public abstract class Neuron
    {
        public int ID;

        public int InputCount { get => input_synapse.Count; }
        public int OutputCount { get => out_synapse.Count; }

        public float Value;
        public float Gradient;

        public List<NeuronLink> out_synapse;
        public List<NeuronLink> input_synapse;

        protected int counter = 0;

        public Neuron()
        {
            out_synapse = new List<NeuronLink>();
            input_synapse = new List<NeuronLink>();
        }

        public abstract void CalculateGradient(float target);
        public void UpdateWeights(float learnRate)
        {
            float prevDelta;

            foreach (var synapse in input_synapse)
            {
                prevDelta = synapse.WeightDelta;
                synapse.WeightDelta = learnRate * Gradient * synapse.source.Value;
                synapse.weight += synapse.WeightDelta + prevDelta;
            }
        }

        protected abstract void CounterInvokeInput();
        protected abstract void CounterInvokeLern(float rate);
        protected void Propagate()
        {
            foreach (var link in out_synapse)
            {
                link.destination.Input();
            }
        }

        public void Input()
        {
            counter++;

            if (counter >= InputCount)
            {
                CounterInvokeInput();
                Reset();
            }
        }

        public void Lern(float rate)
        {
            counter++;

            if (counter >= OutputCount)
            {
                CounterInvokeLern(rate);
                Reset();
            }
        }

        public void Reset()
        {
            counter = 0;
        }
    }
}