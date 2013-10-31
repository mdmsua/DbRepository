using DbRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbRepositoryTest
{
    [TestClass]
    public class Get
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
        public void Get_Returns_Scalar_Value_3()
        {
            var result = repository.Get<int>("uspSearchCandidateResumes", Parameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Get_Throws_InvalidCastException()
        {
            var result = repository.Get<string>("uspSearchCandidateResumes", Parameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public async Task GetAsync_Returns_Scalar_Value_3()
        {
            var result = await repository.GetAsync<int>("uspSearchCandidateResumes", Parameters.Create(1).Set("searchString", "C#"), CancellationToken.None);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task GetAsync_Throws_TaskCanceledException()
        {
            var result = await repository.GetAsync<int>("uspSearchCandidateResumes", Parameters.Create(1).Set("searchString", "C#"), new CancellationTokenSource(0).Token);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task GetAsync_Throws_InvalidCastException()
        {
            var result = await repository.GetAsync<string>("uspSearchCandidateResumes", Parameters.Create(1).Set("searchString", "C#"), CancellationToken.None);
            Assert.AreEqual(3, result);
        }
    }
}
