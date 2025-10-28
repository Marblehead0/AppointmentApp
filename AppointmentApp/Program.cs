using Chipsoft.Assignments.EPDConsole.Application;
using Chipsoft.Assignments.EPDConsole.Data;
using EPDConsole.Application;
using System.Globalization;
using System.Reflection.Emit;

namespace Chipsoft.Assignments.EPDConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Zorg dat DB bestaat
            using (var db = new EPDDbContext())
            {
                db.Database.EnsureCreated();
            }

            while (ShowMenu()) { }
        }

        public static bool ShowMenu()
        {
            Console.Clear();
            if (File.Exists("logo.txt"))
            {
                foreach (var line in File.ReadAllLines("logo.txt"))
                    Console.WriteLine(line);
            }
            Console.WriteLine();
            Console.WriteLine("1 - Patiënt toevoegen");
            Console.WriteLine("2 - Patiënten verwijderen");
            Console.WriteLine("3 - Arts toevoegen");
            Console.WriteLine("4 - Arts verwijderen");
            Console.WriteLine("5 - Afspraak toevoegen");
            Console.WriteLine("6 - Afspraken inzien (per arts)");
            Console.WriteLine("7 - Alle afspraken tonen");
            Console.WriteLine("8 - Sluiten");
            Console.WriteLine("9 - Reset db (let op: verwijdert alles)");
            Console.WriteLine();

            var input = Console.ReadLine();
            if (!int.TryParse(input, out var option))
            {
                return true;
            } 

            switch (option)
            {
                case 1: AddPatient(); return true;
                case 2: DeletePatient(); return true;
                case 3: AddPhysician(); return true;
                case 4: DeletePhysician(); return true;
                case 5: AddAppointment(); return true;
                case 6: ShowAppointment(); return true;
                case 7: ShowAllAppointments(); return true;
                case 8: return false;
                case 9:
                    using (var db = new EPDDbContext())
                    {
                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
                    }
                    Console.WriteLine("Database gereset.");
                    Pause();
                    return true;
                default:
                    return true;
            }
        }

        //#region Input-helpers
        //Verplaatst naar aparte validator helper 'InputValidator.cs'

        //static string ReadRequired(string label)
        //{
        //    while (true)
        //    {
        //        Console.Write($"{label}: ");
        //        var s = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(s))
        //        {
        //            return s!;
        //        }

        //        Console.WriteLine("Verplicht veld. Probeer opnieuw.");
        //    }
        //}

        //static string? ReadOptional(string label)
        //{
        //    Console.Write($"{label} (optioneel): ");
        //    return Console.ReadLine();
        //}

        //static DateTime ReadDate(string label, string formatHint = "dd-MM-yyyy")
        //{
        //    while (true)
        //    {
        //        Console.Write($"{label} ({formatHint}): ");
        //        var s = Console.ReadLine();

        //        if (DateTime.TryParseExact(s, new[] { "dd-MM-yyyy", "dd-MM-yyyy HH:mm", "dd-MM-yyyyTHH:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        //        {
        //            return dt;
        //        }
        //        if (DateTime.TryParse(s, out dt))
        //        {
        //            return dt;
        //        }
        //        Console.WriteLine("Ongeldige datum/tijd. Probeer opnieuw.");
        //    }
        //}

        //private static string ReadValidEmail(string label)
        //{
        //    while (true)
        //    {
        //        Console.Write($"{label}: ");
        //        var input = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(input) &&
        //            System.Text.RegularExpressions.Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        //        {
        //            return input;
        //        }

        //        Console.WriteLine("Ongeldig e-mailadres. Probeer opnieuw.");
        //    }
        //}

        //static int ReadInt(string label)
        //{
        //    while (true)
        //    {
        //        Console.Write($"{label}: ");
        //        var s = Console.ReadLine();
        //        if (int.TryParse(s, out var v))
        //        {
        //            return v;
        //        }
        //        Console.WriteLine("Ongeldig getal. Probeer opnieuw.");
        //    }
        //}

        //private static string ReadValidNationalNumber(string label)
        //{
        //    while (true)
        //    {
        //        Console.Write($"{label}: ");
        //        var input = Console.ReadLine();
        //        if (!string.IsNullOrWhiteSpace(input) && System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d{11}$"))
        //        {
        //            return input;
        //        }
        //        Console.WriteLine("Ongeldig rijksregisternummer. Gebruik exact 11 cijfers (zonder spaties of letters).");
        //    }
        //}


        //#endregion

        private static void AddPatient()
        {
            using var db = new EPDDbContext();
            var svc = new PatientService(db);
            var val = new InputValidator();

            Console.WriteLine("--- Patiënt toevoegen ---");
            var nn = val.ReadValidNationalNumber("Rijksregisternummer (11 cijfers): ");
            var fn = val.ReadRequired("Voornaam");
            var ln = val.ReadRequired("Achternaam");
            var dob = val.ReadDate("Geboortedatum", "dd-MM-yyyy");
            var phone = val.ReadOptional("Telefoon");
            var email = val.ReadValidEmail("E-mail");
            var street = val.ReadRequired("Straat + nr");
            var postal = val.ReadRequired("Postcode");
            var city = val.ReadRequired("Gemeente");

            try
            {
                var p = svc.AddAsync(nn, fn, ln, dob, phone, email, street, postal, city).GetAwaiter().GetResult();
                Console.WriteLine($"Patiënt toegevoegd: {p}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout: {ex.Message}");
            }

            Pause();
        }

        private static void DeletePatient()
        {
            using var db = new EPDDbContext();
            var svc = new PatientService(db);
            var val = new InputValidator();

            Console.WriteLine("--- Patiënt verwijderen ---");
            var nn = val.ReadRequired("Rijksregisternummer");
            try
            {
                var ok = svc.DeleteByNationalNumberAsync(nn).GetAwaiter().GetResult();
                Console.WriteLine(ok ? "Verwijderd." : "Patiënt niet gevonden.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout: {ex.Message}");
            }

            Pause();
        }

        private static void AddPhysician()
        {
            using var db = new EPDDbContext();
            var svc = new PhysicianService(db);
            var val = new InputValidator();

            Console.WriteLine("--- Arts toevoegen ---");
            var reg = val.ReadRequired("Registratienummer");
            var fn = val.ReadRequired("Voornaam");
            var ln = val.ReadRequired("Achternaam"); 
            var dob = val.ReadDate("Geboortedatum", "dd-MM-yyyy");
            var phone = val.ReadOptional("Telefoon");
            var email = val.ReadValidEmail("E-mail");
            var street = val.ReadRequired("Straat + nr");
            var postal = val.ReadRequired("Postcode");
            var city = val.ReadRequired("Gemeente");
            Console.Write("Specialisatie (optioneel): ");
            var specialty = Console.ReadLine();

            try
            {
                var d = svc.AddAsync(reg, fn, ln, dob, phone, email, street, postal, city, specialty).GetAwaiter().GetResult();
                Console.WriteLine($"Arts toegevoegd: {d}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout: {ex.Message}");
            }

            Pause();
        }

        private static void DeletePhysician()
        {
            using var db = new EPDDbContext();
            var svc = new PhysicianService(db);
            var val = new InputValidator();

            Console.WriteLine("--- Arts verwijderen ---");
            var reg = val.ReadRequired("Registratienummer");
            try
            {
                var ok = svc.DeleteByRegistrationNumberAsync(reg).GetAwaiter().GetResult();
                Console.WriteLine(ok ? "Verwijderd." : "Arts niet gevonden.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout: {ex.Message}");
            }

            Pause();
        }

        private static void AddAppointment()
        {
            using var db = new EPDDbContext();
            var pSvc = new PatientService(db);
            var dSvc = new PhysicianService(db);
            var aSvc = new AppointmentService(db);
            var val = new InputValidator();

            var pts = pSvc.GetAllAsync().GetAwaiter().GetResult();
            var docs = dSvc.GetAllAsync().GetAwaiter().GetResult();

            if (pts.Count == 0 || docs.Count == 0)
            {
                Console.WriteLine("Voeg eerst minstens 1 patiënt en 1 arts toe.");
                Pause();
                return;
            }

            Console.WriteLine("Patiënten:");
            foreach (var p in pts)
            {
                Console.WriteLine($"  {p.Id}: {p}");
            } 
            var pid = val.ReadInt("PatientId");

            Console.WriteLine("Artsen:");
            foreach (var d in docs)
            {
                Console.WriteLine($"  {d.Id}: {d}");
            }
            var did = val.ReadInt("PhysicianId");

            var start = val.ReadDate("Start Afspraak, Dag-Maand-Jaar Uur:Min");
            var end = val.ReadDate("Einde Afspraak, Dag-Maand-Jaar Uur:Min");

            Console.Write("Notities (optioneel): ");
            var notes = Console.ReadLine();

            try
            {
                var a = aSvc.AddAsync(pid, did, start, end, notes).GetAwaiter().GetResult();
                Console.WriteLine($"Afspraak #{a.Id} toegevoegd: {a.StartsAt:dd-MM-yyyy HH:mm} - {a.EndsAt:HH:mm}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout: {ex.Message}");
            }

            Pause();
        }

        private static void ShowAppointment()
        {
            using var db = new EPDDbContext();
            var dSvc = new PhysicianService(db);
            var aSvc = new AppointmentService(db);
            var val = new InputValidator();

            var docs = dSvc.GetAllAsync().GetAwaiter().GetResult();
            if (docs.Count == 0)
            { 
                Console.WriteLine("Geen artsen.");
                Pause(); 
                return;
            }

            Console.WriteLine("Artsen:");
            foreach (var d in docs)
            {
                Console.WriteLine($"  {d.Id}: {d}");
            } 
            var did = val.ReadInt("PhysicianId");

            var list = aSvc.GetForPhysicianAsync(did).GetAwaiter().GetResult();
            if (list.Count == 0)
            {
                Console.WriteLine("Geen afspraken.");
            } 
            foreach (var a in list)
            {
                Console.WriteLine($"#{a.Id} {a.StartsAt:dd-MM-yyyy HH:mm} - {a.EndsAt:HH:mm} | PatientId {a.PatientId} | {a.Notes}");
            }

            Pause();
        }

        private static void ShowAllAppointments()
        {
            using var db = new EPDDbContext();
            var aSvc = new AppointmentService(db);


            var list = aSvc.GetAllAsync().GetAwaiter().GetResult();
            if (list.Count == 0)
            {
                Console.WriteLine("Geen afspraken."); 
                Pause(); 
                return; 
            }

            foreach (var a in list)
            {
                var patient = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : $"PatientId {a.PatientId}";
                var physician = a.Physician != null ? a.Physician.FullName : $"PhysicianId {a.PhysicianId}";
                Console.WriteLine($"#{a.Id} {a.StartsAt:dd-MM-yyyy HH:mm} - {a.EndsAt:HH:mm} | {patient} -- {physician} | {a.Notes}");
            }

            Pause();
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Druk op een toets om verder te gaan...");
            Console.ReadKey(true);
        }
    }
}


//Uitbreding van functies:

//Optie om uit functie te stappen, bv: Typ 'X' om te annuleren
//Appointment verwijderen
//Show appointment uitbreiden voor patient

