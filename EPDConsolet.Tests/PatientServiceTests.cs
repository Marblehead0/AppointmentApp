using Chipsoft.Assignments.EPDConsole.Application;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EPDConsole.Tests
{
    public class PatientServiceTests
    {
        [Test]
        public async Task AddPatient_Succeeds_WithValidData()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                var svc = new PatientService(db);

                var p = await svc.AddAsync(
                    nationalNumber: "12345678901",
                    firstName: "Jane",
                    lastName: "Doe",
                    dob: new DateTime(1990, 5, 1),
                    phone: "012345678",
                    email: "jane.doe@example.com",
                    street: "Straat 1",
                    postalCode: "1000",
                    city: "Brussel");

                Assert.That(p.Id, Is.GreaterThan(0));
                Assert.That(p.NationalNumber, Is.EqualTo("12345678901"));
                Assert.That(p.Email, Is.EqualTo("jane.doe@example.com"));
            }
            finally
            {
                conn.Close();
            }
        }

        [Test]
        public async Task DeletePatient_ReturnsTrue_WhenPatientExists()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                var svc = new PatientService(db);

                await svc.AddAsync("12345678902", "Del", "Me", new DateTime(1970, 1, 1), null, "del@example.com", "S1", "1000", "B");

                var removed = await svc.DeleteByNationalNumberAsync("12345678902");

                ClassicAssert.IsTrue(removed);
                var all = await svc.GetAllAsync();
                Assert.That(all.Any(p => p.NationalNumber == "12345678902"), Is.False);
            }
            finally
            {
                conn.Close();
            }
        }

        [Test]
        public async Task DeletePatient_ReturnsFalse_WhenPatientDoesNotExist()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                var svc = new PatientService(db);

                var removed = await svc.DeleteByNationalNumberAsync("12345678903");

                ClassicAssert.IsFalse(removed);
            }
            finally
            {
                conn.Close();
            }
        }


    }
}
