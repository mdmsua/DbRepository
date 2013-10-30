using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DbRepositoryTest
{
    [TestClass]
    public class Ctor
    {
        [TestMethod]
        public void Ctor_Default()
        {
            new DbRepository.DbRepository();
        }

        [TestMethod]
        public void Ctor_Configuration_Key()
        {
            new DbRepository.DbRepository("AdventureWorks2012");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Ctor_Invalid_Configuration_Key()
        {
            new DbRepository.DbRepository("Invalid");
        }
    }
}
