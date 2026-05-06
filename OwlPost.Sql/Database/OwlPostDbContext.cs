namespace OwlPost.Sql.Database;

public class OwlPostDbContext : DbContext
{
    #region Ctor

    internal OwlPostDbContext(DbContextOptions<OwlPostDbContext> options) 
        : base(options)
    {

    }

    #endregion

    #region DbSet

    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatMessageHistory> ChatMessageHistories { get; set; }
    

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseCollation(DbContextHelper.Collation);

        DbContextHelper.EnableIsDeletedQueryFilter(modelBuilder);
        DbContextHelper.ConfigureEntities(modelBuilder);
        DbContextHelper.SeedValues(modelBuilder);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(DbContextHelper.ConnectionString);
        optionsBuilder.EnableDetailedErrors(DbContextHelper.EnableDetailedErrors);
        optionsBuilder.EnableSensitiveDataLogging(DbContextHelper.EnableSensitiveDataLogging);

    }

}