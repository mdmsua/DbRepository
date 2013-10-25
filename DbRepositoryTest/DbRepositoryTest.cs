using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbRepositoryTest
{
    [TestClass]
    public class DbRepositoryTest
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

        [TestMethod]
        public void Get()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("searchString", "C#");
            var result = new DbRepository.DbRepository().Get<int>("uspSearchCandidateResumes", parameters);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public async Task GetAsync()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("searchString", "C#");
            var result = await new DbRepository.DbRepository().GetAsync<int>("uspSearchCandidateResumes", parameters);
            Assert.AreEqual(3, result);
        }
    }
}
