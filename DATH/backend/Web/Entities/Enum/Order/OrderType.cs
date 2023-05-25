namespace Entities.Enum.Order
{
    public enum OrderStatus : byte
    {
        Pending = 1,
        Rejected = 27,
        Preparing = 2,
        Delivering = 23,
        Received = 32
    }

    public enum InstallmentStatus : byte
    {
        Paid = 1,
        Unpaid = 2,
    }

    public enum PaymentType : byte
    {
        PayStraight = 1,
        Installment = 2
    }
}
