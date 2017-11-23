using System.Collections.Generic;
using DBC.Models;

namespace DBC.Search.Lucene
{
    public class LuceneSearchApi
    {
        public static List<BlogpostDataModel> GetBlogposts()
        {
            return Searcher.RawQuery<BlogpostDataModel>("*:*");
        }
    }
}