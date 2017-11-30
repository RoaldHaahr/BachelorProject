using System.Linq;
using DBC.Models.PetaPocoDataModels;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;

namespace DBC.Search.PetaPoco
{
    public class PetaPocoIndexer
    {
        public static bool Index(int id)
        {
            var dbContext = ApplicationContext.Current.DatabaseContext.Database;
            var dbSqlSyntaxProvider = ApplicationContext.Current.DatabaseContext.SqlSyntax;
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            var content = umbracoHelper.TypedContent(id) as Blogpost;

            if (content == null)
            {
                return false;
            }
            
            var model = new BlogpostPetaPocoDataModel
            {
                Name = content.Name,
                Url = content.Url,
                Categories = string.Join(",", content.Categories),
                CreateDate = content.CreateDate,
                Excerpt = content.Excerpt,
                Id = content.Id
            };

            var query = new Sql().Select("*")
                .From<BlogpostPetaPocoDataModel>(dbSqlSyntaxProvider)
                .Where($"[Id] = {id}");

            var nodeExists = dbContext.Query<BlogpostPetaPocoDataModel>(query).Any();

            if (nodeExists)
            {
                dbContext.Update(BlogpostPetaPocoDataModel.TABLENAME, "Id", model, model.Id);
            }
            else
            {
                dbContext.Insert(BlogpostPetaPocoDataModel.TABLENAME, "Id", false, model);
            }

            return true;
        }

        public static void Delete (int id)
        {
            // connect to the db
            var dbContext = ApplicationContext.Current.DatabaseContext.Database;

            // delete node
            var deleteQuery = $"DELETE FROM {BlogpostPetaPocoDataModel.TABLENAME} WHERE [Id] = {id}";
            dbContext.Execute(deleteQuery);
        }

        public static string Rebuild(bool status = false)
        {
            var dbContext = ApplicationContext.Current.DatabaseContext.Database;
            var deleteQuery = $"DELETE FROM {BlogpostPetaPocoDataModel.TABLENAME}";

            dbContext.Execute(deleteQuery);

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var blogposts = umbracoHelper
                .TypedContentAtRoot()
                .Where(x => x.DocumentTypeAlias == Home.ModelTypeAlias)
                .SelectMany(y => y.Descendants<Blogpost>())
                .Select(x => new BlogpostPetaPocoDataModel
                {
                    Id = x.Id,
                    Categories = string.Join(",", x.Categories),
                    Excerpt = x.Excerpt,
                    CreateDate = x.CreateDate,
                    Name = x.Name,
                    Url = x.Url
                })
                .ToList();

            foreach (var blogpost in blogposts)
            {
                dbContext.Insert(BlogpostPetaPocoDataModel.TABLENAME, "Id", false, blogpost);
            }

            return "OK";
        }
    }
}