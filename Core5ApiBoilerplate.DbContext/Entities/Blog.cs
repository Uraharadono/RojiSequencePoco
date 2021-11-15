using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core5ApiBoilerplate.Infrastructure.Repository;

namespace Core5ApiBoilerplate.DbContext.Entities
{
    public class Blog : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Oid { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }
}
