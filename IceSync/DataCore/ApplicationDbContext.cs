
using IceSync.DataCore.Models;
using Microsoft.EntityFrameworkCore;

namespace IceSync.DataCore;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Workflow> Workflows { get; set; }
}