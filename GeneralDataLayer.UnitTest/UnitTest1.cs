using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeneralDataLayer.UnitTest
{
    public class Tests
    {
        /// <summary>
        /// This is database connection string
        /// </summary>
        private string _connectionString = "Data Source=devboscrmdbag.awsnp.gdotawsnp.com;initial Catalog=CAS;Integrated Security=SSPI;Encrypt=True;TrustServerCertificate=True";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestForLoadList()
        {
            //Load all data
            List<Test1> all = await ExecutionHelper.ExecuteReaderListAsync<Test1>(_connectionString, "[CAS].[dbo].[Test_1_LoadAll]");

            Assert.Pass();
        }

        [Test]
        public async Task TestForLoadEntity()
        {
            //Load data by id
            Test1Query query = new Test1Query()
            {
                Id = 8
            };
            Test1 part = await ExecutionHelper.ExecuteReaderAsync<Test1Query, Test1>(_connectionString, "[CAS].[dbo].[Test_1_LoadById]", query);


            Assert.Pass();
        }

        [Test]
        public async Task TestForLoadScalar()
        {
            //Load scalar object
            Test1Query query = new Test1Query()
            {
                Id = 8
            };
            object name = await ExecutionHelper.ExecuteScalarAsync<Test1Query>(_connectionString, "[CAS].[dbo].[Test_1_LoadNameById]", query);

            Assert.Pass();
        }

        [Test]
        public async Task TestForAddData()
        {
            //Add Data
            Test1 test1 = new Test1()
            {
                CreateDateTime = DateTime.Now,
                Name = "ddd",
                MyGuid = Guid.NewGuid(),
                Char = "jjj",
                Binary = Encoding.UTF8.GetBytes("asdf".ToCharArray()),
                DateTimeOffset = new DateTimeOffset(DateTime.Now)
            };
            await ExecutionHelper.ExecuteNonQueryAsync(_connectionString, "[CAS].[dbo].[Test_1_Add]", test1);

            Assert.Pass();
        }
    }
}