namespace Entities
{
    public class Shop : BaseEntity
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
