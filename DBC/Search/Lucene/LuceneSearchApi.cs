using System.Collections.Generic;
using DBC.Models;

namespace DBC.Search.Lucene
{
    public class LuceneSearchApi
    {
        public static List<BlogpostDataModel> GetBlogposts(string query = "")
        {
            return Searcher.RawQuery<BlogpostDataModel>("Excerpt:*" + (string.IsNullOrEmpty(query) ? "" : query.ToLower() + "*"));
        }
    }
}