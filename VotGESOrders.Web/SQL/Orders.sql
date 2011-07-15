USE [VotGESOrders]
GO


drop table orders;
drop table orderObjects;
drop table users;


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Users](
	[userID] [int] identity(1,1) NOT NULL, 
	[name] [varchar](50) NOT NULL, /*имя пользователя (CORP\xxx, SR-VOTGES-PI\xxx)*/
	[fullName] [varchar](250) NOT NULL,/*полное имя пользователя*/
	[mail] [varchar](250) NOT NULL,/*полное имя пользователя*/
	[sendAllMail] [bit] NOT NULL,
	[sendAgreeMail] [bit] NOT NULL,
	[sendCreateMail] [bit] NOT NULL,
	[allowCreateOrder] [bit] NOT NULL, /*пользователь может создавать заявки*/
	[allowCreateCrashOrder] [bit] NOT NULL, /*пользователь может создавать аварийные заявки*/
	[allowReviewOrder] [bit] NOT NULL,/*пользователь может разрешить/отклонить заявку*/
	[allowChangeOrder] [bit] NOT NULL,/*пользователь может изменить заявку*/
	[allowEditTree] [bit] NOT NULL,/*пользователь может редактировать дерево оборудования*/
	[allowEditUsers][bit] NOT NULL,/*пользователь может редактировать дерево список пользователей*/
	[allowAgreeOrders][bit] NOT NULL,/*пользователь может согласовывать заявку*/
	
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[userID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
				


CREATE TABLE [dbo].[OrderObjects](
	[objectID] [int] identity(1,1) NOT NULL, /*код оборудования*/
	[parentID] [int] not NULL,/* код родительского оборудования*/
	[objectName] [varchar](250) NOT NULL, /*наименование оборудования*/
	
 CONSTRAINT [PK_OrderObjects] PRIMARY KEY CLUSTERED 
(
	[objectID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO





USE [VotGESOrders]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Orders](
	
	[parentOrderNumber] [int] NULL, /*ID родительской заявки (эта заявка - продление)*/
	[childOrderNumber] [int] NULL, /*ID дочерней заявки (эта заявка продлена)*/
	
	[orderNumber] [int] identity(1,1) NOT NULL, /*Номер заявки (автоинкремент)*/
	
	[userCreateOrderID] [int] NOT NULL, /*Имя пользователя, создавшего заявку*/
	[userReviewOrderID] [int] NULL, /*Имя пользователя, разрешившего заявку*/
	[userCloseOrderID] [int] NULL, /*Имя пользователя, закрывшего заявку (разрешение ввода оборудования)*/
	[userCompleteOrderID] [int] NULL, /*Имя пользователя, который ввел оборудование в работу (полное завершение заявки)*/
	[userOpenOrderID] [int] NULL, /*Имя пользователя, открывшего заявку*/
	[userCancelOrderID] [int] NULL, /*Имя пользователя, снявшего заявку*/
	
	[orderDateCreate] [datetime2](7) NOT NULL, /*Дата создания заявки - устанавливается временем сервера в момент создания*/
	[orderDateReview] [datetime2](7) NULL, /*Дата разрешения заявки - устанавливается временем сервера*/
	[orderDateClose] [datetime2](7) NULL, /*Дата закрытия заявки (разрешения ввода в работу) - устанавливается временем сервера*/
	[orderDateOpen] [datetime2](7) NULL, /*Дата открытия заявки - устанавливается временем сервера*/
	[orderDateComplete] [datetime2] (7)NULL, /*Дата ввода оборудования в работу (полное завершение работы) - устанавливается временем сервера*/
	[orderDateCancel] [datetime2] (7)NULL, /*Дата снятия заявки - устанавливается временем сервера*/
	[orderLastUpdate] [datetime2] (7) NOT NULL, /*Дата последнего изменения заявки - устанавливается временем сервера*/
	
	[reviewText] [varchar] (250) NULL, /*Комментарий при разрешении заявки*/
	[openText] [varchar] (250) NULL, /*Комментарий при открытии заявки*/
	[closeText] [varchar] (250) NULL, /*Комментарий при закрытии (разрешении ввода) заявки*/
	[completeText] [varchar] (250) NULL, /*Комментарий при вводе в работу (полном завершении заявки*/
	[cancelText] [varchar] (250) NULL, /*Комментарий при снятии заявки*/
	[createText] [varchar] (250) NULL, /*Комментарий при создании заявки*/
	
	[planStartDate] [datetime2](7) NOT NULL, /*Плановая дата начала работ (устанавливается пользователем)*/
	[planStopDate] [datetime2](7) NOT NULL,/*Плановая дата окончания работ (устанавливается пользователем)*/
	[faktStartDate] [datetime2](7) NULL, /*фактическая дата начала работ (устанавливается пользователем)*/
	[faktStopDate] [datetime2](7) NULL, /*фактическая дата окончания работ (разрешение ввода в работ) (устанавливается пользователем)*/
	[faktCompleteDate] [datetime2](7) NULL, /*фактическая дата ввода оборудования в работу (устанавливается пользователем)*/
	
	[orderText] [varchar](250) NOT NULL, /*текст заявки*/
	[orderType] [varchar](5) NOT NULL, /*тип заявки (ПЛ,НПЛ,АВ,НО)*/
	[agreeText] [varchar](250)NOT NULL,/*согласование*/
	[agreeUsersIDS] [varchar](250)NOT NULL,/*согласование*/
	[readyTime] [varchar](50) NOT NULL,
	[orderObjectID] [int] NOT NULL,/* код оборудования*/
	[orderObjectAddInfo] [varchar](100) NOT NULL,/* код оборудования*/
	
	[orderCreated] [bit] NOT NULL, /*заявка создана*/
	[orderReviewed] [bit] NOT  NULL,	/*заявка разрешена*/
	[orderOpened] [bit] NOT NULL, /*заявка открыта*/
	[orderClosed] [bit] NOT  NULL,/*заявка закрыта (разрешен ввод оборудования)*/
	[orderCanceled] [bit] NOT  NULL,/*заявка снята*/
	[orderCompleted] [bit] NOT NULL,/*оборудование введено в работу (заявка полностью завершена)*/
	[orderCompletedWithoutEnter] [bit] NOT NULL,/*оборудование введено в работу (заявка полностью завершена)*/
	[orderExtended] [bit] NOT NULL,/*заявка продлена*/
	[orderAskExtended] [bit] NOT NULL,/*подана заявка на продение текущей*/
	[orderIsExtend] [bit] NOT NULL,/*зявка является продлением*/
	[orderIsFixErrorEnter] [bit] NOT NULL,/*зявка является исправлением ошибки ввода*/
	
	[orderState] [varchar](50) NOT NULL /*состояние заявки - created, accepted, banned, opened, canceled, closed, completed, extended, askExtended*/
	
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[orderNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UsersCreate] FOREIGN KEY([userCreateOrderID])
REFERENCES [dbo].[Users] ([userID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UsersCreate]
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UsersReview] FOREIGN KEY([userReviewOrderID])
REFERENCES [dbo].[Users] ([userID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UsersReview]
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UsersClose] FOREIGN KEY([userCloseOrderID])
REFERENCES [dbo].[Users] ([userID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UsersClose]
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UsersOpen] FOREIGN KEY([userOpenOrderID])
REFERENCES [dbo].[Users] ([userID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UsersOpen]
GO


ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UsersComplete] FOREIGN KEY([userCompleteOrderID])
REFERENCES [dbo].[Users] ([userID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UsersComplete]
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_UsersCancel] FOREIGN KEY([userCancelOrderID])
REFERENCES [dbo].[Users] ([userID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_UsersCancel]
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_OrderObjects] FOREIGN KEY([orderObjectID])
REFERENCES [dbo].[OrderObjects] ([objectID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_OrderObjects]
GO

USE [VotGESOrders]
GO




				
insert into users values('CORP\chekunovamv','Чекунова М.В','chekunovamv@votges.rushydro.ru',1,1,1,1,1,1,1,1,1,1);
insert into users values('RJ128X-PC\rj128x','ДГЩУ','',1,1,1,1,1,1,1,1,1,1);
insert into users values('SR-VOTGES-PI\dgshu','ДГЩУ','',1,1,1,1,1,1,1,1,1,1);
insert into users values('SR-VOTGES-PI\nss','НСС','',1,1,1,1,1,1,1,1,1,1);
insert into users values('SR-VOTGES-PI\author1','Автор1','',1,1,1,1,0,0,0,0,0,1);
insert into users values('SR-VOTGES-PI\author2','Автор2','',1,1,1,1,0,0,0,0,0,1);
insert into users values('SR-VOTGES-PI\gi','ГИ','',0,0,0,0,0,1,0,0,0,0);
insert into users values('SR-VOTGES-PI\Administrator','Админ','',1,1,1,1,1,1,1,1,1,1);
insert into users values('','Зыков С.Л.','',1,1,1,1,0,0,0,0,0,1);
insert into users values('','Никонов А.А.','',1,1,1,1,0,0,0,0,0,1);
insert into users values('','Лазарев А.И.','',1,1,1,1,0,0,0,0,0,1);
insert into users values('','Сидоров В.Л.','',1,1,1,1,0,0,0,0,0,1);
insert into users values('','Турборемонт','',1,1,1,1,0,0,0,0,0,1);
insert into users values('','Гидроремонт','',1,1,1,1,0,0,0,0,0,1);

