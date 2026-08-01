namespace LabRegistry.Models;

public class Loan
{
    public int Id {get;set;}
    public int? StudentId { get; set; }
    public int? EquipmentId { get; set; }
    
    public DateTime LoanDate { get; set; }
    private DateTime _expectedReturnDate;
    public DateTime ExpectedReturnDate
    {
        get => _expectedReturnDate;
        set
        {
            if (value < LoanDate)
                throw new ArgumentException("La fecha de devolución esperada no puede ser anterior a la fecha de préstamo.");

            _expectedReturnDate = value;
        }
    }
    private DateTime? _actualReturnDate;
    public DateTime? ActualReturnDate
    {
        get => _actualReturnDate;
        set => _actualReturnDate = value;
    }

    private LoanStatus _status = LoanStatus.Active;
    public LoanStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public bool IsActive => Status == LoanStatus.Active && !ActualReturnDate.HasValue;
    
    public void Borrow(List<Equipment> equipmentList, List<Student> studentList)
    {
        var equipment = equipmentList.FirstOrDefault(e => e.Id == EquipmentId);
        var student = studentList.FirstOrDefault(s => s.Id == StudentId);

        if (equipment is null)
            throw new InvalidOperationException("No existe el equipo.");

        if (student is null)
            throw new InvalidOperationException("No existe el estudiante.");

        if (equipment.StatusEquipment != StatusEquipment.Available)
            throw new InvalidOperationException("No se puede prestar un equipo que no está disponible.");

        equipment.StatusEquipment = StatusEquipment.InUse;
        Status = LoanStatus.Active;
    }

    public void Return(List<Equipment> equipmentList, DateTime returnDate)
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException("El préstamo ya fue devuelto o cancelado.");

        ActualReturnDate = returnDate;
        Status = LoanStatus.Returned;

        var equipment = equipmentList.FirstOrDefault(e => e.Id == EquipmentId);
        equipment?.StatusEquipment = StatusEquipment.Available;
    }

    public override string ToString()
    {
        return $"[Loan] StudentCode: {StudentId}, EquipmentCode: {EquipmentId}, LoanDate: {LoanDate}, ExpectedReturnDate: {ExpectedReturnDate}, RealReturnDate: {ActualReturnDate}, Status: {Status}";
    }

    
    
}

public enum LoanStatus
{
    Active,
    Returned,
    Overdue
}