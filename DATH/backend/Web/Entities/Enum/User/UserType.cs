namespace Entities.Enum.User
{
    public enum UserType : ushort
    {
        Admin = 1,
        Employee = 2,
        Customer = 3,
    }

    public enum EmployeeType : ushort
    {
        Sale = 1,
        Orders = 2,
        Warehouse = 3,
    }

    public enum Gender : ushort
    {
        Female = 0,
        Male = 1,
    }
}
