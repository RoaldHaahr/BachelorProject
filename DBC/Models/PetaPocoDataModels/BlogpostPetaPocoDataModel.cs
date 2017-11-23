using System;
using System.Collections.Generic;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace DBC.Models.PetaPocoDataModels
{
    [TableName(TABLENAME)]
    [PrimaryKey("Id")]
    public class BlogpostPetaPocoDataModel
    {
        public const string TABLENAME = "rh_blogposts";

        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Excerpt")]
        public string Excerpt { get; set; }
        [Column("Url")]
        public string Url { get; set; }
        [Column("Categories")]
        public string Categories { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
}