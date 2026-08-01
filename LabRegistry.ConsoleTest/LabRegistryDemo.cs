using LabRegistry.Data;
using LabRegistry.Models;

namespace LabRegistry.ConsoleTest;

public class LabRegistryDemo(LabRegistryDbContext context)
{
    private readonly LabRegistryDbContext _context = context;
    private Equipment? _equipment;
    private Student? _student;
    private Loan? _loan;
    public void Run()
    {
        Console.Clear();
        PrintHeader("Pruebas de reglas de negocio del laboratorio");

        PrintInfo("1) Verificando conexión a PostgreSQL...");
        if (!_context.Database.CanConnect())
        {
            PrintWarning("No se pudo conectar a la base de datos. Las pruebas se ejecutarán de forma local.");
        }
        else
        {
            PrintSuccess("Conexión exitosa.");
        }

        Console.WriteLine();

        _equipment = CreateEquipment("Microscopio", "Óptica");
        _student = CreateStudent("Ana", "Pérez", "ana.perez@university.edu");
        _loan = CreateLoan(_student.Id,_equipment.Id);

        RegisterLoanHappyPath();
        ValidateCanBorrowScenario();
        ValidateExpectedReturnDateScenario();
        ValidateDeleteScenario();
        ValidateReturnScenario();

        Console.WriteLine();
        PrintHeader("Fin de las pruebas");
    }

    private Equipment CreateEquipment(string name, string category)
    {
        // 1. Validar si ya existe un registro con ese nombre
        var existingEquipment = _context.Equipment.FirstOrDefault(e => e.Name == name);
        if (existingEquipment != null)
        {
            Console.WriteLine($"El equipo '{name}' ya existía en la base de datos. Se retorna el existente.");
            return existingEquipment;
        }
        var equipment = new Equipment
        {
            Name = name,
            Category = category,
            StatusEquipment = StatusEquipment.Available
        };

        _context.Equipment.Add(equipment);
        _context.SaveChanges();
        Console.WriteLine($"Equipo creado con ID: {equipment.Id}");
        return equipment;
    }

    private Student CreateStudent(string name, string lastName, string email)
    {
        // 1. Validar si ya existe un registro con ese email
        var existingStudent = _context.Student.FirstOrDefault(e => e.Name == name);
        if (existingStudent != null)
        {
            Console.WriteLine($"El studiante con correo '{email}' ya existía en la base de datos. Se retorna el existente.");
            return existingStudent;
        }
        var student = new Student
        {
            Name = name,
            LastName = lastName,
            Email = email
        };

        _context.Student.Add(student);
        _context.SaveChanges();

        Console.WriteLine($"Estudiante creado con ID: {student.Id}");
        return student;
    }

    private Loan CreateLoan(int studentId, int equipmentId)
    {
        var loan = new Loan
        {
            StudentId = studentId,
            EquipmentId = equipmentId,
            LoanDate = DateTime.UtcNow,
            ExpectedReturnDate = DateTime.UtcNow.AddDays(3)
        };

        _context.Loan.Add(loan);
        _context.SaveChanges();
        Console.WriteLine($"Préstamo creado con ID: {loan.Id}");
        return loan;
    }

    private void RegisterLoanHappyPath()
    {
        PrintSection("Escenario 0: Happy Path Registro Prestamo");
        
        var equipment = _equipment!;
        var student = _student!;
        var loan = _loan!;

        Console.WriteLine($"Estado del equipo antes de intentar prestar: {equipment.StatusEquipment}");
        Console.WriteLine("Acción: intentar prestar el equipo...");

        try
        {
            loan.Borrow(new List<Equipment> { equipment }, new List<Student> { student });
            Console.WriteLine($"Estado del equipo despues de prestar: {equipment.StatusEquipment}");
            PrintSuccess("El préstamo fue permitido.");
        }
        catch (Exception ex)
        {
            PrintFailure($"Registro de prestamo incorrecto: {ex.Message}");
        }

        Console.WriteLine();
    }
    private void ValidateCanBorrowScenario()
    {
        PrintSection("Escenario 1: No se puede prestar un equipo si su estado no es disponible");
        
        var equipment = _equipment!;
        var student = _student!;
        var loan = _loan!;

        Console.WriteLine($"Estado del equipo antes de intentar prestar: {equipment.StatusEquipment}");
        Console.WriteLine("Acción: intentar prestar el equipo...");

        try
        {
            loan.Borrow(new List<Equipment> { equipment }, new List<Student> { student });
            PrintFailure("El préstamo fue permitido, pero debería haber fallado.");
        }
        catch (Exception ex)
        {
            PrintSuccess($"Validación correcta: {ex.Message}");
        }

        Console.WriteLine();
    }

    private void ValidateExpectedReturnDateScenario()
    {
        PrintSection("Escenario 2: La fecha de devolución esperada no puede ser anterior a la fecha de préstamo");

        var loan = _loan!;

        Console.WriteLine($"Fecha de préstamo: {loan.LoanDate:dd/MM/yyyy}");
        Console.WriteLine("Acción: asignar una fecha de devolución anterior a la fecha de préstamo...");

        try
        {
            loan.ExpectedReturnDate = DateTime.Today.AddDays(-1);
            PrintFailure("Se permitió guardar una fecha de devolución inválida.");
        }
        catch (Exception ex)
        {
            PrintSuccess($"Validación correcta: {ex.Message}");
        }

        Console.WriteLine();
    }

    private void ValidateDeleteScenario()
    {
        PrintSection("Escenario 3: No se puede eliminar un equipo que tenga un préstamo activo");
        
        var equipment = _equipment!;
        var loan = _loan!;


        Console.WriteLine($"Estado del equipo: {equipment.StatusEquipment}");
        Console.WriteLine("Acción: verificar si se puede eliminar...");

        var canDelete = equipment.CanBeDeleted(new List<Loan> { loan });
        Console.WriteLine($"Resultado de la validación: {(canDelete ? "Se puede eliminar" : "No se puede eliminar")}");

        if (!canDelete)
        {
            PrintSuccess("Validación correcta: no se permite eliminar un equipo con préstamo activo.");
        }
        else
        {
            PrintFailure("El equipo fue marcado como eliminable, pero debería estar protegido.");
        }

        Console.WriteLine();
    }


    private void ValidateReturnScenario()
    {
        PrintSection("Escenario 4: Al registrar la devolución, el equipo debe volver automáticamente a estado disponible");
        var equipment = _equipment!;
        var loan = _loan!;
        

        Console.WriteLine($"Estado actual del equipo: {equipment.StatusEquipment}");
        Console.WriteLine("Acción: Intentar devolver el equipo...");

        loan.Return(new List<Equipment> { equipment }, DateTime.Today.AddDays(1));
        Console.WriteLine($"Estado después de la devolución: {equipment.StatusEquipment}");

        if (loan.Status == LoanStatus.Returned && equipment.StatusEquipment == StatusEquipment.Available)
        {
            PrintSuccess("Validación correcta: el equipo volvió automáticamente a disponible.");
        }
        else
        {
            PrintFailure("El equipo no volvió a disponible como se esperaba.");
        }

        Console.WriteLine();
    }


    private static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================");
        Console.WriteLine(title);
        Console.WriteLine("========================================");
        Console.ResetColor();
    }

    private static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(title);
        Console.ResetColor();
    }

    private static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK] {message}");
        Console.ResetColor();
    }

    private static void PrintFailure(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[FAIL] {message}");
        Console.ResetColor();
    }

    private static void PrintInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }


}
