using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace DBC.Models.MongoDataModels
{
    public class BlogpostMongoDataModel
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("createDate")]
        public DateTime CreateDate { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("categories")]
        public List<string> Categories { get; set; }

        [BsonElement("excerpt")]
        public string Excerpt { get; set; }
    }
}