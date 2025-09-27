using System;
using System.Collections.Generic;
using System.Text;

namespace X10Card.Models.NewUserRegistration
{
    public class LanguagesSaved
    {
        public string? LanguageID { get; set; }
        public string? LanguageName { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Speak { get; set; }
    }
}
