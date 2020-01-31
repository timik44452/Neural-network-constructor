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
}
