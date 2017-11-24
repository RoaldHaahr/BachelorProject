using System.Collections.Generic;
using System.Linq;
using DBC.Interfaces;
using DBC.Models;
using DBC.Models.PetaPocoDataModels;
using Umbraco.Core;

namespace DBC.Search.PetaPoco
{
    public class PetaPocoSearchApi
    {
        public static List<BlogpostPetaPocoDataModel> GetBlogposts(string query = "")
        {
            // connect to the db
            var dbContext = ApplicationContext.Current.DatabaseContext.Database;

            var searchQuery = $"SELECT * FROM {BlogpostPetaPocoDataModel.TABLENAME}{(!string.IsNullOrEmpty(query) ? " WHERE [Excerpt] LIKE '%" + query + "%'" : string.Empty)}";

            var results = dbContext.Query<BlogpostPetaPocoDataModel>(searchQuery).ToList();

            return results;
        }
    }
}