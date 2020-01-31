using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu]
public class NetworkConfiguration : ScriptableObject
{
    public List<Link> links = new List<Link>();
    public List<Node> nodes = new List<Node>();

    public void AddLink(Link link)
    {
        if (link == null)
        {
            return;
        }

        if (links.Find(x => x.from == link.from && x.to == link.to) != null)
        {
            return;
        }

        links.Add(link);
    }

    public void RemoveLink(int from, int to)
    {
        if (links.Find(x => x.from == from && x.to == to) != null)
        {
            links.RemoveAll(x => x.from == from && x.to == to);
        }
    }

    public void AddNode(Node neuron)
    {
        neuron.ID = GetId();

        nodes.Add(neuron);

        Save();
    }

    public void RemoveNode(int Id)
    {
        links.RemoveAll(x => x.from == Id || x.to == Id);
        nodes.RemoveAll(x => x.ID == Id);
    }

    private int GetId()
    {
        if (nodes.Count == 0)
        {
            return 0;
        }

        return nodes.Max(x => x.ID) + 1;
    }

    private void Save()
    {
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}
