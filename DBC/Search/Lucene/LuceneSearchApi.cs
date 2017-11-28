using System.Collections.Generic;
using DBC.Models;

namespace DBC.Search.Lucene
{
    public class LuceneSearchApi
    {
        public static List<BlogpostDataModel> GetBlogposts(string query = "")
        {
            var terms = query.Split(' ');
            var searchQuery = "+Excerpt:*" + string.Join("* +Excerpt:*", terms) + "*";
            return LuceneSearcher.RawQuery<BlogpostDataModel>(searchQuery);
        }
    }
}