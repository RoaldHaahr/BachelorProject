using System.Web.Http;
using DBC.Search.Lucene;
using DBC.Search.Mongo;
using DBC.Search.PetaPoco;
using Umbraco.Web.WebApi;

namespace DBC.Controllers.ApiControllers
{
    public class IndexApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public object RebuildMongo(bool status = false)
        {
            return MongoIndexer.Rebuild(status);
        }

        [HttpGet]
        public object RebuildLucene(bool status = false)
        {
            return LuceneIndexer.Rebuild(status);
        }

        [HttpGet]
        public object RebuildPetaPoco(bool status = false)
        {
            return PetaPocoIndexer.Rebuild(status);
        }

        [HttpGet]
        public object RebuildAll(bool status = false)
        {
            PetaPocoIndexer.Rebuild(status);
            MongoIndexer.Rebuild(status);
            LuceneIndexer.Rebuild(status);

            return "OK";
        } 
    }
}