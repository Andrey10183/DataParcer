# Introduction 
DataScanner solution contains the following projects:
1. ConfigDatabase for deploying SQL database;
2. ConfigDatabaseApi API for services management;
3. UrlsGenerator for generating pack of URLs and sending them via rabbitMQ to next microservice;
4. WebPageDownloader for accepting URLs and downloading corresponding Html pages, then send to the next microservice;
5. HtmlToJsonConverter accepts Html files and convert them to JSON, then send to the next microservice;
6. SimpleDbPersister accepts JSON files and pushes them into a database.

# Getting Started
To check the functionality of this set of applications you have to install the following software:
1.	RabbitMQ
2.	SQL Server
3.	VisualStudio

# Configuration
To setup ConfigDatabaseApi you need to configure the connection string in an appsetting file
example:
"ConnectionStrings": {
    "DefaultConnection": "Server=HOMEPC\\ANDREYPC;Database=LAB.DataScanner.ConfigDatabase;Trusted_Connection=True"
  }
  
To setup SimpleDatabasePersister you need to configure connection string in a appsetting file
example:
"SqlConnectionString": "Server=HOMEPC\\ANDREYPC;Database=PersisterDatabase;Trusted_Connection=True"
  
for all services you might need to configure rabbitMQ settings
example of default settings:
"RmqConfig": {
    "UserName": "guest",
    "Password": "guest",
    "HostName": "localhost",
    "Port": 5672,
    "VirtualHost": "/"
  }
  
For UrlsGenerator you need to specify in appsetting file URL template and sequences.
example:
"Application": {
    "UrlTemplate": "https://www.citilink.ru/catalog/noutbuki/?p={0}",
    "Sequences": [ "3..6" ]
  },
  
for all services it is nesessary to specify binding sections to define exchange and routing keys
example:
"Binding": {
    "ReceiverQueue": "WebQueue",
    "ReceiverExchange": "GeneratorExchange",
    "ReceiverRoutingKeys": [ "#" ],

    "SenderExchange": "WebExchange",
    "SenderRoutingKeys": [ "#" ]
  }

#Note
In SimpleDatabasePersister I didn't manage to completely perform a task. I can't create DbSet dynamically, so my solution is a hardcoded Db model, appsettins don't affect execution.

#start
run ServicesListeners.bat
run ServiceGenerator.bat
On the console log, you can track operation progress. After all, tasks are completed in a target table you
can see products with prices and product codes.