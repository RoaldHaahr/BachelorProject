using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using DBC.Models;
using DBC.Models.MongoDataModels;
using DBC.Models.PetaPocoDataModels;
using DBC.Search.Examine;
using DBC.Search.Lucene;
using DBC.Search.Mongo;
using DBC.Search.PetaPoco;
using DBC.Search.Umbraco;
using DBC.ViewModels;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedContentModels;

namespace DBC.Controllers
{
    public class TestController
    {
        private readonly PropertyInfo[] _properties;
        private readonly PropertyInfo[] _viewModelProperties;
        private readonly List<string> _vmProperties;
        private readonly List<string> _searchQueries;
        private const int NUMBER_OF_TESTS = 20;

        public TestController()
        {
            _properties = typeof(TestModel).GetProperties();
            _viewModelProperties = typeof(TestViewModel).GetProperties();

            _vmProperties = new List<string>();
            foreach (var property in _viewModelProperties)
            {
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(int) || property.PropertyType == typeof(double) || property.PropertyType == typeof(long))
                {
                    _vmProperties.Add(property.Name);
                }
            }

            _searchQueries = new List<string>
            {
                "",
                "",
                "test",
                "skill",
                "built",
                "built none",
                "project walls",
                "year and route",
                "one us that",
                "and",
                "to",
                "or",
                "the",
                "Consumerism",
                "Thanksgiving",
                "shopping",
                "million",
                "people"
            };
        }

        public string Examine()
        {
            return BuildTest("Examine", ExamineSearchApi.GetBlogposts);
        }

        public string UmbracoHelper()
        {
            return BuildTest("Umbraco | UmbracoHelper", UmbracoSearchApi.GetBlogpostsWithUmbracoHelper);
        }

        public string UmbracoContentService(IPublishedContent model)
        {
            return BuildTest("Umbraco | Content service", null, model);
        }

        public string Lucene()
        {
            return BuildTest("Lucene", LuceneSearchApi.GetBlogposts);
        }

        public string Mongo()
        {
            return BuildTest("Mongo", MongoSearchApi.GetBlogposts);           
        }

        public string PetaPoco()
        {
            return BuildTest("PetaPoco", PetaPocoSearchApi.GetBlogposts);
        }

        private string BuildTest(string name, Func<string, List<BlogpostDataModel>> searchApiFunc, IPublishedContent model = null)
        {
            TestViewModel testViewModel = new TestViewModel();

            foreach (var searchQuery in _searchQueries)
            {
                var blogposts = new List<BlogpostDataModel>();
                var totalTime = new double();
                for (int i = 0; i < NUMBER_OF_TESTS; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    blogposts = model == null ? searchApiFunc(searchQuery) : UmbracoSearchApi.GetBlogpostsWithContentService(model, searchQuery);
                    stopwatch.Stop();
                    totalTime += stopwatch.Elapsed.TotalSeconds;
                }

                testViewModel.TestResultList.Add(new TestModel
                {
                    SearchQuery = searchQuery,
                    SearchResults = blogposts.Count,
                    SearchTime = totalTime / NUMBER_OF_TESTS
                });
            }

            testViewModel.AverageTime = testViewModel.TestResultList.Average(x => x.SearchTime);
            var results = testViewModel.TestResultList.Select(x => x.SearchTime).ToList();
            results.RemoveAt(0);
            testViewModel.AverageTimeMinFirst = results.Average();
            testViewModel.MinTime = testViewModel.TestResultList.Min(x => x.SearchTime);
            testViewModel.MaxTime = testViewModel.TestResultList.Max(x => x.SearchTime);

            return CreateTable(testViewModel, name);
        } 

        private string CreateTable(TestViewModel testViewModel, string name)
        {
            var sb = new StringBuilder();
            sb.Append($"<h2>{name}</h2>");
            sb.Append("<table>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            foreach (var property in _properties)
            {
                sb.Append($"<th>{property.Name}</th>");
            }
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            foreach (var item in testViewModel.TestResultList)
            {
                sb.Append("<tr>");
                foreach (var property in _properties)
                {
                    var propertyValue = item.GetType().GetProperties().Single(x => x.Name == property.Name).GetValue(item, null);

                    if (propertyValue is double doubleValue)
                    {
                        propertyValue = doubleValue.ToString("0." + new string('#', 339));
                    }

                    sb.Append($"<td>{propertyValue}</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody>");
            sb.Append("</table>");

            sb.Append("<br />");
            sb.Append("<table>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            foreach (var item in _vmProperties)
            {
                sb.Append($"<th>{item}</th>");
            }
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");

            foreach (var property in _viewModelProperties)
            {
                if (_vmProperties.Contains(property.Name))
                {
                    var propertyValue = testViewModel.GetType().GetProperties().Single(x => x.Name == property.Name).GetValue(testViewModel, null);

                    if (propertyValue is double doubleValue)
                    {
                        propertyValue = doubleValue.ToString("0." + new string('#', 339));
                    }

                    sb.Append($"<td>{propertyValue}</td>");
                }
            }

            sb.Append("</tbody>");
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}