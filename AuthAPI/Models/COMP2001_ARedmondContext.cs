using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=socem1.uopnet.plymouth.ac.uk;Database=COMP2001_ARedmond;User Id=ARedmond;Password=OkwW859*");
            }
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
                entity.HasIndex(e => e.Email, "UQ__Users__AB6E61645165AA5F")
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
