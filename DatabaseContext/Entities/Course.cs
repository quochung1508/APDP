namespace SIMS.DatabaseContext.Entities
{
    public class Course
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // MỚI: Chuyên ngành
        public string? Major { get; set; }

        // MỚI: Thời gian học (4 tháng, 5 tháng, 1 năm, Khác)
        public string? Duration { get; set; }
    }
}