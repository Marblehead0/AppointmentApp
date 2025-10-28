namespace Chipsoft.Assignments.EPDConsole.Domain
{
    public class Physician : Person
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Specialty { get; set; } = "General";
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public override string ToString()
        {
            return $"{FullName} - {Specialty}";
        }
    }
}
