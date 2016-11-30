using System.Collections.Generic;
using System.Linq;
using FinancialAccountingConstruction.DAL.Models;

namespace FinancialAccountingConstruction.DAL.Repository
{
    public class BuildingObjectRepository 
    {
        public List<BuildingObject> GetAllBuildingObjects()
        {
            var db = new FinancialAccountingDbContext();
            return db.BuildingObjects.ToList();
        }
    }
}
