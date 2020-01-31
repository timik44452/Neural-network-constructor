using UnityEngine;

[System.Serializable]
public class Node
{
    public int ID;
    public int Type;
    public Rect position;

    public Color color;

    public override bool Equals(object obj)
    {
        return obj is Node node &&
               ID == node.ID;
    }

    public override int GetHashCode()
    {
        return 1213502048 + ID.GetHashCode();
    }
}
