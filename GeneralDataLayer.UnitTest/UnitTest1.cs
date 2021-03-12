using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
            var commandContent = new CommandContent()
            {
                Type = CommandType.StoredProcedure,
                Value = "[CAS].[dbo].[Test_1_LoadAll]"
            };
            List<Test2> all = await ExecutionHelper.ExecuteReaderListAsync<Test2>(_connectionString, commandContent);

            Assert.IsTrue(all.Count > 0);
        }

        [Test]
        public async Task TestForLoadList2()
        {
            //Load all data
            var commandContent = new CommandContent()
            {
                Type = CommandType.Text,
                Value = @"SELECT TOP (100000) [Id]
                              ,[Name]
                              ,[Age]
                              ,[IsActive]
                              ,[CreateAt]
                              ,[DoubleValue]
                              ,[MyGuid]
                              ,[Byte]
                              ,[Char]
                              ,[Amt]
                              ,[Binary]
                              ,[DateTimeOffset]
                          FROM [CAS].[dbo].[Test_1]"
            };
            List<Test1> all = await ExecutionHelper.ExecuteReaderListAsync<Test1>(_connectionString, commandContent);

            Assert.IsTrue(all.Count > 0);
        }

        [Test]
        public async Task TestForLoadEntity()
        {
            //Load data by id
            Test1Query query = new Test1Query()
            {
                Id = 8
            };
            var commandContent = new CommandContent()
            {
                Type = CommandType.StoredProcedure,
                Value = "[CAS].[dbo].[Test_1_LoadById]"
            };
            Test1 part = await ExecutionHelper.ExecuteReaderAsync<Test1Query, Test1>(_connectionString, commandContent, query);

            Assert.IsTrue(part.Id == 8);
        }

        [Test]
        public async Task TestForLoadScalar()
        {
            //Load scalar object
            Test1Query query = new Test1Query()
            {
                Id = 8
            };
            var commandContent = new CommandContent()
            {
                Type = CommandType.StoredProcedure,
                Value = "[CAS].[dbo].[Test_1_LoadNameById]"
            };
            object name = await ExecutionHelper.ExecuteScalarAsync<Test1Query>(_connectionString, commandContent, query);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(name.ToString()));
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
            var commandContent = new CommandContent()
            {
                Type = CommandType.StoredProcedure,
                Value = "[CAS].[dbo].[Test_1_Add]"
            };
            await ExecutionHelper.ExecuteNonQueryAsync(_connectionString, commandContent, test1);

            Assert.IsTrue(test1.ReturnData == 8);
        }
    }
}