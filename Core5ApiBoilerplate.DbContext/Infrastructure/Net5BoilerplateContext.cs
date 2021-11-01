using Core5ApiBoilerplate.DbContext.Entities;
using Core5ApiBoilerplate.DbContext.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core5ApiBoilerplate.DbContext.Infrastructure
{
    // public partial class Net5BoilerplateContext : DbContext // default
    // public class Net5BoilerplateContext : IdentityDbContext<ApplicationUser> // 
    // public class Net5BoilerplateContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int> // not enough functionalities 1
    // public class Net5BoilerplateContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>  // not enough functionalities 2
    public partial class Net5BoilerplateContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        //public Net5BoilerplateContext() { }

        public Net5BoilerplateContext(DbContextOptions<Net5BoilerplateContext> options) : base(options)
        {
            // Not needed, why? Cause:
            /*
             * The simplest way to use lazy-loading is by installing the Microsoft.EntityFrameworkCore.
             * Proxies package and enabling it with a call to UseLazyLoadingProxies. For example:
             * OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                    => optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(myConnectionString);
             */
            // this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public static Net5BoilerplateContext Create(string connection, int organizationType)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Net5BoilerplateContext>();
            optionsBuilder.UseSqlServer(connection);
            return new Net5BoilerplateContext(optionsBuilder.Options);
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Post> Posts { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=Test;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            base.OnModelCreating(modelBuilder); // DO NOT - under any circumstances - REMOVE THIS LINE. This line basically calls our identity constructor first, and without it NOTHING REGARDING IDENTITY WORKS!

            #region Identity stuff
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");

            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");
            modelBuilder.Entity<ApplicationUserToken>().ToTable("UserTokens");
            #endregion


            #region Global filters
            // https://docs.microsoft.com/en-us/ef/core/querying/filters
            // modelBuilder.Entity<Blog>().HasQueryFilter(a => a.OrganisationType == OrganizationType);
            #endregion

            #region Sequences 
            modelBuilder.HasSequence<long>("BlogSeq")
                .StartsAt(100)
                .IncrementsBy(1)
                .HasMin(100);

            modelBuilder.Entity<Blog>()
                .Property(o => o.Oid)
                .HasDefaultValueSql("NEXT VALUE FOR BlogSeq");
            #endregion
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
