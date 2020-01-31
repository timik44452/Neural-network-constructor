using UnityEditor;

using System.Linq;
using System.Collections.Generic;


[CustomEditor(typeof(NodeInspectorContainer))]
public class NodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        NodeInspectorContainer container = target as NodeInspectorContainer;

        EditorGUILayout.LabelField($"ID: {container.node.ID}");

        container.node.Type = ToType(EditorGUILayout.Popup("Type", ToIndex(container.node.Type), ToPopup()));
        container.node.color = EditorGUILayout.ColorField("Color", container.node.color);

        container.window.Repaint();
    }

    private int ToIndex(int number)
    {
        var nodeTypes = Core.Service.NodeRules.GetNodeList();

        int index = nodeTypes.Values.ToList().IndexOf(number);

        return (index >= 0) ? index : 0;
    }

    private int ToType(int index)
    {
        var nodeTypes = Core.Service.NodeRules.GetNodeList();

        return nodeTypes.Values.ElementAt(index);
    }

    private string[] ToPopup()
    {
        var nodeTypes = Core.Service.NodeRules.GetNodeList();
        var nodeTypesResult = new List<string>();

        foreach (var value in nodeTypes)
        {
            nodeTypesResult.Add(value.Key);
        }

        return nodeTypesResult.ToArray();
    }
}
