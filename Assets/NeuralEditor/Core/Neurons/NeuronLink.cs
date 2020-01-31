namespace Core.Neurons
{
    public class NeuronLink
    {
        public float weight;

        public Neuron source;
        public Neuron destination;


        public NeuronLink(Neuron source, Neuron destination, float weight)
        {
            this.weight = weight;
            this.source = source;
            this.destination = destination;
        }
    }
}
