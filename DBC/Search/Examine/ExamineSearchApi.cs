using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DBC.Models;
using Examine;
using Examine.SearchCriteria;
using UmbracoExamine;

namespace DBC.Search.Examine
{
    public class ExamineSearchApi
    {
        public static List<BlogpostDataModel> GetBlogposts(string query)
        {
            var searcher = ExamineManager.Instance.SearchProviderCollection["BlogSearcher"];
            var searchCriteria = searcher.CreateSearchCriteria(IndexTypes.Content);
            IBooleanOperation searchQuery;
            if (!string.IsNullOrEmpty(query))
            {
                var terms = query.Split(' ');
                searchQuery = searchCriteria.Field("excerpt", terms[0]);
                foreach (var term in terms)
                {
                    if (term == terms[0]) continue;
                    searchCriteria.Field("excerpt", term.ToLower());
                }
            }
            else
            {
                searchQuery = searchCriteria.Range("createDate", DateTime.MinValue, DateTime.MaxValue);
            }

            var searchResults = searcher.Search(searchQuery.Compile());
            var blogposts = searchResults.Select(x => new BlogpostDataModel
            {
                Id = x.Fields["id"],
                Url = x.Fields["url"],
                Excerpt = x.Fields["excerpt"],
                Name = x.Fields["nodeName"],
                Categories = x.Fields["categories"]?.Split(' ').ToList(),
                CreateDate = DateTime.ParseExact(x.Fields["createDate"], "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)
            });

            return blogposts.ToList();
        }
    }
}