using Core.Neurons;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;


public class LogicalExample : MonoBehaviour
{
    private struct LernData
    {
        public float input0;
        public float input1;
        public float result;

        public LernData(float input0, float input1, float result)
        {
            this.input0 = input0;
            this.input1 = input1;
            this.result = result;
        }
    }

    public enum Function
    {
        AND,
        OR,
        XOR
    }

    public NetworkConfiguration configuration;

    public float learningRate = 0.01F;

    public Text[] out_texts;

    #region Hide

    private Function function = Function.AND;

    private Executor executor;
    private Core.Service.ILogger logger;
    private Core.Service.INodeConvertor<Neuron> convertor;

    private Coroutine learning_thread;
    private Coroutine visualization_thread;

    #endregion Hide

    public void Run()
    {
        if (learning_thread != null)
        {
            StopCoroutine(learning_thread);
        }

        if (visualization_thread != null)
        {
            StopCoroutine(visualization_thread);
        }

        learning_thread = StartCoroutine(LearningThread());
        visualization_thread = StartCoroutine(VisualizationThread());
    }

    public void Stop()
    {
        if (learning_thread != null)
        {
            StopCoroutine(learning_thread);
        }

        if (visualization_thread != null)
        {
            StopCoroutine(visualization_thread);
        }

        learning_thread = null;
        visualization_thread = null;
    }

    public void ChangeFunction(int functionIndex)
    {
        function = (Function)functionIndex;

        Stop();
        Run();
    }

    private void Start()
    {
        logger = new UnityDebugLogger();
        convertor = new NodeToNeuronConvertor();

        executor = new Executor(logger, convertor, configuration);

        Run();
    }

    private IEnumerator LearningThread()
    {
        LernData[] dataset = GetDataset();

        while (true)
        {
            foreach (var data in dataset)
            {
                executor.Invoke(data.input0, data.input1);
                executor.Lern(learningRate, data.result);
            }

            yield return new WaitForSeconds(0.01F);
        }
    }

    private IEnumerator VisualizationThread()
    {
        LernData[] dataset = GetDataset();

        for (int i = 0; i < dataset.Length; i++)
        {
            out_texts[i * 3].text = dataset[i].input0.ToString();
            out_texts[i * 3 + 1].text = dataset[i].input1.ToString();
        }

        while (true)
        {
            for (int line = 0; line < dataset.Length; line++)
            {
                LernData data = dataset[line];
                executor.Invoke(data.input0, data.input1);

                executor.GetOutput(out float[] output);

                out_texts[line * 3 + 2].text = output[0].ToString("0.00");
            }

            yield return new WaitForSeconds(0.25F);
        }
    }

    private LernData[] GetDataset()
    {
        switch (function)
        {
            case Function.AND:
                return new LernData[]
                {
                    new LernData(0, 0, 0),
                    new LernData(0, 1, 0),
                    new LernData(1, 0, 0),
                    new LernData(1, 1, 1),
                };

            case Function.OR:
                return new LernData[]
                {
                    new LernData(0, 0, 0),
                    new LernData(0, 1, 1),
                    new LernData(1, 0, 1),
                    new LernData(1, 1, 1),
                };

            case Function.XOR:
                return new LernData[]
                {
                    new LernData(0, 0, 0),
                    new LernData(0, 1, 1),
                    new LernData(1, 0, 1),
                    new LernData(1, 1, 0),
                };
        }

        return new LernData[0];
    }
}