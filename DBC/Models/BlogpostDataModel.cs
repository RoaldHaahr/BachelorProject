using System;
using System.Collections.Generic;

namespace DBC.Models
{
    public class BlogpostDataModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public List<string> Categories { get; set; }

        public string Url { get; set; }

        public string Excerpt { get; set; }
    }
}