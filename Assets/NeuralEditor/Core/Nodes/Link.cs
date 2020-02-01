[System.Serializable]
public class Link
{
    public int from;
    public int to;

    public Link Clone()
    {
        return (Link)MemberwiseClone();
    }

    public override bool Equals(object obj)
    {
        return obj is Link link &&
               from == link.from &&
               to == link.to;
    }

    public override int GetHashCode()
    {
        var hashCode = -1951484959;
        hashCode = hashCode * -1521134295 + from.GetHashCode();
        hashCode = hashCode * -1521134295 + to.GetHashCode();
        return hashCode;
    }
}
