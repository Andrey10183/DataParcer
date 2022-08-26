CREATE TABLE [binding].[Binding]
(
	[PublisherInstanceID] INT NOT NULL, 
    [ConsumerInstanceID] INT NOT NULL,
	PRIMARY KEY([PublisherInstanceID],[ConsumerInstanceID]),
	
	CONSTRAINT FK_Binding_ApplicationInstance_Publisher FOREIGN KEY ([PublisherInstanceID])
	REFERENCES [component].[ApplicationInstance](InstanceID),

	CONSTRAINT FK_Binding_ApplicationInstance1_Consumer FOREIGN KEY ([ConsumerInstanceID])
	REFERENCES [component].[ApplicationInstance](InstanceID)
)
