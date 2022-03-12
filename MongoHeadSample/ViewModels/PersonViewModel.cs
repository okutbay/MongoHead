using System.ComponentModel.DataAnnotations;

namespace MongoHeadSample.ViewModels;

public class PersonViewModel
{
    public PersonViewModel()
    {
        _id = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Age = 0;
    }

    [Key]
    public string _id { get; set; }

    [Required(ErrorMessage = "First name field is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name field is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Age field is required.")]
    [Range(0, 150, ErrorMessage = "Age Value for {0} must be between {1} and {2}.")]
    [Display(Name = "Age Of The Person")]
    public int Age { get; set; }
}
