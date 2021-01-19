using System;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;


#nullable disable

namespace AuthAPI.Models
{
    public partial class DataAccess : DbContext
    {
        private string Connection;
        // Details for hashing the passwords
        private static string Salt = "baxScwIOxIX3/XadCtwdMg==";
        private static int HashIterations = 10000;
        private static int HashLength = 32;

        public DataAccess()
        {
        }

        public DataAccess(DbContextOptions<DataAccess> options, IConfiguration configuration)
            : base(options)
        {
            // Load information in from appsettings.json
            Connection = configuration.GetConnectionString("socem1");
        }

        public virtual DbSet<Password> Passwords { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Connection);
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

        public async Task<bool> Validate(User details)
        {
            // Use database stored procedure to check an email and password
            SqlParameter response = new SqlParameter("@Response", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            // Use ExecuteSqlRawAsync and await the result to allow this thread to be returned to the pool while waiting for the database response
            await Database.ExecuteSqlRawAsync("EXEC @Response = ValidateUser @Email, @Password",
                response,
                new SqlParameter("@Email", details.Email),
                new SqlParameter("@Password", HashPassword(details.Password)));
            // Return true if response is 1 (meaning validation was successful) and false otherwise
            return (int)response.Value == 1;
        }

        public void Register(User details, out string responseCode)
        {
            // Try to create new user, and return response code
            // Pass back user id by changing userId property of details

            SqlParameter response = new SqlParameter("@ResponseMessage", SqlDbType.VarChar, 10)
            {
                Direction = ParameterDirection.Output
            };
            Database.ExecuteSqlRaw("EXEC Register @FirstName, @LastName, @Email, @Password, @ResponseMessage OUTPUT",
                new SqlParameter("@FirstName", details.FirstName),
                new SqlParameter("@LastName", details.LastName),
                new SqlParameter("@Email", details.Email),
                new SqlParameter("@Password", HashPassword(details.Password)),
                response);
            // Process response
            // The stored procedure separates the response code and user Id with a comma, so split the string on commas
            string responseString = response.Value.ToString();
            string[] responseComponents = responseString.Split(',');
            if (responseComponents[0] == "200")
            {
                // New user was created successfully, so pass back the new user id in the User object
                details.UserId = Convert.ToInt32(responseComponents[1]);
            }
            // Return response code through out parameter
            responseCode = responseComponents[0];
        }

        public async Task Update(User details, int idToUpdate)
        {
            // Use database stored procedure to change user details
            // Must replace any empty strings with null, as the stored procedure will not update fields with null values supplied
            await Database.ExecuteSqlRawAsync("EXEC UpdateUser @FirstName, @LastName, @Email, @Password, @id",
                new SqlParameter("@FirstName", ReturnDbNullIfEmpty(details.FirstName)),
                new SqlParameter("@LastName", ReturnDbNullIfEmpty(details.LastName)),
                new SqlParameter("@Email", ReturnDbNullIfEmpty(details.Email)),
                new SqlParameter("@Password", ReturnDbNullIfEmpty(HashPassword(details.Password))),
                new SqlParameter("@id", idToUpdate));
        }

        public async Task Delete(int idToRemove)
        {
            // Use DeleteUser stored procedure to delete the user with the given Id
            await Database.ExecuteSqlRawAsync("EXEC DeleteUser @id",
                new SqlParameter("@id", idToRemove));
        }

        public async Task<int> GetUserId(User user)
        {
            // Use GetUserId stored procedure to get the id of a user from an email address
            SqlParameter response = new SqlParameter("@id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            await Database.ExecuteSqlRawAsync("EXEC GetUserId @email, @id OUTPUT",
                new SqlParameter("@email", user.Email),
                response);
            return Convert.ToInt32(response.Value);
        }

        public string HashPassword(string password)
        {
            // Hash a password with ASP.NET's inbuilt PBKDF2 implementation
            // Do not hash and simply return null if a blank string was provided (allowing ReturnDbNullIfEmpty to work)
            if (String.IsNullOrWhiteSpace(password)) return null;
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                Convert.FromBase64String(Salt),
                KeyDerivationPrf.HMACSHA1,
                HashIterations,
                HashLength));
        }

        private object ReturnDbNullIfEmpty(string inputString)
        {
            // Return a DbNull object if the string is empty, and return the string if not empty
            return String.IsNullOrWhiteSpace(inputString) ? (object)DBNull.Value : (object)inputString;
        }
    }
}
