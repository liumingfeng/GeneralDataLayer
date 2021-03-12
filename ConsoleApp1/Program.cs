using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using GeneralDataLayer;
using Microsoft.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        private static string _connectionString = "Data Source=devboscrmdbag.awsnp.gdotawsnp.com;initial Catalog=CAS;Integrated Security=SSPI;Encrypt=True;TrustServerCertificate=True";

        static void Main(string[] args)
        {
            //Prepare test data
            DataTable dataTable = LoadDataToDataTable();

            GC.Collect();
            Stopwatch watch1 = new Stopwatch();
            watch1.Start();
            List<Test1> all = ExecutionHelper.GenerateListByDataTable<Test1>(dataTable);
            watch1.Stop();
            Console.WriteLine("General data layer spent time: " + watch1.ElapsedMilliseconds.ToString() + "ms");

            GC.Collect();
            Stopwatch watch2 = new Stopwatch();
            watch2.Start();
            List<Test1> list = new List<Test1>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow row = dataTable.Rows[i];
                var test = new Test1()
                {
                    Id = GetValue<long>(row, "Id"),
                    Name = GetValue<string>(row, "Name"),
                    Age = GetValue<int>(row, "Age"),
                    IsActive = GetValue<bool>(row, "IsActive"),
                    CreateDateTime = GetValue<DateTime>(row, "CreateAt"),
                    DoubleValue = GetValue<double>(row, "DoubleValue"),
                    MyGuid = GetValue<Guid?>(row, "MyGuid"),
                    Byte = GetValue<short>(row, "Byte"),
                    Char = GetValue<string>(row, "Char"),
                    Amt = GetValue<decimal>(row, "Amt"),
                    Binary = GetValue<byte[]>(row, "Binary"),
                    DateTimeOffset = GetValue<DateTimeOffset?>(row, "DateTimeOffset")
                };
                list.Add(test);
            }
            watch2.Stop();
            Console.WriteLine("Manual code spent time: " + watch2.ElapsedMilliseconds.ToString() + "ms");
        }

        private static DataTable LoadDataToDataTable()
        {
            DataTable dataTable = new DataTable();
            string query = "[CAS].[dbo].[Test_1_LoadAll]";

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(query, conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            conn.Close();
            da.Dispose();
            return dataTable;
        }

        public static T GetValue<T>(DataRow row, string key)
        {
            var value = row[key];
            return value == DBNull.Value ? default(T) : (T)value;
        }
    }
}
