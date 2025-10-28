namespace Chipsoft.Assignments.EPDConsole.Domain
{
    public class Patient : Person
    {
        public string NationalNumber { get; set; } = string.Empty;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public override string ToString()
        {
            return $"{FullName} - {NationalNumber} ({City})";
        }
    }
}
