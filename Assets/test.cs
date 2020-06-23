
using System.Collections.Generic;
using UnityEngine;
using LiveWorld.NeuralNetworkCore;

public class test : MonoBehaviour
{
    public float a, b, c;

    private NeuralNet neuralNet;

    private void Start()
    {
        var list = new List<DataSet>();

        list.Add(new DataSet(new float[] { 1, 1 }, new float[] { 0 }));
        list.Add(new DataSet(new float[] { 1, 0 }, new float[] { 1 }));
        list.Add(new DataSet(new float[] { 0, 1 }, new float[] { 1 }));
        list.Add(new DataSet(new float[] { 0, 0 }, new float[] { 0 }));

        neuralNet = new NeuralNet(2, 5, 1);
        neuralNet.Train(list, 1000);
    }

    private void FixedUpdate()
    {
        c = neuralNet.Run(a, b)[0];
    }
}
