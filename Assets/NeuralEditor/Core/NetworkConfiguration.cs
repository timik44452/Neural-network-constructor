using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;


[CreateAssetMenu]
public class NetworkConfiguration : ScriptableObject
{
    public List<Link> links = new List<Link>();
    public List<Node> nodes = new List<Node>();

    public void AddLink(Link link)
    {
        if (link == null || links.Contains(link))
        {
            return;
        }

        links.Add(link);
    }

    public void RemoveLink(Link link)
    {
        links.Remove(link);

        Save();
    }

    public void AddNode(Node node)
    {
        node.ID = GetID();

        nodes.Add(node);

        Save();
    }

    public void RemoveNode(Node node)
    {
        if(node == null)
        {
            return;
        }
        
        links.RemoveAll(x => x.from == node.ID || x.to == node.ID);
        nodes.Remove(node);
    }

    private int GetID()
    {
        if(nodes.Count() == 0)
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

    public void Dublicate(params Node[] nodes)
    {
        if (this.nodes.Count == 0 || nodes.Length == 0)
        {
            return;
        }

        int stride = GetID() - nodes.Max(x => x.ID);

        foreach(Node node in nodes)
        {
            Node cloneNode = node.Clone();

            cloneNode.ID = GetID();

            links.FindAll(x => x.to == node.ID).ForEach(x =>
            {
                Link cloneLink = x.Clone();

                cloneLink.to = cloneNode.ID;

                links.Add(cloneLink);
            });


            links.FindAll(x => x.from == node.ID).ForEach(x =>
            {
                Link cloneLink = x.Clone();

                cloneLink.from = cloneNode.ID;

                links.Add(cloneLink);
            });

            this.nodes.Add(cloneNode);
        }
    }
}
