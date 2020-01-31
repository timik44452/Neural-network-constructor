using Core.Neurons;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TrainData
{
    public float[] input;
    public float[] output;
}

public class Test : MonoBehaviour
{
    public NetworkConfiguration configuration;

    public InputField inputField;
    public Button runButton;

    public float rate = 0.01F;
    public float[] input;
    public TrainData[] train_set;

    public bool is_training = true;

    private Executor executor;
    private Core.Service.ILogger logger;
    private Core.Service.INodeConvertor<Neuron> convertor;

    private Coroutine thread;

    private void Start()
    {
        logger = new UnityDebugLogger();
        convertor = new NodeToNeuronConvertor();

        executor = new Executor(logger, convertor, configuration);

        runButton.onClick.AddListener(Run);
        inputField.onEndEdit.AddListener(Parse);
    }

    private void Run()
    {
        if(thread == null)
        {
            thread = StartCoroutine(Thread());
        }
        else
        {
            StopCoroutine(thread);
            thread = null;
        }
    }

    private IEnumerator Thread()
    {
        while (true)
        {
            //if (is_training)
            {
                float a = 0;

                foreach (TrainData data in train_set)
                {
                    executor.Invoke(data.input);

                    a = Mathf.Max(a, executor.Lern(rate, data.output));
                }

                executor.Invoke(input);

                logger.Log($"Error:{a} OUT:{executor.OutToString()}");
            }

            yield return new WaitForSeconds(0.01F);
        }
    }

    private void Parse(string value)
    {
        string[] parts = value.Split(' ');

        value = value.Replace('.', ',');

        input = new float[parts.Length];

        for(int i = 0; i < input.Length; i++)
        {
            float.TryParse(parts[i], out input[i]);
        }
    }
}
