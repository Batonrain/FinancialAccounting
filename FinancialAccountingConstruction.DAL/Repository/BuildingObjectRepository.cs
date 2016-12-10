using System.Collections.Generic;
using System.Linq;
using FinancialAccountingConstruction.DAL.Models.Building;

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

        public BuildingObject GetObjectById(int id)
        {
            return _context.BuildingObjects.Single(obj => obj.Id == id);
        }
    }
}
