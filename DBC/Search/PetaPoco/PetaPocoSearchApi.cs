using System.Collections.Generic;
using System.Linq;
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

            var queryIsEmpty = string.IsNullOrEmpty(query);

            var searchQuery = queryIsEmpty ? string.Empty : $" WHERE [Excerpt] REGEXP '[[:<:]]{string.Join("[[:>:]]' AND [Excerpt] REGEXP '[[:<:]]", query.Split(' '))}[[:>:]]'";
            var sql = $"SELECT * FROM {BlogpostPetaPocoDataModel.TABLENAME}{searchQuery}";

            var results = dbContext.Query<BlogpostPetaPocoDataModel>(sql).ToList();

            return results;
        }
    }
}