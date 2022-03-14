using MongoHead;

namespace MongoHeadSample.Models;

public class Person: BaseEntityLight
{
    public Person()
    { 
        FirstName = String.Empty;
        LastName = String.Empty;
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public int Age { get; set; }
}

