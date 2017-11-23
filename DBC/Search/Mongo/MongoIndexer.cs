using System;
using System.Linq;
using DBC.App_Start;
using DBC.Models.MongoDataModels;
using MongoDB.Driver;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;

namespace DBC.Search.Mongo
{
    public class MongoIndexer
    {
        public static bool Index(int id)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var node = umbracoHelper.TypedContent(id) as Blogpost;

            if (node == null) return false;

            Save(node);
            return true;
        }

        private static void Save(Blogpost blogpost)
        {
            var dbContext = new MongoContext();

            var collection = dbContext.Database.GetCollection<BlogpostMongoDataModel>("blogposts");

            var blogpostModel = new BlogpostMongoDataModel
            {
                Id = blogpost.Id,
                Name = blogpost.Name,
                CreateDate = blogpost.CreateDate,
                Url = blogpost.Url,
                Categories = blogpost.Categories.ToList(),
                Excerpt = blogpost.Excerpt
            };

            var filter = Builders<BlogpostMongoDataModel>.Filter.Eq(x => x.Id, blogpostModel.Id);

            var modelExists = collection.Find(filter).Any();

            if (modelExists)
            {
                // update
                var update = Builders<BlogpostMongoDataModel>.Update
                    .Set("name", blogpostModel.Name)
                    .Set("createDate", blogpostModel.CreateDate)
                    .Set("url", blogpostModel.Url)
                    .Set("categories", blogpostModel.Categories)
                    .Set("excerpt", blogpostModel.Excerpt);
                collection.FindOneAndUpdateAsync<BlogpostMongoDataModel>(filter, update);
            }
            else
            {
                // insert
                collection.InsertOneAsync(blogpostModel);
            }
        }


        public static string Rebuild(bool status = false)
        {
            // connect to the mongodb
            var dbContext = new MongoContext();
            
            // get collection
            var collection = dbContext.Database.GetCollection<BlogpostMongoDataModel>(Constants.MONGO_BLOGPOSTS_NAME);

            // make filter to delete all documents
            var delete = Builders<BlogpostMongoDataModel>.Filter.Empty;

            // delete all documents
            collection.DeleteMany(delete);

            // get an instance of an umbraco helper
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            // get all blogposts in the db and convert the to a mongo model
            var blogposts = umbracoHelper
                .TypedContentAtRoot()
                .Where(x => x.DocumentTypeAlias == Home.ModelTypeAlias)
                .SelectMany(y => y.Descendants<Blogpost>())
                .Select(x => new BlogpostMongoDataModel { Id = x.Id, Categories = x.Categories.ToList(), Excerpt = x.Excerpt, CreateDate = x.CreateDate, Name = x.Name, Url = x.Url })
                .ToList();

            // insert each blogpost in the empty db
            foreach (var blogpost in blogposts)
            {
                collection.InsertOneAsync(blogpost);
            }

            return "OK";
        }
    }
}