/****** Object:  Table [dbo].[Category]    Script Date: 01/06/2023 14:16:19 ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Category]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Category];
GO

CREATE TABLE [dbo].[Category]
    (
      [CategoryId] [INT] PRIMARY KEY
                         IDENTITY(1, 1) ,
      [CategoryName] [VARCHAR](50) NULL
    );

/****** Object:  Table [dbo].[Product]    Script Date: 01/06/2023 14:17:32 ******/
IF EXISTS ( SELECT  *
            FROM    sys.objects
            WHERE   object_id = OBJECT_ID(N'[dbo].[Product]')
                    AND type IN ( N'U' ) )
    DROP TABLE [dbo].[Product];
GO

CREATE TABLE [dbo].[Product]
    (
      [ProductId] [INT] PRIMARY KEY
                        IDENTITY(1, 1) ,
      [CategoryId] [INT] NULL ,
      [ProductCode] [VARCHAR](50) NULL ,
      [ProductName] [VARCHAR](100) NULL ,
      [CreateTime] [DATETIME] NULL ,
      [ModifyTime] [DATETIME] NULL ,
      [DeleteMark] [INT] NULL ,
      [Custom1] [VARCHAR](50) NULL ,
      [Custom2] [VARCHAR](50) NULL ,
      [Custom3] [VARCHAR](50) NULL ,
      [Custom4] [VARCHAR](50) NULL ,
      [Custom5] [VARCHAR](50) NULL ,
      [Custom6] [VARCHAR](50) NULL ,
      [Custom7] [VARCHAR](50) NULL ,
      [Custom8] [VARCHAR](50) NULL ,
      [Custom9] [VARCHAR](50) NULL ,
      [Custom10] [VARCHAR](50) NULL ,
      [Custom11] [VARCHAR](50) NULL ,
      [Custom12] [VARCHAR](50) NULL
    );
