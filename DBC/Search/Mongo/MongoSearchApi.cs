using System.Collections.Generic;
using DBC.App_Start;
using DBC.Models.MongoDataModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBC.Search.Mongo
{
    public class MongoSearchApi
    {
        public static List<BlogpostMongoDataModel> GetBlogposts(string query = "")
        {
            var dbContext = new MongoContext();
            var collection = dbContext.Database.GetCollection<BlogpostMongoDataModel>(Constants.MONGO_BLOGPOSTS_NAME);
            var filter = MongoDB.Driver.Builders<BlogpostMongoDataModel>.Filter.Empty;
            if (!string.IsNullOrEmpty(query))
            {
                filter = Builders<BlogpostMongoDataModel>.Filter.Regex("excerpt", new BsonRegularExpression(query.ToLower()));
            }

            var blogpostMongoDataModels = collection.Find(filter).ToList();
            return blogpostMongoDataModels;
        }
    }
}