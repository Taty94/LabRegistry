#region CODIGO DE CONSOLA
// using LabRegistry.Models;

// // Create sample data for testing
// List<Equipment> equipments = new List<Equipment>
// {
//     new() { Code = 1, Name = "Microscope", Category = "Optical", StatusEquipment= StatusEquipment.InUse},
//     new() { Code = 2, Name = "Centrifuge", Category = "Mechanical" },
//     new() { Code = 3, Name = "Spectrophotometer", Category = "Optical" }
// };

// Console.WriteLine("---- Equipment List ----");
// foreach (var equipment in equipments)
// {
//     Console.WriteLine(equipment);
// }
// Console.WriteLine("------------------------\n");

// List<Student> students = new List<Student>
// {
//     new() { Code = 101, Name = "Alice", LastName = "Smith", Email = "alice.smith@university.edu" },
//     new() { Code = 102, Name = "Bob", LastName = "Johnson", Email = "bob.johnson@university.edu" },
//     new() { Code = 103, Name = "Charlie", LastName = "Williams", Email = "charlie.williams@university.edu" }
// };

// Console.WriteLine("---- Student List ----");
// foreach (var student in students)
// {
//     Console.WriteLine(student);
// }
// Console.WriteLine("------------------------\n");

// List<Loan> loans = new List<Loan>();
// var loan = new Loan {Code=1, StudentCode = 101, EquipmentCode = 2, LoanDate = DateTime.Today, ExpectedReturnDate = DateTime.Today.AddDays(2) };


// loan.Borrow(equipments,students);
// loans.Add(loan);
// Console.WriteLine("---- Prestamo creado ----");
// Console.WriteLine(loan);
// Console.WriteLine(equipments.FirstOrDefault(e => e.Code == 2));

// Console.WriteLine("---- Prestamo erroneo ----");
// try
// {
//     var loan2 = new Loan
//     {
//         Code= 2,
//         EquipmentCode = 1,
//         StudentCode = 102,
//         LoanDate = DateTime.Today,
//         ExpectedReturnDate = DateTime.Today.AddDays(2)
//     };

//     loan2.Borrow(equipments,students);
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"\nError al prestar: {ex.Message}");
// }
// Console.WriteLine("------------------------\n");

// Console.WriteLine("\nEliminar equipo con prestamo activo:");
// if (equipments[1].CanBeDeleted(loans))
// {
//     equipments.Remove(equipments[1]);
//     Console.WriteLine("\nEquipo eliminado correctamente.");
// }
// else
// {
//     Console.WriteLine("\nNo se puede eliminar el equipo porque tiene un préstamo activo.");
// }
// Console.WriteLine("------------------------\n");

// loan.Return(equipments,DateTime.Today.AddDays(2));

// Console.WriteLine("\nPréstamo devuelto:");
// Console.WriteLine(loan);
// Console.WriteLine(equipments.FirstOrDefault(e => e.Code == 2));
#endregion


using Microsoft.EntityFrameworkCore;
using LabRegistry.Data;
using LabRegistry.ConsoleTest;

//Configuration a PostgresSQL
string connectionString ="Host=localhost;Port=5432;Database=lab_registry;Username=postgres;Password=admin";
var optionBuilder = new DbContextOptionsBuilder<LabRegistryDbContext>();
optionBuilder.UseNpgsql(connectionString);


using var context = new LabRegistryDbContext(optionBuilder.Options);

var demo = new LabRegistryDemo(context);
demo.Run();
