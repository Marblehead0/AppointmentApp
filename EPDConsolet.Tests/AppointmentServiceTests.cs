using Chipsoft.Assignments.EPDConsole.Application;
using Chipsoft.Assignments.EPDConsole.Domain;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EPDConsole.Tests
{
    public class AppointmentServiceTests
    {
        // Succesvolle aanmaak van afspraak
        [Test]
        public async Task AddAppointment_Succeeds_When_ValidData()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                // Arrange: seed patient + physician
                var patient = new Patient
                {
                    NationalNumber = "11111111111",
                    FirstName = "Test",
                    LastName = "Patient",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Email = "test@p.be",
                    Street = "Straat 1",
                    PostalCode = "1000",
                    City = "Brussel"
                };
                var physician = new Physician
                {
                    RegistrationNumber = "REG1",
                    FirstName = "Dr",
                    LastName = "Test",
                    Specialty = "General"
                };
                db.Patients.Add(patient);
                db.Physicians.Add(physician);
                db.SaveChanges();

                var svc = new AppointmentService(db);

                var start = DateTime.Now.AddHours(2);
                var end = start.AddMinutes(30);

                // Act
                var added = await svc.AddAsync(patient.Id, physician.Id, start, end, "Checkup");

                // Assert
                ClassicAssert.IsNotNull(added);
                Assert.That(added.PatientId, Is.EqualTo(patient.Id));
                Assert.That(added.PhysicianId, Is.EqualTo(physician.Id));
                Assert.That(added.StartsAt, Is.EqualTo(start));
                Assert.That(added.EndsAt, Is.EqualTo(end));
            }
            finally
            {
                conn.Close();
            }
        }

        // Afspraak in het verleden mag niet
        [Test]
        public void AddAppointment_Throws_When_InPast()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                // seed valid patient + physician
                db.Patients.Add(new Patient
                {
                    NationalNumber = "22222222222",
                    FirstName = "A",
                    LastName = "B",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Email = "a@b.com",
                    Street = "S",
                    PostalCode = "1000",
                    City = "B"
                });
                db.Physicians.Add(new Physician
                {
                    RegistrationNumber = "REG2",
                    FirstName = "Dr",
                    LastName = "Test",
                    Specialty = "General"
                });
                db.SaveChanges();

                var svc = new AppointmentService(db);

                var start = DateTime.Now.AddDays(-1);
                var end = start.AddMinutes(30);

                // Assert
                var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                    await svc.AddAsync(db.Patients.First().Id, db.Physicians.First().Id, start, end, null));
                Assert.That(ex.Message, Does.Contain("niet in het verleden"));
            }
            finally
            {
                conn.Close();
            }
        }

        // Overlap check
        [Test]
        public async Task AddAppointment_Throws_On_Overlap()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                var patient = new Patient
                {
                    NationalNumber = "33333333333",
                    FirstName = "C",
                    LastName = "D",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Email = "c@d.com",
                    Street = "S",
                    PostalCode = "1000",
                    City = "B"
                };
                var physician = new Physician
                {
                    RegistrationNumber = "REG3",
                    FirstName = "Dr",
                    LastName = "Test",
                    Specialty = "General"
                };
                db.Patients.Add(patient);
                db.Physicians.Add(physician);
                db.SaveChanges();

                var svc = new AppointmentService(db);

                var start1 = DateTime.Now.AddHours(3);
                var end1 = start1.AddMinutes(30);

                // create first appointment
                var a1 = await svc.AddAsync(patient.Id, physician.Id, start1, end1, null);
                ClassicAssert.IsNotNull(a1);

                // overlapping appointment (starts inside first)
                var start2 = start1.AddMinutes(15);
                var end2 = start2.AddMinutes(30);

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    await svc.AddAsync(patient.Id, physician.Id, start2, end2, null));
                Assert.That(ex.Message, Does.Contain("overlappende"));
            }
            finally
            {
                conn.Close();
            }
        }

        // Patiënt bestaat niet
        [Test]
        public void AddAppointment_Throws_When_PatientMissing()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                // seed only physician
                db.Physicians.Add(new Physician
                {
                    RegistrationNumber = "REG4",
                    FirstName = "Dr",
                    LastName = "Test",
                    Specialty = "General"
                });
                db.SaveChanges();

                var svc = new AppointmentService(db);

                var start = DateTime.Now.AddHours(2);
                var end = start.AddMinutes(20);

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    await svc.AddAsync(9999, db.Physicians.First().Id, start, end, null));

                Assert.That(ex.Message, Does.Contain("Patiënt"));
            }
            finally
            {
                conn.Close();
            }
        }

        // Arts bestaat niet
        [Test]
        public void AddAppointment_Throws_When_PhysicianMissing()
        {
            var (db, conn) = TestHelpers.CreateInMemory();
            try
            {
                // seed only patient
                db.Patients.Add(new Patient
                {
                    NationalNumber = "44444444444",
                    FirstName = "E",
                    LastName = "F",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Email = "e@f.com",
                    Street = "S",
                    PostalCode = "1000",
                    City = "B"
                });
                db.SaveChanges();

                var svc = new AppointmentService(db);

                var start = DateTime.Now.AddHours(2);
                var end = start.AddMinutes(20);

                var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    await svc.AddAsync(db.Patients.First().Id, 9999, start, end, null));

                Assert.That(ex.Message, Does.Contain("Arts"));
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
