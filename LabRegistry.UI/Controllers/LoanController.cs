using LabRegistry.Data;
using LabRegistry.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LabRegistry.UI.Controllers;

public class LoanController(LabRegistryDbContext context)
{
    private readonly LabRegistryDbContext _context = context;

    public List<Loan> GetAll()
    {
        return _context.Loan.ToList();
    }

    public Loan? GetById(int id)
    {
        return _context.Loan.FirstOrDefault(r => r.Id == id);
    }

    public void Add(Loan newLoan)
    {
        // 1. Validar que el estudiante existe
        Student? student =  _context.Student.FirstOrDefault(s=>s.Id==newLoan.StudentId);
        if (student == null)
            throw new InvalidOperationException("El estudiante seleccionado no existe.");

        // 2. Validar que el equipo existe
        Equipment? equipment = _context.Equipment.FirstOrDefault(e=>e.Id==newLoan.EquipmentId);
        if (equipment == null)
            throw new InvalidOperationException("El equipo seleccionado no existe.");

        // 3. Validar disponibilidad del equipo
        if (equipment.StatusEquipment != StatusEquipment.Available)
            throw new InvalidOperationException("El equipo no está disponible para préstamo.");
        
        // 4. Iniciar transacción o modificar estados
        equipment.StatusEquipment = StatusEquipment.InUse;

        var loan = new Loan
        {
            StudentId = newLoan.StudentId,
            EquipmentId = newLoan.EquipmentId,
            LoanDate = newLoan.LoanDate,
            ExpectedReturnDate = newLoan.ExpectedReturnDate,
            Status = newLoan.Status 
        };

        _context.Loan.Add(loan);
        _context.SaveChanges();
    }

    public void Update(Loan loanModified)
    {
        // 1. Consultar el estado REAL guardado en PostgreSQL (sin rastreo)
        var loanBdd = _context.Loan
            .AsNoTracking()
            .Where(l => l.Id == loanModified.Id)
            .FirstOrDefault();
        
        // 2. Validar contra el estado real de la BD (Si en la BD no es Active, rebotamos)
        if (loanBdd!.Status != LoanStatus.Active)
        {
            throw new InvalidOperationException("Este préstamo ya fue devuelto o no está activo en la base de datos.");
        }

        Loan? loan = _context.Loan.AsNoTracking().FirstOrDefault(l=>l.Id==loanModified.Id) 
            ?? throw new InvalidOperationException("El prestamo seleccionado no existe.");
        Equipment? equipment = _context.Equipment.FirstOrDefault(e=>e.Id ==loanModified.EquipmentId) 
            ?? throw new InvalidOperationException("El equipo seleccionado no existe.");
        
        // MAPEO AUTOMÁTICO: EF Core copia todo de loanModified a loan
        _context.Entry(loan).CurrentValues.SetValues(loanModified);
        // Si el nuevo estado es Devuelto (Returned), liberamos el equipo
        if (loan.Status == LoanStatus.Returned)
        {
            loan.ActualReturnDate =  loanModified.ActualReturnDate??DateTime.UtcNow;
            equipment.StatusEquipment = StatusEquipment.Available;
        }

        // EF Core detecta los cambios automáticamente y genera los UPDATE correspondientes
        _context.Loan.Update(loanModified);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        Loan? loan = GetById(id);
        if (loan is not null)
        {
            _context.Loan.Remove(loan);
            _context.SaveChanges();
        }
    }

}
