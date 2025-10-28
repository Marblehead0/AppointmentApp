namespace Chipsoft.Assignments.EPDConsole.Domain
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int PhysicianId { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string? Notes { get; set; }

        public Patient? Patient { get; set; }
        public Physician? Physician { get; set; }

    }
}
