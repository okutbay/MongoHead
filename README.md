# MongoHead
Provides the layer for MongoDB operations over .NET driver 2.7.0. Also MongoHead includes a BaseData and BaseEntity class to speed up your application development process

## Dependencies
Here is the list of installed NuGet packages and versions
* MongoDB.Driver (v2.7.0)
* Microsoft.NETCore.App (v2.0.0)
* Microsoft.Extensions.Options (v2.1.1)
* Microsoft.Extensions.Configuration (v2.1.1)

## Classes
### MongoDBHelper
Helper for mongodb operations. Provides CRUD operations.
Needs MongoDBConfig instance to configure connection to Db and and a Type to operate with it. Type names are equal to the collection names in MongoHead ecosystem.

**We assume that your entity (type) has inherited BaseEntity and these some properties by defalt. Please check BaseEntity class**

### MongoDBConfig
Simple class used to transfer database connection parameters to our MongoDBhelper class
These parameters are "ConnectionString" and "DatabaseName"

### Filter and ExpressionBuilder
Filter used in MongoDBHelper and BaseData class to define query parameters and ExpressionBuilder is used as a helper class to convert filter object to linq expressions

### BaseData
An implementation of IBaseData and aims to provide base CRUD methods to your data layer classes

### BaseEntity
Bases a foundation for your entities by providing base properties like id, date and basic status fields.
* _id
* _DateUtcCreated
* _DateUtcModified
* _IsActive

# Sample Application
## MongoHead Configuration
Add Mongo DB settings to you appsettings.json setting file

```
Sample JSON settings
  "MongoDBConfig": {
    "ConnectionString": "mongodb://localhost",
    "DatabaseName": "MongoHeadDB"
  }
```

**Information given after this line is draft at the moment**

## Sample Application Organization
Of course there may be lots of different achitectures. We are going to keep it simple and follow n-layered architecture. Different samples using different aproches are appreciated. :)

### Entities
You need entities to interact with mongodb collection. If you have an entity named X you are going a have same named collection in your datebase.
Youe entity classes may inherit our "BaseEntity" class provided by MongoHead. With this inheritence all your class will have same base fiels in your database.
To show a simple entity implementation, our sample application contains an entity named "Test". 

### Data Layers

### Business Layers

**PS: To keep things in one place we've placed entity, data layer and business layer**

# MongoDB
Setup and installation details can be obtained from official web site. 

## MongoDB Community Server Download
For setup please visit https://www.mongodb.com/download-center?jmp=nav#community

## MongoDB Community Server Installation steps for Windows
https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows/


# AutoMapper
```
PM> Install-Package AutoMapper
```

When I'm trying to configure a had difficulty to find 'AddAutoMapper' extension method. To have it install this package too
```
PM> install-package AutoMapper.Extensions.Microsoft.DependencyInjection
```

If you don't install and try to use method you will have this error.
```
'IServiceCollection' does not contain a definition for 'AddAutoMapper' and no extension method 'AddAutoMapper' accepting a first argument of type 'IServiceCollection' could be found (are you missing a using directive or an assembly reference?
```
