using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using DBC.Models;
using DBC.Search.Lucene;
using DBC.Search.Mongo;
using DBC.Search.PetaPoco;
using DBC.ViewModels;

namespace DBC.Controllers
{
    public class TestController
    {
        private readonly PropertyInfo[] _properties;
        private readonly PropertyInfo[] _viewModelProperties;
        private readonly List<string> _vmProperties;
        private readonly List<string> _searchQueries;

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

        public string Lucene()
        {
            TestViewModel testViewModel = new TestViewModel();

            foreach (var searchQuery in _searchQueries)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var blogposts = LuceneSearchApi.GetBlogposts(searchQuery); // pass searchQuery variable
                stopwatch.Stop();

                testViewModel.TestResultList.Add(new TestModel
                {
                    SearchQuery = searchQuery,
                    SearchResults = blogposts.Count,
                    SearchTime = stopwatch.Elapsed.TotalSeconds
                });
            }

            testViewModel.AverageTime = testViewModel.TestResultList.Average(x => x.SearchTime);
            var results = testViewModel.TestResultList.Select(x => x.SearchTime).ToList();
            results.RemoveAt(0);
            testViewModel.AverageTimeMinFirst = results.Average();
            testViewModel.MinTime = testViewModel.TestResultList.Min(x => x.SearchTime);
            testViewModel.MaxTime = testViewModel.TestResultList.Max(x => x.SearchTime);

            return CreateTable(testViewModel, "Lucene");
        }

        public string Mongo()
        {
            TestViewModel testViewModel = new TestViewModel();

            foreach (var searchQuery in _searchQueries)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var blogposts = MongoSearchApi.GetBlogposts(searchQuery); // pass searchQuery variable
                stopwatch.Stop();

                testViewModel.TestResultList.Add(new TestModel
                {
                    SearchQuery = searchQuery,
                    SearchResults = blogposts.Count,
                    SearchTime = stopwatch.Elapsed.TotalSeconds
                });
            }

            testViewModel.AverageTime = testViewModel.TestResultList.Average(x => x.SearchTime);
            var results = testViewModel.TestResultList.Select(x => x.SearchTime).ToList();
            results.RemoveAt(0);
            testViewModel.AverageTimeMinFirst = results.Average();
            testViewModel.MinTime = testViewModel.TestResultList.Min(x => x.SearchTime);
            testViewModel.MaxTime = testViewModel.TestResultList.Max(x => x.SearchTime);

            return CreateTable(testViewModel, "MongoDB");
        }

        public string PetaPoco()
        {
            TestViewModel testViewModel = new TestViewModel();

            foreach (var searchQuery in _searchQueries)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var blogposts = PetaPocoSearchApi.GetBlogposts(searchQuery); // pass searchQuery variable
                stopwatch.Stop();

                testViewModel.TestResultList.Add(new TestModel
                {
                    SearchQuery = searchQuery,
                    SearchResults = blogposts.Count,
                    SearchTime = stopwatch.Elapsed.TotalSeconds
                });
            }

            testViewModel.AverageTime = testViewModel.TestResultList.Average(x => x.SearchTime);
            var results = testViewModel.TestResultList.Select(x => x.SearchTime).ToList();
            results.RemoveAt(0);
            testViewModel.AverageTimeMinFirst = results.Average();
            testViewModel.MinTime = testViewModel.TestResultList.Min(x => x.SearchTime);
            testViewModel.MaxTime = testViewModel.TestResultList.Max(x => x.SearchTime);

            return CreateTable(testViewModel, "Peta Poco");
        }

        private string CreateTable(TestViewModel testViewModel, string name)
        {
            TestViewModel viewModel = new TestViewModel();

            viewModel.TestResultList.Add(new TestModel
            {
                SearchQuery = "test",
                SearchResults = 4,
                SearchTime = 4.3
            });

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
            foreach(var item in testViewModel.TestResultList)
            {
                sb.Append("<tr>");
                foreach(var property in _properties)
                {
                    sb.Append($"<td>{item.GetType().GetProperties().Single(x => x.Name == property.Name).GetValue(item, null)}</td>");
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
                    sb.Append($"<td>{testViewModel.GetType().GetProperties().Single(x => x.Name == property.Name).GetValue(testViewModel, null)}</td>");
                }
            }

            sb.Append("</tbody>");
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}