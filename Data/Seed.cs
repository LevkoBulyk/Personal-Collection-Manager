using Microsoft.AspNetCore.Identity;
using Personal_Collection_Manager.Data.DataBaseModels;

namespace Personal_Collection_Manager.Data
{
    public class Seed
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRole.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
                if (!await roleManager.RoleExistsAsync(UserRole.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRole.User));

                //Users

                var userStore = serviceScope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
                //var emailStore = serviceScope.ServiceProvider.GetRequiredService<IUserEmailStore<ApplicationUser>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string adminUserEmail = "test1@gmail.com";
                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var user = Activator.CreateInstance<ApplicationUser>();
                    user.UserName = "admin1";
                    await userStore.SetUserNameAsync(user, adminUserEmail, CancellationToken.None);
                    user.Email = adminUserEmail;
                    user.EmailConfirmed = true;
                    //await emailStore.SetEmailAsync(user, adminUserEmail, CancellationToken.None);
                    await userManager.CreateAsync(user, "Test!123");
                    await userManager.AddToRoleAsync(user, UserRole.Admin);
                }

                string appUserEmail = "test2@gmail.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var user = Activator.CreateInstance<ApplicationUser>();
                    user.UserName = "admin2";
                    await userStore.SetUserNameAsync(user, appUserEmail, CancellationToken.None);
                    user.Email = appUserEmail;
                    user.EmailConfirmed = true;
                    //await emailStore.SetEmailAsync(user, appUserEmail, CancellationToken.None);
                    await userManager.CreateAsync(user, "Test!123");
                    await userManager.AddToRoleAsync(user, UserRole.Admin);
                }

                // Topics

                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                List<Topic> topicList = new List<Topic>
                {
                    new Topic { Title = "sports" },
                    new Topic { Title = "coins"},
                    new Topic {Title="antique"},
                    new Topic {Title="cans"},
                    new Topic { Title="Alcohol"},
                    new Topic{Title="Books"},
                    new Topic{Title="Vine"},
                    new Topic{Title="stamp"}
                    };

                context.Topics.AddRange(topicList);

                context.SaveChanges();
            }
        }
    }
}