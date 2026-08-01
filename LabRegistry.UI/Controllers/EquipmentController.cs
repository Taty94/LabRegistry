using LabRegistry.Data;
using LabRegistry.Models;
using System.Collections.Generic;
using System.Linq;

namespace LabRegistry.UI.Controllers;

public class EquipmentController(LabRegistryDbContext context)
{
    private readonly LabRegistryDbContext _context = context;

    public List<Equipment> GetAll()
        {
            return _context.Equipment.ToList();
        }

        public Equipment? GetById(int id)
        {
            return _context.Equipment.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Equipment newEquipment)
        {
            _context.Equipment.Add(newEquipment);
            _context.SaveChanges();
        }

        public void Update(Equipment equipmentModified)
        {
            _context.Equipment.Update(equipmentModified);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Equipment? equipment = GetById(id);
            if (equipment is not null)
            {
                _context.Equipment.Remove(equipment);
                _context.SaveChanges();
            }
        }
}
