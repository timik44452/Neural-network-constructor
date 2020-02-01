using System;
using System.Collections.Generic;
using UnityEngine;

public static class NodeInspectorContainerFactory
{
    public static NodeInspectorContainer CreateContainer(Node node, NetworkConfiguration configuration, NeuralEditorWindow window)
    {
        if(node == null || configuration == null || window == null)
        {
            return null;
        }

        NodeInspectorContainer container = ScriptableObject.CreateInstance<NodeInspectorContainer>();

        container.node = node;
        container.window = window;
        container.configuration = configuration;

        return container;
    }

    public static NodeInspectorContainer[] CreateContainer(List<Node> selected_nodes, NetworkConfiguration configuration, NeuralEditorWindow window)
    {
        List<NodeInspectorContainer> nodeInspectorContainers = new List<NodeInspectorContainer>();

        selected_nodes.ForEach(x => CreateContainer(x, configuration, window));

        return nodeInspectorContainers.ToArray();
    }
}
