using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;

namespace DBC.Search.Umbraco
{
    public class UmbracoSearchApi
    {
        public static List<Blogpost> GetBlogposts(IPublishedContent model, string query)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var roots = umbracoHelper.TypedContentAtRoot();
            var blogposts = roots.SelectMany(x => x.Descendants<Blogpost>());
            //var blogposts = model.Descendants<Blogpost>();

            var terms = query.ToLower().Split(' ');

            foreach (var term in terms)
            {
                blogposts = blogposts.Where(x => x.Excerpt.ToLower().Contains(term));
            }
            
            return blogposts.ToList();
        }
    }
}