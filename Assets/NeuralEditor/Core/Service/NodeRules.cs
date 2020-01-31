using System.Collections.Generic;

namespace Core.Service
{
    public static class NodeRules
    {
        public static Dictionary<string, int> GetNodeList()
        {
            Dictionary<string, int> nodes = new Dictionary<string, int>();

            nodes.Add("Entry node", 0);
            nodes.Add("Out node", 1);
            nodes.Add("Sigmoid node", 2);

            return nodes;
        }

        public static Node CreateNode(int Type)
        {
            Node node = new Node();

            node.Type = Type;

            switch (Type)
            {
                case 0: node.color = ColorAtlas.gray; break;
                case 1: node.color = ColorAtlas.gray; break;
                case 2: node.color = ColorAtlas.gray; break;
            }

            return node;
        }

        public static bool ContainsLeftConnection(Node node)
        {
            return node.Type != 0;
        }

        public static bool ContainsRightConnection(Node node)
        {
            return node.Type != 1;
        }
    }
}