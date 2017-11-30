using System;
using System.Configuration;
using MongoDB.Driver;

namespace DBC
{
    public class MongoContext
    {
        public MongoClient Client;
        public IMongoDatabase Database;

        public MongoContext()   
        {
            var mongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"]; //blog
            var mongoUsername = ConfigurationManager.AppSettings["MongoUsername"]; //rh  
            var mongoPassword = ConfigurationManager.AppSettings["MongoPassword"]; //1234
            var mongoPort = ConfigurationManager.AppSettings["MongoPort"];  //27017  
            var mongoHost = ConfigurationManager.AppSettings["MongoHost"];  //localhost  

            var credential = MongoCredential.CreateMongoCRCredential(
                mongoDatabaseName,
                mongoUsername,
                mongoPassword
            );

            var settings = new MongoClientSettings
            {
                Credentials = new[] { credential },
                Server = new MongoServerAddress(mongoHost, Convert.ToInt32(mongoPort))
            };

            Client = new MongoClient(settings);
            Database = Client.GetDatabase(mongoDatabaseName);
        }
    }
}