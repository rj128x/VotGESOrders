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
	[sendAllMail] [bit] NOT NULL,/*Оповещать о всех событиях*/
	[sendAgreeMail] [bit] NOT NULL,/*Оповещать о создании согласованных с ним заявках*/
	[sendCreateMail] [bit] NOT NULL,/*Оповещать о всех операциях со своими*/
	[sendAllAgreeMail] [bit] NOT NULL,/*Оповещать о всех операциях с согласованными заявками*/
	[sendAllCreateMail] [bit] NOT NULL,/*Оповещать о всех созданных заявках*/
	[allowCreateOrder] [bit] NOT NULL, /*пользователь может создавать заявки*/
	[allowCreateCrashOrder] [bit] NOT NULL, /*пользователь может создавать аварийные заявки*/
	[allowReviewOrder] [bit] NOT NULL,/*пользователь может разрешить/отклонить заявку*/
	[allowChangeOrder] [bit] NOT NULL,/*пользователь может изменить заявку*/
	[allowEditTree] [bit] NOT NULL,/*пользователь может редактировать дерево оборудования*/
	[allowEditUsers][bit] NOT NULL,/*пользователь может редактировать дерево список пользователей*/
	[allowEditOrders][bit] NOT NULL,/*пользователь может редактировать дерево список пользователей*/
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
	[showInFullName] [bit] not NULL
	
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
	
	[parentOrderNumber] [float] NULL, /*ID родительской заявки (эта заявка - продление)*/
	[childOrderNumber] [float] NULL, /*ID дочерней заявки (эта заявка продлена)*/
	
	[orderNumber] [float] NOT NULL, /*Номер заявки (автоинкремент)*/
	
	[userCreateOrderID] [int] NOT NULL, /*Имя пользователя, создавшего заявку*/
	[userReviewOrderID] [int] NULL, /*Имя пользователя, разрешившего заявку*/
	[userCloseOrderID] [int] NULL, /*Имя пользователя, закрывшего заявку (разрешение ввода оборудования)*/
	[userCompleteOrderID] [int] NULL, /*Имя пользователя, который ввел оборудование в работу (полное завершение заявки)*/
	[userOpenOrderID] [int] NULL, /*Имя пользователя, открывшего заявку*/
	[userCancelOrderID] [int] NULL, /*Имя пользователя, снявшего заявку*/
	
	[orderDateCreate] [datetime] NOT NULL, /*Дата создания заявки - устанавливается временем сервера в момент создания*/
	[orderDateReview] [datetime] NULL, /*Дата разрешения заявки - устанавливается временем сервера*/
	[orderDateClose] [datetime] NULL, /*Дата закрытия заявки (разрешения ввода в работу) - устанавливается временем сервера*/
	[orderDateOpen] [datetime] NULL, /*Дата открытия заявки - устанавливается временем сервера*/
	[orderDateComplete] [datetime] NULL, /*Дата ввода оборудования в работу (полное завершение работы) - устанавливается временем сервера*/
	[orderDateCancel] [datetime] NULL, /*Дата снятия заявки - устанавливается временем сервера*/
	[orderLastUpdate] [datetime] NOT NULL, /*Дата последнего изменения заявки - устанавливается временем сервера*/
	
	[reviewText] [varchar] (250) NULL, /*Комментарий при разрешении заявки*/
	[openText] [varchar] (250) NULL, /*Комментарий при открытии заявки*/
	[closeText] [varchar] (250) NULL, /*Комментарий при закрытии (разрешении ввода) заявки*/
	[completeText] [varchar] (250) NULL, /*Комментарий при вводе в работу (полном завершении заявки*/
	[cancelText] [varchar] (250) NULL, /*Комментарий при снятии заявки*/
	[createText] [varchar] (250) NULL, /*Комментарий при создании заявки*/
	
	[planStartDate] [datetime] NOT NULL, /*Плановая дата начала работ (устанавливается пользователем)*/
	[planStopDate] [datetime] NOT NULL,/*Плановая дата окончания работ (устанавливается пользователем)*/
	[faktStartDate] [datetime] NULL, /*фактическая дата начала работ (устанавливается пользователем)*/
	[faktStopDate] [datetime] NULL, /*фактическая дата окончания работ (разрешение ввода в работ) (устанавливается пользователем)*/
	[faktCompleteDate] [datetime] NULL, /*фактическая дата ввода оборудования в работу (устанавливается пользователем)*/
	
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
	
	[orderState] [varchar](50) NOT NULL, /*состояние заявки - created, accepted, banned, opened, canceled, closed, completed, extended, askExtended*/

	[commentsText] [text] NULL, /*Текст комментариев к заявке*/
	[expiredReglamentHours] [float] NULL,/*Количество часов, на которые заявка подана не по регламенту*/
	[expiredOpenHours] [float] NULL,/*Количество часов на которое просрочено открытие заявки*/
	[expiredCloseHours] [float] NULL,/*Количество часов на которое просрочено разрешение на ввод заявки*/
	[expiredCompleteHours] [float] NULL,/*Количество часов на которое просрочено закрытие заявки*/

	
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



