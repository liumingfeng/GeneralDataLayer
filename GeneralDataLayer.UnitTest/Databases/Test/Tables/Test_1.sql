CREATE TABLE [dbo].[Test_1](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Age] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateAt] [datetime] NOT NULL,
	[DoubleValue] [float] NOT NULL,
	[MyGuid] [uniqueidentifier] NULL,
	[Byte] [smallint] NOT NULL,
	[Char] [varchar](10) NOT NULL,
	[Amt] [decimal](18, 2) NOT NULL,
	[Binary] [binary](50) NOT NULL,
	[DateTimeOffset] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_Test_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_Name]  DEFAULT (N'MyTest') FOR [Name]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_Age]  DEFAULT ((8)) FOR [Age]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_CreateAt]  DEFAULT (getdate()) FOR [CreateAt]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_DoubleValue]  DEFAULT ((5.8)) FOR [DoubleValue]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_Byte]  DEFAULT ((3)) FOR [Byte]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_Char]  DEFAULT ('abc') FOR [Char]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_Amt]  DEFAULT ((8.8)) FOR [Amt]
GO

ALTER TABLE [dbo].[Test_1] ADD  CONSTRAINT [DF_Test_1_Binary]  DEFAULT ((11111111.)) FOR [Binary]
GO


