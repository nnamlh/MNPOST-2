using Microsoft.AspNet.Identity.EntityFramework;

namespace MNPOST.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int IsActivced { get; set; }

        public string FullName { get; set; }

        public string AccountType { get; set; }

        public string GroupId { get; set; }

        public string ULevel { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}