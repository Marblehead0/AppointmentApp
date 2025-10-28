using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EPDConsole.Application
{
    public class InputValidator
    {
        public string ReadRequired(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    return s!;
                }

                Console.WriteLine("Verplicht veld. Probeer opnieuw.");
            }
        }

        public string? ReadOptional(string label)
        {
            Console.Write($"{label} (optioneel): ");
            return Console.ReadLine();
        }


        public DateTime ReadDate(string label, string formatHint = "dd-MM-yyyy")
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var s = Console.ReadLine();

                if (DateTime.TryParseExact(s, new[] { "dd-MM-yyyy", "dd-MM-yyyy HH:mm", "dd-MM-yyyyTHH:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    return dt;
                }
                if (DateTime.TryParse(s, out dt))
                {
                    return dt;
                }
                Console.WriteLine("Ongeldige datum/tijd. Probeer opnieuw.");
            }
        }

        public string ReadValidEmail(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) &&
                    System.Text.RegularExpressions.Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return input;
                }

                Console.WriteLine("Ongeldig e-mailadres. Probeer opnieuw.");
            }
        }


        public int ReadInt(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var s = Console.ReadLine();
                if (int.TryParse(s, out var v))
                {
                    return v;
                }
                Console.WriteLine("Ongeldig getal. Probeer opnieuw.");
            }
        }

        public string ReadValidNationalNumber(string label)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d{11}$"))
                {
                    return input;
                }
                Console.WriteLine("Ongeldig rijksregisternummer. Gebruik exact 11 cijfers (zonder spaties of letters).");
            }
        }


        //Meer checks doen op ongeldige characters, vooral in naam en voornaam.
        //check toevoegen dat onmiddelijk datums van afspraken in het verleden tegenhoudt voor dat de afpsraak wordt aangemaakt.

    }
}
