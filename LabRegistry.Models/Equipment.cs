namespace LabRegistry.Models;

public class Equipment
{
    public int Id { get; set; }
    private string _name = string.Empty;

    public string Name
    {
        get { return _name; }
        set { 
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El nombre del equipo no puede estar vacío.");
            }
            _name = value; }
    }

    private  string _category = string.Empty;
    public string Category
    {
        get { return _category; }
        set { 
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("La categoría del equipo no puede estar vacía.");
            }
            _category = value; }
    }

    public StatusEquipment StatusEquipment {get;set;} = StatusEquipment.Available;


    public bool CanBeDeleted(IEnumerable<Loan> loans)
    {
        return !loans.Any(l => l.EquipmentId==Id && l.IsActive);
    }

    public override string ToString()
    {
        return $"[Equipment-{Id}]Name: {Name}, Category: {Category}, Status: {StatusEquipment}";
    }

}   

public enum StatusEquipment
{
    Available,
    InUse,
    Maintenance
}