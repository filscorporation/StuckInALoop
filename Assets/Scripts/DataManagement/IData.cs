namespace DataManagement
{
    public interface IData
    {
        int Priority { get; }
        DataObject ToObject();
    }
}