# MongoHead
Provides the layer for MongoDB operations over .NET driver 2.15.0. Also MongoHead includes a BaseData and BaseEntity class to speed up your application development process.
MongoHead is a .NET 6.0 Library

## Installation
Available on NuGet:
https://www.nuget.org/packages/MongoHead/

## Dependencies
Here is the list of installed NuGet packages and versions
1. MongoDB.Driver (v2.15.0)
1. Microsoft.Extensions.Options (v6.0.0)
1. Microsoft.Extensions.Configuration (v6.0.1)

## Classes
### MongoDBHelper
Helper for mongodb operations. Provides CRUD operations.
Needs MongoDBConfig instance to configure connection to Db and and a Type to operate with it. Type names are equal to the collection names in MongoHead ecosystem.

:loudspeaker: **We assume that your entity (type) has inherited BaseEntity and these some properties by defalt. Please check [BaseEntity class](../blob/master/README.mdbaseentity-classes-baseentitysimple-baseentitylight-baseentity-baseentitycomplex)**

### MongoDBConfig
Simple class used to transfer database connection parameters to our MongoDBhelper class
These parameters are "ConnectionString" and "DatabaseName"

### Filter and ExpressionBuilder
Filter used in MongoDBHelper and BaseData class to define query parameters and ExpressionBuilder is used as a helper class to convert filter object to linq expressions

### BaseData
An implementation of IBaseData and aims to provide base CRUD methods to your data layer classes

### BaseEntity Classes (BaseEntitySimple, BaseEntityLight, BaseEntity, BaseEntityComplex)
Each class adds some fields to entity provide to support to eliminating unnecessary properties on your entities and keeps your database collections lighter.
Latest base entity hierarchy (inheritence) is: BaseEntitySimple < BaseEntityLight < BaseEntity < BaseEntityComplex

#### BaseEntitySimple adds this property
* _id

#### BaseEntityLight adds this property
* _DateUtcCreated

#### BaseEntity  adds these properties
* _DateUtcModified
* _IsActive

#### BaseEntityComplex adds this property
* _UserId


# Sample Application
## MongoHead Configuration
Add Mongo DB settings to you appsettings.json setting file

Sample JSON settings:
```JSON
{
  "Settings": {
    "MongoDB": {
      "ConnectionString": "mongodb://localhost",
      "DatabaseName": "MongoHeadSample"
    } //MongoDB
  } //Settings
}
```

:exclamation::exclamation::exclamation: **Information given after this line is draft at the moment** :exclamation::exclamation::exclamation:

## Sample Application Organization
Our sample application is an ASP.NET Razor Pages application for .NET 6
Of course there may be lots of different achitectures. We are going to keep it simple and follow n-layered architecture. Different samples using different approaches are appreciated. :)

### Entities
You need entities to interact with mongodb collection. If you have an entity named X you are going a have collection with name X in your database.
Your entity classes may inherit our "BaseEntity" class provided by MongoHead. With this inheritence all your class will have same base fiels in your database.
To show a simple entity implementation, our sample application contains an entity named "Test". 
Sample entity may be found in "/Model" folder of the sample app.

### Data Layers
Sample data layer may be found in "/Data" folder of the sample app.

### Business Layers
Sample business layer may be found in "/Business" folder of the sample app.

### ViewModels 
Sample viewmodel may be found in "/ViewModels" folder of the sample app.

### Pages/People
Wraps all elements with two pages: a list page (/pages/people/index) and a form page (/pages/people/form).
List page has search and delete samples.
Form page has add/update samples.

# MongoDB
Setup and installation details can be obtained from official web site. 

## MongoDB Community Server Download
For setup please visit https://www.mongodb.com/download-center?jmp=nav#community

## MongoDB Community Server Installation steps for Windows
https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows/

# AutoMapper
In our sample projects we prefer to use automapper to map our objects between presentation and data layer
For more informtation: https://automapper.org/

## References
* [Download MongoDB](https://www.mongodb.com/download-center?jmp=nav#community)
* [Windows Installation](https://docs.mongodb.com/manual/tutorial/install-mongodb-on-windows/)
* [Automapper](https://automapper.org/)