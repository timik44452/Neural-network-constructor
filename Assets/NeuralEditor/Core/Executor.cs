using Core.Neurons;
using Core.Service;

using System.Text;
using System.Collections.Generic;

public class Executor
{
    private ILogger logger;
    private NetworkConfiguration configuration;
    private INodeConvertor<Neuron> convertor;

    private List<Neuron> neurons;

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

                from.out_weights.Add(neuronLink);
                to.input_weights.Add(neuronLink);
            }
            else
            {
                logger.Error($"Найдена связка несуществующих нейронов");
            }
        }
    }

    public float Lern(float rate, params float[] values)
    {
        float error = 0;

        var outNeurons = neurons.FindAll(x => x is OutputNeuron);

        if (outNeurons.Count == values.Length)
        {
            for (int i = 0; i < outNeurons.Count; i++)
            {
                float o = outNeurons[i].Value;
                float t = values[i];

                error += (o - t) * (o - t) * 0.5F;

                outNeurons[i].Lern(rate, values[i]);
            }
        }
        else
        {
            logger.Error($"Размер массива входных данных не соответствует количеству входных нейронов");
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
                entryNeurons[i].Input(values[i]);
            }
        }
        else
        {
            logger.Error($"Размер массива входных данных не соответствует количеству входных нейронов");
        }
    }

    public void GetOutput(out float[] output)
    {
        var outputs = neurons.FindAll(x => x is OutputNeuron);

        float[] result = new float[outputs.Count];

        for (int i = 0; i < outputs.Count; i++)
        {
            OutputNeuron output_neuron = (OutputNeuron)outputs[i];

            result[i] = output_neuron.Value;
        }

        output = result;
    }

    public string OutToString()
    {
        string text = string.Empty;

        StringBuilder stringBuilder = new StringBuilder(text);

        neurons.FindAll(x => x is OutputNeuron).ForEach(x =>
        {
            text += $"{((OutputNeuron)x).Value.ToString()}\n";
        });

        return text;
    }
}
