using Core.Neurons;
using Core.Service;

using System.Linq;
using System.Text;
using System.Collections.Generic;

public class Executor
{
    private ILogger logger;
    private NetworkConfiguration configuration;
    private INodeConvertor<Neuron> convertor;

    private List<Neuron> neurons;
    private List<Neuron> outputNeurons
    {
        get
        {
            return neurons.FindAll(x => x is OutputNeuron);
        }
    }

    public Executor(ILogger logger, INodeConvertor<Neuron> convertor, NetworkConfiguration configuration)
    {
        this.logger = logger;
        this.convertor = convertor;
        this.configuration = configuration;

        neurons = new List<Neuron>();

        Rebuild();
    }

    public void Rebuild()
    {
        neurons = new List<Neuron>();

        if (configuration == null)
        {
            return;
        }

        foreach (var node in configuration.nodes)
        {
            neurons.Add(convertor.ToObject(node));
        }

        System.Random random = new System.Random();

        foreach (var link in configuration.links)
        {
            var from = neurons.Find(x => x.ID == link.from);
            var to = neurons.Find(x => x.ID == link.to);

            if (from != null && to != null)
            {
                NeuronLink neuronLink = new NeuronLink(from, to, (float)random.NextDouble());

                from.out_synapse.Add(neuronLink);
                to.input_synapse.Add(neuronLink);
            }
            else
            {
                logger.Error($"Найдена связка несуществующих нейронов", this);
            }
        }
    }

    public float Lern(float rate, params float[] targetValues)
    {
        float error = 0;

        List<OutputNeuron> outNeurons = outputNeurons.Select(x => (OutputNeuron)x).ToList();

        if (outNeurons.Count == targetValues.Length)
        {
            for (int i = 0; i < outNeurons.Count; i++)
            {
                outNeurons[i].CalculateGradient(targetValues[i]);
                outNeurons[i].UpdateWeights(rate);
                outNeurons[i].Lern(rate);
            }
        }
        else
        {
            logger.Error($"Размер массива входных данных не соответствует количеству входных нейронов", this);
        }

        return error;
    }

    public void Invoke(params float[] values)
    {
        var entryNeurons = neurons.FindAll(x => x is EntryNeuron);

        if (entryNeurons.Count == values.Length)
        {
            for (int i = 0; i < values.Length; i++)
            {
                entryNeurons[i].Value = values[i];
                entryNeurons[i].Input();
            }
        }
        else
        {
            logger.Error($"Размер массива входных данных не соответствует количеству входных нейронов", this);
        }
    }

    public void GetOutput(out float[] output)
    {
        var outputs = neurons.FindAll(x => x is OutputNeuron);

        output = new float[outputs.Count];

        for (int i = 0; i < outputs.Count; i++)
        {
            output[i] = outputs[i].Value;
        }
    }

    public string OutToString()
    {
        string text = string.Empty;

        StringBuilder stringBuilder = new StringBuilder(text);

        neurons.FindAll(x => x is OutputNeuron).ForEach(x =>
        {
            text += $"{((OutputNeuron)x).Gradient.ToString()}\n";
        });

        return text;
    }
}
