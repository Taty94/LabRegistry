using System.Collections.Generic;
using System.Linq;
using LabRegistry.Data;
using LabRegistry.Models;

namespace LabRegistry.UI.Controllers;

public class StudentController
{
    private readonly LabRegistryDbContext _context;

    public StudentController(LabRegistryDbContext context)
    {
        _context = context;
    }

    
        public List<Student> GetAll()
        {
            return _context.Student.ToList();
        }

        public Student? GetById(int id)
        {
            return _context.Student.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Student newStudent)
        {
            _context.Student.Add(newStudent);
            _context.SaveChanges();
        }

        public void Update(Student studentModified)
        {
            _context.Student.Update(studentModified);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Student? student = GetById(id);
            if (student is not null)
            {
                _context.Student.Remove(student);
                _context.SaveChanges();
            }
        }
}
