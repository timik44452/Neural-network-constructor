namespace Core.Service
{
    public interface INodeConvertor<T>
    {
        Node ToNode(T value);
        T ToObject(Node node);
    }
}