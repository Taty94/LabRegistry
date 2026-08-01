using System.Text.RegularExpressions;

namespace LabRegistry.Models;

public class Student
{
    public int Id { get; set; }
    private string _name = string.Empty;
    
    public string Name
    {
        get { return _name; }
        set { 
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El nombre no puede estar vacío.");
            }
            _name = value; }
    }

    private string _lastName = string.Empty;

    public string LastName
    {
        get { return _lastName; }
        set { 
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El apellido no puede estar vacío.");
            }
            _lastName = value; }
    }

    private string _email = string.Empty;
    public string Email
    {
        get { return _email; }
        set {
            if (!IsValidEmail(value))
            {
                throw new ArgumentException("El correo institucional no tiene un formato válido.");
            }
            _email = value;
        }
    }

    private static bool IsValidEmail(string email)
    {
         // Patrón de expresión regular para validar el formato del correo electrónico
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public override string ToString()
    {
        return $"[Student-{Id}] Name: {Name}, LastName: {LastName}, Email: {Email}";
    }
}
