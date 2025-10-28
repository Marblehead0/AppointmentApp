using Chipsoft.Assignments.EPDConsole.Data;
using Chipsoft.Assignments.EPDConsole.Domain;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Chipsoft.Assignments.EPDConsole.Application
{
    public class PatientService
    {
        private readonly EPDDbContext _db;
        public PatientService(EPDDbContext db) => _db = db;

        public async Task<Patient> AddAsync(
            string nationalNumber,
            string firstName,
            string lastName,
            DateTime dob,
            string? phone,
            string email,
            string street,
            string postalCode,
            string city)
        {

            // Uniekheidscheck
            if (await _db.Patients.AnyAsync(p => p.NationalNumber == nationalNumber))
            {
                throw new InvalidOperationException("Patiënt met dit rijksregisternummer bestaat al.");
            }

            var p = new Patient
            {
                NationalNumber = nationalNumber,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                DateOfBirth = dob,
                Phone = phone?.Trim(),
                Email = email.Trim(),
                Street = street.Trim(),
                PostalCode = postalCode.Trim(),
                City = city.Trim()
            };

            _db.Patients.Add(p);
            await _db.SaveChangesAsync();
            return p;
        }


        public async Task<bool> DeleteByNationalNumberAsync(string nationalNumber)
        {
            if (string.IsNullOrWhiteSpace(nationalNumber)) return false;
            var p = await _db.Patients.FirstOrDefaultAsync(x => x.NationalNumber == nationalNumber.Trim());
            if (p == null) return false;
            _db.Patients.Remove(p);
            await _db.SaveChangesAsync();
            return true;
        }

        public Task<List<Patient>> GetAllAsync()
            => _db.Patients.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToListAsync();


    }
}
