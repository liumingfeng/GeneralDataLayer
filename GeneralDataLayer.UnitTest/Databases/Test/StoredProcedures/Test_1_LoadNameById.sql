USE [CAS]
GO

/****** Object:  StoredProcedure [dbo].[Test_1_LoadNameById]    Script Date: 9/21/2020 10:40:17 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Test_1_LoadNameById]
	@Id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT [Name]
  FROM [CAS].[dbo].[Test_1]
  WHERE Id = @Id
END
GO


