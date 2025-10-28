using Chipsoft.Assignments.EPDConsole.Data;
using Chipsoft.Assignments.EPDConsole.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chipsoft.Assignments.EPDConsole.Application
{
    public class AppointmentService
    {
        private readonly EPDDbContext _db;
        public AppointmentService(EPDDbContext db) => _db = db;

        public async Task<Appointment> AddAsync(int patientId, int physicianId, DateTime startsAt, DateTime endsAt, string? notes)
        {

            if (startsAt < DateTime.Now || endsAt < DateTime.Now)
            {
                throw new ArgumentException("Een afspraak mag niet in het verleden liggen.");
            }
                
            if (endsAt <= startsAt)
            {
                throw new ArgumentException("Eindtijd moet na starttijd liggen.");
            }

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == patientId);
            if (!patientExists)
            {
                throw new InvalidOperationException("Patiënt bestaat niet.");
            }

            var physicianExists = await _db.Physicians.AnyAsync(p => p.Id == physicianId);
            if (!physicianExists)
            {
                throw new InvalidOperationException("Arts bestaat niet.");
            }

            var overlap = await _db.Appointments.AnyAsync(a =>
                a.PhysicianId == physicianId &&
                a.StartsAt < endsAt &&
                startsAt < a.EndsAt);

            if (overlap)
            {
                throw new InvalidOperationException("Arts heeft al een overlappende afspraak.");
            }

            var a = new Appointment
            {
                PatientId = patientId,
                PhysicianId = physicianId,
                StartsAt = startsAt,
                EndsAt = endsAt,
                Notes = notes
            };

            _db.Appointments.Add(a);
            await _db.SaveChangesAsync();
            return a;
        }

        public Task<List<Appointment>> GetForPhysicianAsync(int physicianId)
            => _db.Appointments
                  .Where(a => a.PhysicianId == physicianId)
                  .OrderBy(a => a.StartsAt)
                  .ToListAsync();

        public Task<List<Appointment>> GetAllAsync()
            => _db.Appointments
                  .Include(a => a.Patient)
                  .Include(a => a.Physician)
                  .OrderBy(a => a.StartsAt)
                  .ToListAsync();
    }
}
