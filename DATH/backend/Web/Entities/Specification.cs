namespace Entities
{
    public class Specification : BaseEntity<long>
    {
        public string? Code { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }

        //Relationship
        public int SpecificationCategoryId { get; set; }
        public SpecificationCategory? SpecificationCategory { get; set; }
    }
}
