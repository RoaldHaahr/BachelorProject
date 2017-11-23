using System;
using System.IO;
using System.Linq;
using System.Web;
using DBC.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Newtonsoft.Json;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;
using Version = Lucene.Net.Util.Version;

namespace DBC.Search.Lucene
{
    public class LuceneIndexer
    {
        public const string INDEX_PATH = "~/App_Data/TEMP/ArlanetIndex/Index/";
        //private const int INDEX_BATCHSIZE = 100;

        private static bool _busyIndexing;
        private static readonly object _lockIndex = new object();
        private static string _status;

        //Serializer settings to store information about the serialized object with it
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static string Rebuild(bool status = false)
        {
            if (_busyIndexing)
            {
                return $"BUSY: {_status ?? "Starting..."}";
            }

            if (status)
            {
                return "OK";
            }

            // NOTE: Does this have to be changed to have multiple locks?
            lock (_lockIndex)
            {
                _busyIndexing = true;

                try
                {
                    InternalRebuild();
                }
                catch (Exception ex)
                {
                    _busyIndexing = false;
                    return ex.ToString();
                }

                _busyIndexing = false;
            }

            return "OK";
        }

        private static void InternalRebuild()
        {
            //Get the absolute path to the directory where the indexes will be created (and if it doesn't exist, create it)
            string directoryPath = HttpContext.Current.Server.MapPath(INDEX_PATH);

            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FSDirectory directory = FSDirectory.Open(directoryInfo);

            //Select the standard Lucene analyser
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);

            using (IndexWriter writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.DeleteAll();

                //Get all the nodes we wish to index
                _status = "Retrieving home...";

                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                var home = umbracoHelper
                    .TypedContentAtRoot()
                    .Cast<Home>()
                    .SingleOrDefault();

                if (home == null) { return; }

                var blog = home.Descendants<Blogpost>();

                _status = "Indexing blog...";

                foreach (var blogpost in blog)
                {
                    Document document = null;

                    try
                    {
                        document = BlogpostToDocument(blogpost);
                    }
                    catch
                    {
                        //TODO: Logging?
                    }

                    if (document != null)
                    {
                        writer.AddDocument(document);
                    }
                }

                //We optimize the index and close the writer
                writer.Optimize();
                writer.Close();
            }

            Searcher.Reset();

            _status = null;
        }

        private static Document BlogpostToDocument(Blogpost blogpost)
        {
            var doc = new Document();

            var id = blogpost.Id.ToString();
            var pageTitle = blogpost.PageTitle;
            var categories = blogpost.Categories?.ToList();
            var excerpt = blogpost.Excerpt;
            var url = blogpost.Url;
            var createDate = blogpost.CreateDate;

            var dataObject = new BlogpostDataModel
            {
                Id = id,
                Url = url,
                Name = pageTitle,
                Excerpt = excerpt,
                Categories = categories,
                CreateDate = createDate
            };

            var data = JsonConvert.SerializeObject(dataObject, _jsonSerializerSettings);
            
            doc.Add(new Field("Id", id, Field.Store.NO, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
            doc.Add(new Field("Data", data, Field.Store.YES, Field.Index.NO, Field.TermVector.NO));

            return doc;
        }

        public static bool Index(int id)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            var content = umbracoHelper.TypedContent(id) as Blogpost;

            string directoryPath = HttpContext.Current.Server.MapPath(INDEX_PATH);

            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FSDirectory directory = FSDirectory.Open(directoryInfo);

            var analyzer = new StandardAnalyzer(Version.LUCENE_29);

            using (IndexWriter writer = new IndexWriter(directory, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                try
                {
                    Term term = new Term("Id", id.ToString());

                    writer.DeleteDocuments(term);

                    var document = BlogpostToDocument(content);

                    if (document == null)
                    {
                        return false;
                    }

                    writer.AddDocument(document);

                    writer.Close();
                }
                catch (Exception)
                {
                    //Do nothing
                }
            }

            Searcher.Reset();

            return false;
        }

        public static void RemoveFromIndex(int id)
        {
            string directoryPath = HttpContext.Current.Server.MapPath(INDEX_PATH);

            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FSDirectory directory = FSDirectory.Open(directoryInfo);

            var analyzer = new StandardAnalyzer(Version.LUCENE_29);

            using (IndexWriter writer = new IndexWriter(directory, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                try
                {
                    Term term = new Term("Id", id.ToString());
                    writer.DeleteDocuments(term);

                    writer.Close();
                }
                catch (Exception)
                {
                    //Do nothing
                }
            }

            Searcher.Reset();
        }
    }
}