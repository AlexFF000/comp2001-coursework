using Microsoft.EntityFrameworkCore;


#nullable disable

namespace AuthAPI.Models
{
    public partial class COMP2001_ARedmondContext : DbContext
    {
        public COMP2001_ARedmondContext()
        {
        }

        public COMP2001_ARedmondContext(DbContextOptions<COMP2001_ARedmondContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Password> Passwords { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<Password>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.TimeChanged })
                    .HasName("PK__Password__2759FE5905E0D62D");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.TimeChanged)
                    .HasColumnType("datetime")
                    .HasColumnName("time_changed");

                entity.Property(e => e.Password1)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Passwords)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Passwords__user___6ABAD62E");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.SessionTime })
                    .HasName("PK__Sessions__0DA477DFD7ED59C6");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.SessionTime)
                    .HasColumnType("datetime")
                    .HasColumnName("session_time");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sessions__user_i__6E8B6712");
            });

            modelBuilder.Entity<User>(entity =>
            {
                /* 
                This overload is only added in EF 5, so will give error: "Cannot convert lambda expression to type 'string' because it is not a delegate type
                entity.HasIndex(e => e.Email, "UQ__Users__AB6E61645165AA5F");
                */
                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(320)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("last_name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
