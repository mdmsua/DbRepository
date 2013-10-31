using DbRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbRepositoryTest
{
    [TestClass]
    public class Read
    {
        private DbRepository.IDbRepository repository;

        [TestInitialize]
        public void Initialize()
        {
            repository = new DbRepository.DbRepository();
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
            GC.Collect();
        }
        
        [TestMethod]
        public void Read_Is_Not_Null()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void Read_Is_Instance_Of_Type()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.IsInstanceOfType(results, typeof(IEnumerable<BillOfMaterials>));
        }

        [TestMethod]
        public void Read_Has_3_Elements()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void Read_First_Element_Equals_Reference()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.ReferenceEquals(Reference, results.First());
        }

        [TestMethod]
        public async Task ReadAsync_Is_Not_Null()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters, CancellationToken.None);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task ReadAsync_Is_Instance_Of_Type()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters, CancellationToken.None);
            Assert.IsInstanceOfType(results, typeof(IEnumerable<BillOfMaterials>));
        }

        [TestMethod]
        public async Task ReadAsync_Has_3_Elements()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters, CancellationToken.None);
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public async Task ReadAsync_First_Element_Equals_Reference()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters, CancellationToken.None);
            Assert.ReferenceEquals(Reference, results.First());
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task ReadAsync_Throws_TaskCanceledException()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters, new CancellationTokenSource(0).Token);
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

        static Parameters Parameters
        {
            get
            {
                return Parameters.Create(2).Set("StartProductID", 3).Set("CheckDate", new DateTime(2004, 7, 25));
            }
        }
    }
}
