USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_LineBooking]    Script Date: 16-04-2021 14:11:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_LineBooking](
	[LineBookingID] [bigint] NOT NULL,
	[Module] [nvarchar](255) NOT NULL,
	[Line] [nvarchar](255) NOT NULL,
	[Style] [nvarchar](255) NOT NULL,
	[SONo] [nvarchar](255) NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[Quantity] [int] NOT NULL,
	[SMV] [float] NOT NULL,
	[PlannedEffeciency] [float] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[UserID] [int] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_LineBooking] PRIMARY KEY CLUSTERED 
(
	[LineBookingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

