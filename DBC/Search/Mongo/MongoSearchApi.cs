using System.Collections.Generic;
using DBC.App_Start;
using DBC.Models.MongoDataModels;
using MongoDB.Driver;

namespace DBC.Search.Mongo
{
    public class MongoSearchApi
    {
        public static List<BlogpostMongoDataModel> GetBlogposts()
        {
            var dbContext = new MongoContext();

            var collection = dbContext.Database.GetCollection<BlogpostMongoDataModel>(Constants.MONGO_BLOGPOSTS_NAME);

            var filter = MongoDB.Driver.Builders<BlogpostMongoDataModel>.Filter.Empty;

            return collection.Find(filter).ToList();
        }
    }
}