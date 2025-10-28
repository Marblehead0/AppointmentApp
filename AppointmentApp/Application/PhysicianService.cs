using Chipsoft.Assignments.EPDConsole.Data;
using Chipsoft.Assignments.EPDConsole.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chipsoft.Assignments.EPDConsole.Application
{
    public class PhysicianService
    {
        private readonly EPDDbContext _db;
        public PhysicianService(EPDDbContext db) => _db = db;

        public async Task<Physician> AddAsync(
            string registrationNumber,
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            string? phone,
            string email,
            string street,
            string postalCode,
            string city,
            string? specialty)
        {

            registrationNumber = registrationNumber.Trim();

            if (await _db.Physicians.AnyAsync(d => d.RegistrationNumber == registrationNumber))
                throw new InvalidOperationException("Arts met dit registratienummer bestaat al.");

            var d = new Physician
            {
                RegistrationNumber = registrationNumber,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                DateOfBirth = dateOfBirth,
                Phone = phone?.Trim(),
                Email = email.Trim(),
                Street = street.Trim(),
                PostalCode = postalCode.Trim(),
                City = city.Trim(),
                Specialty = string.IsNullOrWhiteSpace(specialty) ? "General" : specialty.Trim()
            };

            _db.Physicians.Add(d);
            await _db.SaveChangesAsync();
            return d;
        }

        public async Task<bool> DeleteByRegistrationNumberAsync(string regNr)
        {
            if (string.IsNullOrWhiteSpace(regNr)) return false;
            var d = await _db.Physicians.FirstOrDefaultAsync(x => x.RegistrationNumber == regNr.Trim());
            if (d == null) return false;
            _db.Physicians.Remove(d);
            await _db.SaveChangesAsync();
            return true;
        }

        public Task<List<Physician>> GetAllAsync()
        {
            return _db.Physicians
                      .OrderBy(p => p.Id)
                      .ToListAsync();
        }

    }
}
