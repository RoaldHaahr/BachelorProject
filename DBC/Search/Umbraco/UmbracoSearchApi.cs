using System.Collections.Generic;
using System.Linq;
using DBC.Models;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;

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
    }
}