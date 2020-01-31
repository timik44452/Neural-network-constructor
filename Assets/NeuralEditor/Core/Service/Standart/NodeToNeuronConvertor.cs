using Core.Neurons;
using Core.Service;
using UnityEngine;

public class NodeToNeuronConvertor : INodeConvertor<Neuron>
{
    public Node ToNode(Neuron value)
    {
        if (value is EntryNeuron)
        {
            return NodeRules.CreateNode(0);
        }
        else if (value is OutputNeuron)
        {
            return NodeRules.CreateNode(1);
        }
        else if (value is SigmoidNeuron)
        {
            return NodeRules.CreateNode(2);
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    public Neuron ToObject(Node node)
    {
        switch (node.Type)
        {
            case 0: return new EntryNeuron() { ID = node.ID };
            case 1: return new OutputNeuron() { ID = node.ID };
            case 2:return new SigmoidNeuron() { ID = node.ID };

            default:
                throw new System.NotImplementedException();
        }
    }
}
