using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreDemos.SitecoreLayer.Templates
{
    public static class EventTemplate
    {
        public static ID ID = new ID("");

        public static class Fields
        {
            public static string DisplayName = "__displayname";

            public static string Text = "Text";

            public static string Location = "Location";

            public static string DateAndTime = "Date and time";
        }
    }
}
