using Core5ApiBoilerplate.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;

namespace Core5ApiBoilerplate.DbContext.Entities.Identity
{
    public class ApplicationRole : IdentityRole<long>, IIdentityEntity
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) { Name = name; }
    }
}
