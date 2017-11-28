using System.Collections.Generic;
using System.IO;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Newtonsoft.Json;
using Version = Lucene.Net.Util.Version;

namespace DBC.Search.Lucene
{
    public class LuceneSearcher
    {
        private static readonly object _lockIndexSearcher = new object();
        private static IndexSearcher _indexSearcher;

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static List<T> RawQuery<T>(string query)
            where T : class
        {
            var parser = new QueryParser(Version.LUCENE_29, "Excerpt", new StandardAnalyzer(Version.LUCENE_29));
            parser.SetAllowLeadingWildcard(true);
            
            Query luceneQuery = parser.Parse(query);

            return RawQuery<T>(luceneQuery);
        }

        public static List<T> RawQuery<T>(Query query)
            where T : class
        {
            var results = new List<T>();
            var searcher = GetIndexSearcher();
            var collector = TopScoreDocCollector.create(searcher.MaxDoc(), false);

            searcher.Search(query, collector);

            var topDocs = collector.TopDocs();
            var scoreDocs = topDocs.ScoreDocs;

            var maxRecord = scoreDocs.Length;

            for (var index = 0; index < maxRecord; index++)
            {
                var scoreDoc = scoreDocs[index];
                var doc = searcher.Doc(scoreDoc.doc);
                var data = doc.Get("Data");
                var result = JsonConvert.DeserializeObject(data, _jsonSerializerSettings) as T;

                if (result == null)
                {
                    continue;
                }

                results.Add(result);
            }

            return results;
        }

        public static void Reset()
        {
            _indexSearcher = null;
        }

        private static IndexSearcher GetIndexSearcher()
        {
            var directoryPath = HttpContext.Current.Server.MapPath(LuceneIndexer.INDEX_PATH);
            var directoryInfo = new DirectoryInfo(directoryPath);

            if (_indexSearcher != null)
            {
                return _indexSearcher;
            }

            lock (_lockIndexSearcher)
            {
                return _indexSearcher ?? 
                    (_indexSearcher = new IndexSearcher(IndexReader.Open(FSDirectory.Open(directoryInfo), true)));
            }
        }
    }
}