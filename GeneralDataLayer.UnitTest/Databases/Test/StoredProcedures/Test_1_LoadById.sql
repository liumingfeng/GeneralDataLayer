USE [CAS]
GO

/****** Object:  StoredProcedure [dbo].[Test_1_LoadById]    Script Date: 9/21/2020 10:39:39 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Test_1_LoadById]
	@Id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [Id]
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
  WHERE Id = @Id
END
GO


