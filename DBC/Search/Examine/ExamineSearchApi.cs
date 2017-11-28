using System;
using System.Collections.Generic;
using System.Linq;
using DBC.Models;
using Examine;
using Umbraco.Web.PublishedContentModels;

namespace DBC.Search.Examine
{
    public class ExamineSearchApi
    {
        public List<BlogpostDataModel> GetBlogposts(string query)
        {
            var searcher = ExamineManager.Instance.SearchProviderCollection["ExternalIndexSet"];
            var searchCriteria = searcher.CreateSearchCriteria();
            var terms = query.Split(' ');

            var searchQuery = searchCriteria.Field("nodeTypeAlias", Blogpost.ModelTypeAlias.ToLower());

            searchQuery = terms.Aggregate(searchQuery, (current, term) => current.And().Field("excerpt", term));

            var searchResults = searcher.Search(searchQuery.ToString(), true);
            var blogposts = searchResults.Select(x => new BlogpostDataModel {Id = x.Fields["_NodeId"], Url = x.Fields["urlName"], Excerpt = x.Fields["excerpt"], Name = x.Fields["nodeName"], Categories = x.Fields["category"].Split(' ').ToList(), CreateDate = new DateTime()});
            return blogposts.ToList();
        }
    }
}