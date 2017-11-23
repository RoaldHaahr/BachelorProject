using System.Linq;
using DBC.Models.PetaPocoDataModels;
using DBC.Search;
using DBC.Search.Lucene;
using DBC.Search.Mongo;
using DBC.Search.PetaPoco;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web.PublishedContentModels;

namespace DBC.EventHandlers
{
    public class SearchEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Get the Umbraco Database context
            var ctx = applicationContext.DatabaseContext;
            var db = new DatabaseSchemaHelper(ctx.Database, applicationContext.ProfilingLogger.Logger, ctx.SqlSyntax);

            //Check if the table does exist in the db
            if (!db.TableExist(BlogpostPetaPocoDataModel.TABLENAME))
            {
                db.CreateTable<BlogpostPetaPocoDataModel>(false);
            }

            ContentService.Published += UpdateIndeces;
        }

        public void UpdateIndeces(IPublishingStrategy sender, PublishEventArgs<IContent> args)
        {
            foreach (var node in args.PublishedEntities.Where(x => x.ContentType.Alias == Blogpost.ModelTypeAlias))
            {
                MongoIndexer.Index(node.Id);
                LuceneIndexer.Index(node.Id);
                PetaPocoIndexer.Index(node.Id);
            }
        }
    }
}