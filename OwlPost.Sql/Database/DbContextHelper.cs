namespace OwlPost.Sql.Database;

public class DbContextHelper
{
    internal static string Collation => "Latin1_General_100_CI_AI_SC_UTF8";

    internal static bool EnableDetailedErrors => ApplicationSetting.IsDebugMode;
    internal static bool EnableSensitiveDataLogging => ApplicationSetting.IsDebugMode;

    internal static void EnableIsDeletedQueryFilter(ModelBuilder modelBuilder)
    {
        //ignored
    }

    internal static string ConnectionString
    {
        get
        {
            //m.hamzeh
            if (string.Equals(Environment.MachineName, "MEYTOL_PC", StringComparison.CurrentCultureIgnoreCase))
                return "Data Source=.;Initial Catalog=OwlPostDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";


            if (ApplicationSetting.IsDebugMode)
                return "";
            else
                return "";
        }

    }

    internal static void SeedValues(ModelBuilder modelBuilder)
    {

    }

    internal static void ConfigureEntities(ModelBuilder modelBuilder)
    {

    }


    #region FillBaseValues

    [Obsolete(message: "", error: true)]
    private static void FillBaseValues_Template(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<InvoicePaymentCalculationType>().HasData(
        //   new InvoicePaymentCalculationType()
        //   {
        //       Id = InvoicePaymentCalculationTypeEnm.Unknown,
        //       PublicId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
        //       Title = InvoicePaymentCalculationTypeEnm.Unknown.GetDescription(),
        //       TitleEn = nameof(InvoicePaymentCalculationTypeEnm.Unknown),
        //       Description = string.Empty
        //   },
        //   );
    }

    #endregion

    #region ConfigureEntities

    [Obsolete(message: "", error: true)]
    private static void ConfigureEntity_Template(ModelBuilder modelBuilder)
    {
        #region Properties

        //modelBuilder.Entity<Apartment>().Property(p => p.PostalCode)
        //    .HasMaxLength(10)
        //    .IsUnicode(false)
        //    .IsFixedLength()
        //    .HasComment("کد پستی");

        #endregion

        #region Relations

        //modelBuilder.Entity<Apartment>()
        //   .HasMany(e => e.ApartmentPeople)
        //   .WithOne(e => e.Apartment)
        //   .HasForeignKey(e => e.ApartmentId);

        //modelBuilder.Entity<Apartment>()
        //   .HasOne(e => e.ApartmentPeople)
        //   .WithMany(e => e.Apartment)
        //   .HasForeignKey(e => e.ApartmentId);

        #endregion

        #region Index

        //modelBuilder.Entity<Apartment>().HasIndex(e => e.Guid).IsUnique();

        #endregion
    }



    #endregion


}