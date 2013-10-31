using DbRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace DbRepositoryTest
{
    [TestClass]
    public class Write
    {
        private DbRepository.DbRepository repository;

        static Parameters Parameters
        {
            get
            {
                return Parameters.Create(1).Set("Value", DateTimeOffset.UtcNow.Ticks);
            }
        }

        static Procedures Procedures
        {
            get
            {
                return Procedures.Create(2).Set("Submit", Parameters).Set("Rebind", Parameters);
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            repository = new DbRepository.DbRepository("Write-Only");
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
            GC.Collect();
        }

        [TestMethod]
        public void Write_One_Procedure_Succeeds()
        {
            var result = repository.Write("Submit", Parameters);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Write_Two_Procedures_Succeeds()
        {
            var result = repository.Write(Procedures);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Write_One_Procedure_Succeeds_Async()
        {
            var result = await repository.WriteAsync("Submit", Parameters);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Write_Two_Procedures_Succeeds_Async()
        {
            var result = await repository.WriteAsync(Procedures);
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task Write_One_Procedure_Fails_With_TaskCanceledException()
        {
            await repository.WriteAsync("Submit", Parameters, new CancellationTokenSource(0).Token);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public async Task Write_Two_Procedures_Fails_With_TaskCanceledException()
        {
            await repository.WriteAsync(Procedures, new CancellationTokenSource(0).Token);
        }
    }
}
