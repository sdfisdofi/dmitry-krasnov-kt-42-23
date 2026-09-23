namespace dmitry_krasnov_kt_42_23.Models
{
    // Зачет: сдал / не сдал
    public class Credit
    {
        public int CreditId { get; set; }
        public bool IsPassed { get; set; }
        public DateTime Date { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int DisciplineId { get; set; }
        public Discipline? Discipline { get; set; }
    }
}
