using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiveWorld.NeuralNetworkCore
{
    public class Neuron
    {
        public List<Synapse> InputSynapses { get; set; }
        public List<Synapse> OutputSynapses { get; set; }
        public float Gradient { get; set; }
        public float Value { get; set; }

        public Neuron()
        {
            InputSynapses = new List<Synapse>();
            OutputSynapses = new List<Synapse>();
        }

        public Neuron(IEnumerable<Neuron> inputNeurons) : this()
        {
            foreach (var inputNeuron in inputNeurons)
            {
                var synapse = new Synapse(inputNeuron, this);
                inputNeuron.OutputSynapses.Add(synapse);
                InputSynapses.Add(synapse);
            }
        }

        public virtual float CalculateValue()
        {
            return Value = Sigmoid.Output(InputSynapses.Sum(a => a.Weight * a.InputNeuron.Value));
        }

        public float CalculateError(float target)
        {
            return target - Value;
        }

        public float CalculateGradient(float? target = null)
        {
            if (target == null)
                return Gradient = OutputSynapses.Sum(a => a.OutputNeuron.Gradient * a.Weight) * Sigmoid.Derivative(Value);

            return Gradient = CalculateError(target.Value) * Sigmoid.Derivative(Value);
        }

        public void UpdateWeights(float learnRate, float momentum)
        {
            float prevDelta;

            foreach (var synapse in InputSynapses)
            {
                prevDelta = synapse.WeightDelta;
                synapse.WeightDelta = learnRate * Gradient * synapse.InputNeuron.Value;
                synapse.Weight += synapse.WeightDelta + momentum * prevDelta;
            }
        }
    }

    public class Synapse
    {
        public Neuron InputNeuron { get; set; }
        public Neuron OutputNeuron { get; set; }
        public float Weight { get; set; }
        public float WeightDelta { get; set; }

        public Synapse(Neuron inputNeuron, Neuron outputNeuron)
        {
            InputNeuron = inputNeuron;
            OutputNeuron = outputNeuron;
            Weight = 0.5F;// NeuralNet.GetRandom();
        }
    }

    public static class Sigmoid
    {
        public static float Output(double x)
        {
            return x < -45.0F ? 0.0F : x > 45.0F ? 1.0F : 1.0F / (1.0F + Mathf.Exp((float)-x));
        }

        public static float Derivative(float x)
        {
            return x * (1 - x);
        }
    }

    public class DataSet
    {
        public float[] Values { get; set; }
        public float[] Targets { get; set; }

        public DataSet(float[] values, float[] targets)
        {
            Values = values;
            Targets = targets;
        }
    }
}