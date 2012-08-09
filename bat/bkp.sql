DECLARE @filedate VARCHAR(20)
SET @filedate=REPLACE(REPLACE(CONVERT(VARCHAR(20),GETDATE(),20),':','.'),' ','_')

DECLARE @file_path VARCHAR(256)
SET @file_path='e:\Projects\OrdersBKP\VotGESOrders_'+@filedate+'.bak'

BACKUP DATABASE [VotGESOrders] TO  DISK = @file_path WITH NOFORMAT, INIT,  NAME = N'VotGESOrders', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
GO