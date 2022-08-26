CREATE TABLE [component].[ApplicationInstance]
(
	[InstanceID] INT IDENTITY(1,1) NOT NULL, 
    [TypeID] INT NOT NULL, 
    [InstanceName] NVARCHAR(50) NOT NULL, 
    [ConfigJson] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT PK_ApplicationInstance_InstanceID PRIMARY KEY CLUSTERED (InstanceID),
    CONSTRAINT FK_ApplicationInstance_ApplicationType FOREIGN KEY (TypeID)
    REFERENCES [meta].[ApplicationType](TypeID)
)
