using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var baseSql = $"SELECT * FROM {BlogpostPetaPocoDataModel.TABLENAME}";

            if (string.IsNullOrEmpty(query))
            {
                return dbContext.Query<BlogpostPetaPocoDataModel>(baseSql).ToList();
            }

            var terms = query.Split(' ');

            var sb = new StringBuilder();

            sb.Append(baseSql);
            sb.Append(" WHERE ");
            foreach (var term in terms)
            {
                if (term != terms[0])
                {
                    sb.Append(" AND ");
                }
                sb.Append($"lower([Excerpt]) LIKE '%{term}%'");
                //sb.Append($"(lower([Excerpt]) LIKE '% {term} %' OR lower([Excerpt]) LIKE '{term} %' OR lower([Excerpt]) LIKE '% {term}.%') OR lower([Excerpt]) LIKE '% {term},%'");
            }

            var searchQuery = sb.ToString();
            //var sql = $"{baseSql}{searchQuery}";

            var results = dbContext.Query<BlogpostPetaPocoDataModel>(searchQuery).ToList();

            return results;
        }
    }
}