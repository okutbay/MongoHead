# MongoHead Version Info
## 0.1.0
- Initial version
- Migrating existing methods to this project
- New helper methods implemented
- Base data methods added to use helper methods. BaseData class implements IBaseData
- BaseEntity class added to shape entity base properties
- BaseEntity dates are converted to UTC, and UTC offset is added as a property. Also BaseData and MongoDBHelper class is updated to reflect this change
- MongoDBHelper methods refactoring and bug fixes
- MongoDBHelper CollectionName method parameter is moved as a class property.
- BaseData refactoring according to MongoDBHelper changes
- BaseData MongoDBHelper instance moved a property and created in the constructor
- MongoDBHelper constructor parameter CollectionName is changed to EntityType to reflect CollectionName from the Entity Type's name. BaseData helper instace creation is changed according to this.
- MongoDBConfig class change. KeyName properties added to access config settings
- BaseData changed. Config Key Names changed from string to static members
- MongoDBHelper changed. Save methods refactored
- MongoDBHelper class changed. _id field name property changed to static and other field name properties move to BaseData
- BaseData class changed. Field name properties moved from MongoDBHelper to BaseData and code is modified according to this change
- MongoDBHelper bug fixed. GetList<T>(List<Filter> Filter) method was calling itself by a forgotton parameter.
- [BsonIgnoreExtraElements] attribute added to Test entity to prevent Format exception.
- MongoDBHelper GetByObjectId method is refactored
- MongoDBHelper GetLast method is refactored
- MongoDBHelper GetByObjectId method bugfix
- BaseData, Filter, MongoDBHelper naming refactorings
- IBaseData method signatures refactored
- BaseData GetKeyValueList method signatures refactored and valueFieldName variable is changed to $"{CollectionName}Value". With this change our method is going to get values from the XValue column.
- BaseData GetKeyValueList method modified to use Named string interpolation instead of String.Format
- MongoDBHelper Save method is removed and 2 new methods added for Insert and Replace operations.
- BaseData Save method is modified to reflect changes in MongoDBHelper
- BaseEntity simplified to use with log objects. BaseData Save method is modified to check unexisted properties.
## 0.2.0
- "BaseEntitySimple" simplified use just with _id to use when no-date is necessary. To achieve this "_DateUtcCreated" property is moved to another base class called "BaseEntityLight" which inherits "BaseEntitySimple". Now "BaseEntity" inherits "BaseEntityLight"
- "BaseEntityComplex" class is added to provide UserId support. Latest base entity hierarchy is: BaseEntitySimple < BaseEntityLight < BaseEntity < BaseEntityComplex
## 0.3.0
- Added New GetList method to MongoDBHelper class to return a list filtered by an ObjectId in any provided field 
## 0.4.0
- Added CollectionExists and CollectionExistsAsync methods to MongoDBHelper class 
- Added DeleteByFieldValue method to MongoDBHelper class 
## 0.5.0
- Upgraded target fremwork to .NET 6.0
- Upgraded Microsoft.Extensions.Configuration to 6.0.0
- Upgraded Microsoft.Extensions.Options to 6.0.0
- Upgraded MongoDB.Driver to 2.14.1
- BUGFIX: _UserId property type is changed from int to string to support ObjectId values
- Config change: "MongoDBConfig" renamed to "MongoDB" and moved under settings configuration section. Also "MongoDBConfig" class is obsolete and we have a new config class name "Config"
- "MongoDBHelper" type is now generic: "MongoDBHelper<T>"
## 0.5.1
- Added CreateIndexExpireAfterDuration function for BaseData class. With this function we may create TTL indexes for date fields like "_DateUtcCreated" or "_DateUtcModified"
## 0.5.2
- "MongoHead.Info.txt" file renamed to and format changed to markdown. -> "MongoHead.VersionInfo.md".
- "RunCommand" function to "MongoDBHelper". And "CreateIndexExpireAfterDuration" function modified accordingly.
- New constructor option for "MongoHead.Config" class with ConnectionString and DatabaseName
- New constructor option for "MongoHead.MongoDBHelper" class with ConnectionString and DatabaseName and 
- "MongoHead.MongoDBHelper" refactored.
## 0.5.3
- Added new construtor for BaseData class to support operations over different databases than config.
- Modified post built script tp point output folder to C drive again. Also added folder for keeping versions
- Upgrade "MongoDB.Driver" package version "2.15.0" to "2.23.1".