namespace Entities.Enum.Order
{
    public enum OrderType : ushort
    {
        Pending = 1,
        Rejected = 27,
        Preparing = 2,
        Delivering = 23,
        Received = 32
    }
}
