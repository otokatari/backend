using Microsoft.EntityFrameworkCore;
using OtokatariBackend.Persistence.MySQL.Model;

namespace OtokatariBackend.Persistence.MySQL.DAO
{
    public partial class OtokatariContext : DbContext
    {
        public OtokatariContext()
        {
        }

        public OtokatariContext(DbContextOptions<OtokatariContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Follows> Follows { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follows>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("follows", "otokatari");

                entity.HasIndex(e => e.FollowUserid)
                    .HasName("follows_follow_userid_index");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("varchar(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.FollowUserid)
                    .HasColumnName("follow_userid")
                    .HasColumnType("varchar(20)");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("user_login", "otokatari");

                entity.HasIndex(e => e.Identifier)
                    .HasName("user_login_identifier_index");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("varchar(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Credentials)
                    .HasColumnName("credentials")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Identifier)
                    .IsRequired()
                    .HasColumnName("identifier")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(4)");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("user_profile", "otokatari");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("varchar(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Avatar)
                    .HasColumnName("avatar")
                    .IsUnicode(false);

                entity.Property(e => e.Birthday)
                    .HasColumnName("birthday")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Nickname)
                    .HasColumnName("nickname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sex)
                    .HasColumnName("sex")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Signature)
                    .HasColumnName("signature")
                    .IsUnicode(false);
            });
        }
    }
}
