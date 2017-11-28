using System.Collections.Generic;
using System.Linq;
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
            var builder = Builders<BlogpostMongoDataModel>.Filter;
            var filter = builder.Empty;
            var terms = query.Split(' ');

            if (!terms.Any())
            {
                return collection.Find(filter).ToList();
            }

            var filters = terms.Select(term => builder.Regex("excerpt", new BsonRegularExpression(term, "i"))).ToList();

            filter = builder.And(filters);

            var blogpostMongoDataModels = collection.Find(filter).ToList();
            return blogpostMongoDataModels;
        }
    }
}