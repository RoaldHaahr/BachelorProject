using System;
using System.Configuration;
using DBC.Models.MongoDataModels;
using MongoDB.Driver;

namespace DBC.App_Start
{
    public class MongoContext
    {
        public MongoClient Client;
        public IMongoDatabase Database;

        public MongoContext()        //constructor   
        {
            // Reading credentials from Web.config file   
            var mongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"]; //blog
            var mongoUsername = ConfigurationManager.AppSettings["MongoUsername"]; //rh  
            var mongoPassword = ConfigurationManager.AppSettings["MongoPassword"]; //1234
            var mongoPort = ConfigurationManager.AppSettings["MongoPort"];  //27017  
            var mongoHost = ConfigurationManager.AppSettings["MongoHost"];  //localhost  

            // Creating credentials  
            var credential = MongoCredential.CreateMongoCRCredential
            (mongoDatabaseName,
                mongoUsername,
                mongoPassword);

            // Creating MongoClientSettings  
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