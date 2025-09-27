using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class SubmittedFormsDetails
    {
        public string? RegistrationNo { get; set; }
        public string? ActiveForm { get; set; }
        public string? Stat { get; set; }
        
        public string? ContactDetails { get; set; }
        public string? EducationDetails { get; set; }
        public string? EmployedDetails { get; set; }
        public string? ExDetails { get; set; }
        public string? NCODetails { get; set; }
        public string? MiscDetails { get; set; }
        public string? PH { get; set; }
        public string? PersonalDetails { get; set; }       
        public string? SubCat { get; set; }

        public string? PersonalDetailsYN { get; set; }
        public string? ContactDetailsYN { get; set; }
      
    }
}
