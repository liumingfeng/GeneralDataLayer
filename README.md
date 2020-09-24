# GeneralDataLayer
# How to use?
# Example 1: Load a list from database by sp
  List<Test1> all = await ExecutionHelper.ExecuteReaderListAsync<Test1>(_connectionString, "[CAS].[dbo].[Test_1_LoadAll]");

# Examlpe 2: Load a entity from database
  Test1Query query = new Test1Query()
  {
      Id = 8
  };
  Test1 part = await ExecutionHelper.ExecuteReaderAsync<Test1Query, Test1>(_connectionString, "[CAS].[dbo].[Test_1_LoadById]", query);

# Example 3: Load scalare object
  Test1Query query = new Test1Query()
  {
      Id = 8
  };
  object name = await ExecutionHelper.ExecuteScalarAsync<Test1Query>(_connectionString, "[CAS].[dbo].[Test_1_LoadNameById]", query);
  
# Example 4: Add data to database
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
  
# Set up Stored procedure params
public class Test1
{
    ...
    [Parameter]
    public string Name { get; set; }
    [Parameter(Name = "@NewId", Direction = ParameterDirection.Output)]
    public long Id { get; set; }
    [Parameter(Direction = ParameterDirection.ReturnValue)]
    public int? ReturnData { get; set; }
    ...
}

# Set up the returned columns to object:
public class Test1
{
    ...
    [Column]
    public long Id { get; set; }
    [Column]
    public string Name { get; set; }
    ...
}

# Support the rename of columns and params to object:
public class Test1
{
    ...
    [Parameter(Name = "@NewId", Direction = ParameterDirection.Output)]
    public long Id { get; set; }
    [Column(Name = "CreateAt")]
    [Parameter(Name = "CreateAt")]
    public DateTime CreateDateTime { get; set; }
    ...
}
