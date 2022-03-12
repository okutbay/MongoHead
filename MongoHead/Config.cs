using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoHead;
public class Config
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Key name for JSON Settings file
    /// </summary>
    private const string keyNameConnectionString = "Settings:MongoDB:ConnectionString";

    /// <summary>
    /// Key name for JSON Settings file
    /// </summary>
    private const string keyNameDatabaseName = "Settings:MongoDB:DatabaseName";

    public Config(IConfiguration configuration)
    {
        _configuration = configuration;

        ConnectionString = _configuration[keyNameConnectionString];
        DatabaseName = _configuration[keyNameDatabaseName];
    }

    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}


/// <summary>
/// Used to store parameter values to connect and operate on MongoDB
/// This class also contains KeyName static properties to access values of the settings file
/// </summary>
[Obsolete("This class is obsolete. Use Config class instead")]
public class MongoDBConfig
{
    /// <summary>
    /// Key name for JSON Settings file
    /// </summary>
    public static string KeyNameConnectionString = "MongoDBConfig:ConnectionString";

    /// <summary>
    /// Key name for JSON Settings file
    /// </summary>
    public static string KeyNameDatabaseName = "MongoDBConfig:DatabaseName";

    /// <summary>
    /// This parameter must contain MongoDB connection string
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// This is the database name to work with
    /// </summary>
    public string DatabaseName { get; set; }

    /// <summary>
    /// Default construtor with two required parameteters
    /// </summary>
    /// <param name="ConnectionString">Check class definition for details</param>
    /// <param name="DatabaseName">Check class definition for details</param>
    public MongoDBConfig(string ConnectionString, string DatabaseName)
    {
        this.ConnectionString = ConnectionString;
        this.DatabaseName = DatabaseName;
    }
}