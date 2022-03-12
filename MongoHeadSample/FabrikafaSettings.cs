using System;
using System.Text.Json.Serialization;

namespace MongoHeadSample;

public class FabrikafaSettings
{
    public FabrikafaSettings()
    {
        Logging = new Logging();
        AllowedHosts = String.Empty;
        Settings = new Settings();
    }

    public Logging Logging { get; set; }

    public string AllowedHosts { get; set; }

    public Settings Settings { get; set; }
}


public class Logging
{
    public Logging()
    {
        LogLevel = new LogLevel();
    }

    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    public LogLevel()
    {
        Default = String.Empty;
        Microsoft_AspNetCore = String.Empty;
    }

    public string Default { get; set; }
    
    //TODO: we cannot find a way to bind this property with the names have dot in it. We need to assign this value manually
    [JsonPropertyName("Microsoft.AspNetCore")]
    public string Microsoft_AspNetCore { get; set; }
}

public class Settings
{
    public Settings()
    {
        MongoDB = new MongoDB();
    }

    public MongoDB MongoDB { get; set; }

}

public class MongoDB
{
    public MongoDB()
    {
        ConnectionString = String.Empty;
        DatabaseName = String.Empty;
    }

    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}