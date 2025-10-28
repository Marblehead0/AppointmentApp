# 🩺 EPD Console Application  
*A simple Electronic Patient Record system built in .NET (C#) with Entity Framework Core.*

---

## 📖 Project Description
This is a console-based application that allows a small medical practice to manage:
    - Patients  
    - Physicians  
    - Appointments  

It is built using **Entity Framework Core (SQLite)** and demonstrates clean architecture, validation, and adherence to **SOLID principles**.

---

## 🧩 Features
✅ Add, list, and delete patients and physicians  

✅ Create and view appointments  

✅ Console interface with menu navigation and cancel option

✅ Unit tests (NUnit + EFCore InMemory)

✅ Validation for:
- Email format
- National Register number (11 digits)
- Appointment date not in the past  

---

## 🧱 Architecture
The application follows a layered architecture:
[ Console UI ]
↓
[ Services Layer ] - Business logic and validation
↓
[ Data Layer ] - EF Core context and entity models


Each layer respects **Single Responsibility** and **Dependency Inversion**.

---

## 🧪 Unit Testing
Tests are written using **NUnit** and the **EFCore InMemory** provider.

Example test:
```csharp
[Test]
public void AddAppointment_ShouldThrow_WhenInPast()
{
    var svc = new AppointmentService(GetInMemoryContext());
    Assert.Throws<ArgumentException>(() =>
        svc.AddAsync(patientId, physicianId, DateTime.Now.AddHours(-1), DateTime.Now));
}
```

Run tests with:
    dotnet test

🧰 Technologies
    .NET 8.0        
    Entity Framework Core
    SQLite / InMemory
    NUnit

📂 Folder Overview

| Folder      | Description                                   |
| ----------- | --------------------------------------------- |
| `/Models`   | Entities: Patient, Physician, Appointment     |
| `/Services` | Logic for managing entities                   |
| `/Tests`    | Unit tests for business logic                 |

🚀 How to Run
git clone https://github.com/<your-username>/EPDConsole.git
cd EPDConsole
dotnet run --project Chipsoft.Assignments.EPDConsole

To reset database:
Choose option 8 from the main menu.

🏷️ Tags

C# EntityFramework SQLite .NET ConsoleApp CleanArchitecture NUnit SOLID

---

## 🧩 Commit Suggestions

Use clear commits like:
```bash
git commit -m "Add PatientService with validation and unit tests"
git commit -m "Implement AppointmentService with past-date validation"
git commit -m "Add UseCases.pdf and DesignChoices.pdf documentation"
git commit -m "Improve console input handling with cancel option"
```
