CREATE PROCEDURE [dbo].[Test_1_Add]
	@Name nvarchar(100)
	,@Age int
    ,@IsActive bit
    ,@CreateAt datetime
    ,@DoubleValue float
    ,@MyGuid uniqueidentifier
    ,@Byte smallint
    ,@Char varchar(10)
    ,@Amt decimal(18,2) 
    ,@Binary binary(50)
    ,@DateTimeOffset datetimeoffset(7)
	,@NewId bigint output
AS
BEGIN
     INSERT INTO [dbo].[Test_1]
           ([Name]
           ,[Age]
           ,[IsActive]
           ,[CreateAt]
           ,[DoubleValue]
           ,[MyGuid]
           ,[Byte]
           ,[Char]
           ,[Amt]
           ,[Binary]
           ,[DateTimeOffset])
     VALUES
           (@Name
           ,@Age
           ,@IsActive
           ,@CreateAt
           ,@DoubleValue
           ,@MyGuid
           ,@Byte
           ,@Char
           ,@Amt
           ,@Binary
           ,@DateTimeOffset)

		select @NewId = @@Identity;

		return 8;
END
GO


