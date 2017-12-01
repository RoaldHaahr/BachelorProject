using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DBC.Models;
using Examine;
using Examine.SearchCriteria;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;
using UmbracoExamine;

namespace DBC.Search.Umbraco
{
    public class UmbracoSearchApi
    {
        // https://our.umbraco.org/forum/core/general/63905-Scalability-and-Performance
        public static List<BlogpostDataModel> GetBlogpostsWithUmbracoHelper(string query)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var roots = umbracoHelper.TypedContentAtRoot();
            var blogposts = roots.SelectMany(x => x.Descendants<Blogpost>());

            var terms = query.ToLower().Split(' ');

            foreach (var term in terms)
            {
                blogposts = blogposts.Where(x => x.Excerpt.ToLower().Contains(term));
            }
            
            return blogposts.Select(x => new BlogpostDataModel
            {
                Categories = x.Categories.ToList(),
                CreateDate = x.CreateDate,
                Excerpt = x.Excerpt,
                Url = x.Url,
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToList();
        }

        public static List<BlogpostDataModel> GetBlogpostsWithContentService(IPublishedContent model, string query)
        {
            var blogposts = model.AncestorOrSelf<Home>().Descendants<Blogpost>();
            var terms = query.Split(' ');
            blogposts = terms.Aggregate(blogposts, (current, term) => current.Where(x => x.Excerpt.ToLower().Contains(term)));
            return blogposts.Select(x => new BlogpostDataModel
            {
                Categories = x.Categories.ToList(),
                CreateDate = x.CreateDate,
                Excerpt = x.Excerpt,
                Url = x.Url,
                Id = x.Id.ToString(),
                Name = x.Name
            }).ToList(); ;
        }

        public static List<BlogpostDataModel> GetBlogpostsWithExamine(string query)
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