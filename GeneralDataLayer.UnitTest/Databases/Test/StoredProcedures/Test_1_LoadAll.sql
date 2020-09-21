CREATE PROCEDURE [dbo].[Test_1_LoadAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT TOP (100000) [Id]
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
  FROM [CAS].[dbo].[Test_1]
END
GO


