using DataAccessLayer.Enums;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            var adminUserId = "019a0ee5-cea2-7ffa-9ebe-27d88ee5d125";
            var hasher = new PasswordHasher<ApplicationUser>();

            builder.HasData(
                new ApplicationUser()
                {
                    Id = adminUserId,
                    Email = "admin@SurveyApp.com",
                    NormalizedEmail = "admin@SurveyApp.com".ToUpper(),
                    EmailConfirmed = true,
                    UserName = "admin",
                    NormalizedUserName = "admin".ToUpper(),
                    PhoneNumber = "01512345678",
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    PasswordHash = hasher.HashPassword(null!, "Admin@123") //Default admin password
                });
        }
    }

    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            // --- Create Admin Role ---
            var adminRoleId = "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111";
            var adminRole = new ApplicationRole
            {
                Id = adminRoleId,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsDefault = false
            };

            // --- Create Default Role ---
            var defaultRoleId = "019a0ee5-cea2-7ffa-9ebe-27d9dad81e83";
            var defaultRole = new ApplicationRole()
            {
                Id = defaultRoleId,
                Name = DefaultRoles.Member,
                NormalizedName = DefaultRoles.Member.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsDefault = true
            };

            builder.HasData(adminRole, defaultRole);
        }
    }

    public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {

            var AdminRoleAssigning = new IdentityUserRole<string>()
            {
                UserId = "019a0ee5-cea2-7ffa-9ebe-27d88ee5d125",
                RoleId = "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111"
            };

            builder.HasData(AdminRoleAssigning);
        }
    }

    public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            var adminRoleId = "019a0ee5-cea2-7ffa-9ebe-27d88ee5d111";
            var defaultRoleId = "019a0ee5-cea2-7ffa-9ebe-27d9dad81e83";

            // Collect role claims dynamically
            var claims = Permissions.All()
                .Select((claim, index) => new IdentityRoleClaim<string>
                {
                    Id = index + 1, // Each claim needs a unique Id
                    RoleId = adminRoleId,
                    ClaimType = Permissions.Type,
                    ClaimValue = claim
                })
                .ToArray();

            builder.HasData(claims);
        }
    }
}
