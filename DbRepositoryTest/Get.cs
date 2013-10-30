using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbRepositoryTest
{
    [TestClass]
    public class Get
    {
        [TestMethod]
        public void Get_Returns_Scalar_Value_3()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("searchString", "C#");
            var result = new DbRepository.DbRepository().Get<int>("uspSearchCandidateResumes", parameters);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Get_Throws_InvalidCastException()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("searchString", "C#");
            var result = new DbRepository.DbRepository().Get<string>("uspSearchCandidateResumes", parameters);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public async Task GetAsync_Returns_Scalar_Value_3()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("searchString", "C#");
            var result = await new DbRepository.DbRepository().GetAsync<int>("uspSearchCandidateResumes", parameters);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task GetAsync_Throws_InvalidCastException()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("searchString", "C#");
            var result = await new DbRepository.DbRepository().GetAsync<string>("uspSearchCandidateResumes", parameters);
            Assert.AreEqual(3, result);
        }
    }
}
