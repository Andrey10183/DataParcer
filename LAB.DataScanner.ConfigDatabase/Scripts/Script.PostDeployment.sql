/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/* ApplicationType table initialization with sample data*/
SET IDENTITY_INSERT meta.ApplicationType ON; 

MERGE INTO meta.ApplicationType AS tgt  
USING (VALUES 
    (1, 'downloader', 'ver. 1', 'dwnl jsonFile'),
	(2, 'generator', 'ver. 1', 'gen jsonFile'),
	(3, 'validator', 'ver. 1', 'valid jsonFile'))  
       AS src (NewTypeID, NewTypeName, NewTypeVersion, NewConfigTemplateJson)  
ON tgt.TypeID = src.NewTypeID  
WHEN MATCHED THEN  
UPDATE SET
    TypeName = src.NewTypeName,
    TypeVersion = src.NewTypeVersion,
    ConfigTemplateJson = src.NewConfigTemplateJson

WHEN NOT MATCHED BY TARGET THEN  
INSERT (TypeID, TypeName, TypeVersion, ConfigTemplateJson) 
VALUES (NewTypeID, NewTypeName, NewTypeVersion, NewConfigTemplateJson)  

OUTPUT $action, 
    inserted.TypeID,
    inserted.TypeName,
    inserted.TypeVersion,
    inserted.ConfigTemplateJson;

SET IDENTITY_INSERT meta.ApplicationType OFF;

/* ApplicationInstance table initialization with sample data*/
SET IDENTITY_INSERT component.ApplicationInstance ON;

MERGE INTO [component].[ApplicationInstance] AS tgt  
USING (VALUES 
    (1, 1, 'app1Instance1', 'app1_1 jsonFile'),
	(2, 2, 'app1Instance2', 'app1_2 jsonFile'),
	(3, 3, 'app2Instance1', 'app2_1 jsonFile'))  
       AS src (NewInstanceID, NewTypeID, NewInstanceName, NewConfigJson)  
ON tgt.InstanceID = src.NewInstanceID  
WHEN MATCHED THEN  
UPDATE SET
    TypeID = src.NewTypeID,
    InstanceName = src.NewInstanceName,
    ConfigJson = src.NewConfigJson

WHEN NOT MATCHED BY TARGET THEN  
INSERT (InstanceID, TypeID, InstanceName, ConfigJson) 
VALUES (NewInstanceID, NewTypeID, NewInstanceName, NewConfigJson)  

OUTPUT $action, 
    inserted.InstanceID,
    inserted.TypeID,
    inserted.InstanceName,
    inserted.ConfigJson;

SET IDENTITY_INSERT component.ApplicationInstance OFF;

/* Binding table initialization with sample data*/
MERGE INTO [binding].[Binding] AS tgt  
USING (VALUES 
    (1, 3),
	(2, 3))  
        AS src (NewPublisherInstanceID, NewConsumerInstanceID)  
ON (tgt.PublisherInstanceID = src.NewPublisherInstanceID AND
    tgt.ConsumerInstanceID = src.NewConsumerInstanceID)

WHEN NOT MATCHED BY TARGET THEN  
INSERT (PublisherInstanceID, ConsumerInstanceID) 
VALUES (NewPublisherInstanceID, NewConsumerInstanceID)  

OUTPUT $action, 
    inserted.PublisherInstanceID,
    inserted.ConsumerInstanceID;