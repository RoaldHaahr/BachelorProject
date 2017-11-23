using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Newtonsoft.Json;
using Version = Lucene.Net.Util.Version;

namespace DBC.Search.Lucene
{
    public class Searcher
    {
        public class SortOptions
        {
            public enum ESortField
            {
                Integer = SortField.INT,
                String = SortField.STRING
            }

            public string FieldName = null;
            public ESortField FieldType = ESortField.String;
            public bool Ascending = true;
        }

        private static readonly object _lockIndexSearcher = new object();
        private static IndexSearcher _indexSearcher;

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static List<T> RawQuery<T>(string query, SortOptions sortOptions = null)
            where T : class
        {
            var parser = new QueryParser(Version.LUCENE_29, "Id", new StandardAnalyzer(Version.LUCENE_29));
            parser.SetAllowLeadingWildcard(true);
            
            Query luceneQuery = parser.Parse(query);

            return RawQuery<T>(luceneQuery);
        }

        public static List<T> RawQuery<T>(Query query, SortOptions sortOptions = null)
            where T : class
        {
            List<T> results = new List<T>();

            IndexSearcher searcher = GetIndexSearcher();

            TopDocsCollector collector;

            if (sortOptions == null)
            {
                collector = TopScoreDocCollector.create(searcher.MaxDoc(), true);
            }
            else
            {
                collector = TopFieldCollector.create(
                    new Sort(new SortField(sortOptions.FieldName, (int)sortOptions.FieldType, sortOptions.Ascending)),
                    searcher.MaxDoc(),
                    false,
                    false,
                    false,
                    true
                );
            }

            searcher.Search(query, collector);
            var topDocs = collector.TopDocs();
            var scoreDocs = topDocs.ScoreDocs;

            var maxRecord = scoreDocs.Length;

            for (int index = 0; index < maxRecord && index < scoreDocs.Length; index++)
            {
                ScoreDoc scoreDoc = scoreDocs[index];
                Document doc = searcher.Doc(scoreDoc.doc);

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
            string directoryPath = HttpContext.Current.Server.MapPath(LuceneIndexer.INDEX_PATH);
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            if (_indexSearcher != null)
            {
                return _indexSearcher;
            }

            lock (_lockIndexSearcher)
            {
                //NOTE: Just in case a thread is waiting for this
                return _indexSearcher ?? (_indexSearcher = new IndexSearcher(IndexReader.Open(FSDirectory.Open(directoryInfo), true)));
            }
        }
    }
}