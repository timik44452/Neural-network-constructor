using Core.Neurons;
using System.Collections;
using UnityEngine;

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

    public Function function = Function.AND;

    public float learningRate = 0.01F;

    #region Hide

    private Executor executor;
    private Core.Service.ILogger logger;
    private Core.Service.INodeConvertor<Neuron> convertor;

    private Coroutine thread;

    #endregion Hide

    public void Run()
    {
        if (thread == null)
        {
            thread = StartCoroutine(Thread());
        }
        else
        {
            StopCoroutine(thread);
            thread = null;
        }
    }

    private void Start()
    {
        logger = new UnityDebugLogger();
        convertor = new NodeToNeuronConvertor();

        executor = new Executor(logger, convertor, configuration);
    }

    private IEnumerator Thread()
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