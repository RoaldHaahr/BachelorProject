using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBC.Interfaces
{
    public interface ISearchApi
    {
        List<IBlogpost> GetBlogposts { get; set; }
    }
}
