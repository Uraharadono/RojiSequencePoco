using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core5ApiBoilerplate.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;

namespace Core5ApiBoilerplate.DbContext.Entities.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<long>, IIdentityEntity
    {
        [Required, Column(TypeName = "VARCHAR(250)")]
        public string FirstName { get; set; }

        [Required, Column(TypeName = "VARCHAR(250)")]
        public string LastName { get; set; }

        [Required, Column(TypeName = "VARCHAR(100)")]
        public string Title { get; set; }

        public DateTime BirthDate { get; set; }

        //[Required]
        //[DefaultValue(EGenderType.Unresolved)]
        //public EGenderType Gender { get; set; }

        // public virtual ICollection<ClientRemark> Remarks { get; set; }


        /* =============== Navigation properties =============== */

        // Keep in mind this logic as it only way that 1 to 0 relation works in ef core for now - see Worker.cs
        //[Required, ForeignKey("Worker"), InverseProperty("User")]
        //public long WorkerId { get; set; }
        //public Worker Worker { get; set; }


        /* =============== Non-mapped properties =============== */
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
