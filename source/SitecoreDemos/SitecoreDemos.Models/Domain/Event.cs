using System;

namespace SitecoreDemos.Models.Domain
{
    public class Event : PublicationBase
    {
        public string Location { get; set; }

        public DateTime DateAndTime { get; set; }
    }
}
