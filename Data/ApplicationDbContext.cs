using Eventflow.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventflow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }


    }
}
