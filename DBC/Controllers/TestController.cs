using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
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
        private readonly List<string> _searchQueries;

        public TestController()
        {
            _properties = typeof(TestModel).GetProperties();
            _searchQueries = new List<string>
            {
                "Test",
                "Skill",
                "built",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                ""
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
            sb.Append("<th>Min</th><th>Max</th><th>Average</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            sb.Append($"<td>{testViewModel.MinTime}</td><td>{testViewModel.MaxTime}</td><td>{testViewModel.AverageTime}</td>");
            sb.Append("</tbody>");
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}