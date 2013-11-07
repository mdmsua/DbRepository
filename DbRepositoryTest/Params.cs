using DbRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DbRepositoryTest
{
    [TestClass]
    public class Params
    {
        [TestMethod]
        public void From_Parameters_Count()
        {
            var parameters = Parameters.From<BillOfMaterials>(Reference);
            Assert.AreEqual<int>(8, parameters.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(CapacityExceededException))]
        public void From_Throws_CapacityExceededException()
        {
            Parameters.From<BillOfMaterials>(Reference).Set("foo", "bar");
        }

        [TestMethod]
        public void Create_Parameters_Count()
        {
            var parameters = Parameters.Create(1);
            Assert.AreEqual<int>(0, parameters.Count);
        }

        [TestMethod]
        public void Set_Parameters_Key()
        {
            var parameters = Parameters.Create(1).Set("foo", "bar");
            Assert.AreEqual<string>("foo", parameters.Keys.Single());
        }

        [TestMethod]
        public void Set_Parameters_Value()
        {
            var parameters = Parameters.Create(1).Set("foo", "bar");
            Assert.AreEqual<string>("bar", (string)parameters.Values.Single());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Set_Parameters_Throws_ArgumentException()
        {
            Parameters.Create(2).Set("foo", "bar").Set("foo", "bar");
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
    }
}
