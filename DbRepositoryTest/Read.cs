using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbRepositoryTest
{
    [TestClass]
    public class Read
    {
        [TestMethod]
        public void Read_Is_Not_Null()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = new DbRepository.DbRepository().Read<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void Read_Is_Instance_Of_Type()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = new DbRepository.DbRepository().Read<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.IsInstanceOfType(results, typeof(IEnumerable<BillOfMaterials>));
        }

        [TestMethod]
        public void Read_Has_3_Elements()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = new DbRepository.DbRepository().Read<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void Read_First_Element_Equals_Reference()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = new DbRepository.DbRepository().Read<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.ReferenceEquals(Reference, results.First());
        }

        static BillOfMaterials Reference
        {
            get
            {
                return new BillOfMaterials
                {
                    ProductAssemblyID = 3,
                    ComponentID = 461,
                    ComponentDesc = "Lock Ring",
                    TotalQuantity = 1,
                    StandardCost = 0,
                    ListPrice = 0,
                    BOMLevel = 3,
                    RecursionLevel = 0
                };
            }
        }

        [TestMethod]
        public async Task ReadAsync_Is_Not_Null()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = await new DbRepository.DbRepository().ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task ReadAsync_Is_Instance_Of_Type()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = await new DbRepository.DbRepository().ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.IsInstanceOfType(results, typeof(IEnumerable<BillOfMaterials>));
        }

        [TestMethod]
        public async Task ReadAsync_Has_3_Elements()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = await new DbRepository.DbRepository().ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public async Task ReadAsync_First_Element_Equals_Reference()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("StartProductID", 3);
            parameters.Add("CheckDate", new DateTime(2004, 7, 25));
            var results = await new DbRepository.DbRepository().ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", parameters);
            Assert.ReferenceEquals(Reference, results.First());
        }
    }
}
