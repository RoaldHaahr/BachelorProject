using System.Collections.Generic;
using System.Linq;
using DBC.Models;
using DBC.Models.PetaPocoDataModels;
using Umbraco.Core;

namespace DBC.Search.PetaPoco
{
    public class PetaPocoSearchApi
    {
        public static List<BlogpostPetaPocoDataModel> GetBlogposts()
        {
            // connect to the db
            var dbContext = ApplicationContext.Current.DatabaseContext.Database;

            var query = $"SELECT * FROM {BlogpostPetaPocoDataModel.TABLENAME}";

            var results = dbContext.Query<BlogpostPetaPocoDataModel>(query).ToList();

            return results;
        }
    }
}