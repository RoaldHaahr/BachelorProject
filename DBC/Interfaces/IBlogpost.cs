using System;
using System.Collections.Generic;

namespace DBC.Interfaces
{
    public interface IBlogpost
    {
        string Id { get; set; }

        string Name { get; set; }

        DateTime CreateDate { get; set; }

        List<string> Categories { get; set; }

        string Url { get; set; }

        string Excerpt { get; set; }
    }
}
