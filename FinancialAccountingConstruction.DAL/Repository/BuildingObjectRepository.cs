using System.Collections.Generic;
using System.Linq;
using FinancialAccountingConstruction.DAL.Models.Building;
using FinancialAccountingConstruction.DAL.Models.Contracts;

namespace FinancialAccountingConstruction.DAL.Repository
{
    public class BuildingObjectRepository 
    {
        private readonly FinancialAccountingDbContext _context;

        public BuildingObjectRepository()
        {
            _context = new FinancialAccountingDbContext();
        }

        public IEnumerable<BuildingObject> GetAllBuildingObjects()
        {
            return _context.BuildingObjects.ToList();
        }

        public void AddBuildingObject(BuildingObject buildingObject)
        {
            _context.BuildingObjects.Add(buildingObject);
            _context.SaveChanges();
        }

        public void UpdateBuildingObject(BuildingObject buildingObject)
        {
            _context.BuildingObjects.Attach(buildingObject);
            var entry = _context.Entry(buildingObject);
            entry.Property(e => e.Name).IsModified = true;
            entry.Property(e => e.Notes).IsModified = true;
            _context.SaveChanges();
        }

        public BuildingObject GetObjectById(int id)
        {
            return _context.BuildingObjects.Single(obj => obj.Id == id);
        }

        public void RemoveBuildingObjectById(int id)
        {
            var toDelete = _context.BuildingObjects.Single(obj => obj.Id == id);
            _context.BuildingObjects.Remove(toDelete);
            _context.SaveChanges();
        }

        public Contractor GetContractorById(int id)
        {
            return _context.Contractors.Single(obj => obj.Id == id);
        }
    }
}
