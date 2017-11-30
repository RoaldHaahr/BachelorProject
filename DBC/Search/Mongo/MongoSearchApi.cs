using System.Collections.Generic;
using System.Linq;
using DBC.Models;
using DBC.Models.MongoDataModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBC.Search.Mongo
{
    public class MongoSearchApi
    {
        public static List<BlogpostDataModel> GetBlogposts(string query = "")
        {
            var dbContext = new MongoContext();
            var collection = dbContext.Database
                .GetCollection<BlogpostMongoDataModel>(Constants.MONGO_BLOGPOSTS_NAME);
            var builder = Builders<BlogpostMongoDataModel>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                var terms = query.Split(' ');
                var filters = terms
                    .Select(term => builder.Regex("excerpt", new BsonRegularExpression(term, "i")))
                    .ToList();
                filter = builder.And(filters);
            }

            return collection.Find(filter)
                .ToEnumerable()
                .Select(x => new BlogpostDataModel
                {
                    Categories = x.Categories,
                    CreateDate = x.CreateDate,
                    Excerpt = x.Excerpt,
                    Url = x.Url,
                    Id = x.Id.ToString(),
                    Name = x.Name
                })
                .ToList();
        }
    }
}