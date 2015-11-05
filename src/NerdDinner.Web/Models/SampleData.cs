using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Models
{
    public static class SampleData
    {
        public static async Task InitializeNerdDinner(IServiceProvider provider)
        {
            NerdDinnerDbContext dbContext = provider.GetService<NerdDinnerDbContext>();
            UserManager<ApplicationUser> userManager = provider.GetService<UserManager<ApplicationUser>>();

            INerdDinnerRepository repository = new NerdDinnerRepository(dbContext);

            var users = GetUsers();
            var dinners = GetDinners();

            foreach(RegisterViewModel user in users)
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = user.Email
                };

                await userManager.CreateAsync(applicationUser, user.Password);
            }

            foreach (Dinner dinner in dinners)
            {
                await repository.CreateDinnerAsync(dinner);
            }
        }

        private static RegisterViewModel[] GetUsers()
        {
            var users = new RegisterViewModel[]
                {
                    new RegisterViewModel { Email = "user1@xxx.com", Password = "!QAZ2wsx", ConfirmPassword = "!QAZ2wsx" },
                    new RegisterViewModel { Email = "user2@xxx.com", Password = "!QAZ2wsx", ConfirmPassword = "!QAZ2wsx" },
                    new RegisterViewModel { Email = "user3@xxx.com", Password = "!QAZ2wsx", ConfirmPassword = "!QAZ2wsx" },
                };

            return users;
        }

        private static Dinner[] GetDinners()
        {
            var dinners = new Dinner[]
                {
                new Dinner { Title = "Test Dinner One", Address = "Seattle, WA", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner One", EventDate = DateTime.Now, UserName = "user1@xxx.com", Latitude = 47.671798706054688, Longitude = -122.12323760986328 },
                new Dinner { Title = "Test Dinner Two", Address = "Redmond, WA", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Two", EventDate = DateTime.Now.AddDays(1), UserName = "user2@xxx.com", Latitude = 47.671798706054688, Longitude = -122.12323760986328 },
                new Dinner { Title = "Test Dinner Three", Address = "New York", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Three", EventDate = DateTime.Now.AddDays(2), UserName = "user3@xxx.com", Latitude = 40.712784, Longitude = 74.005941 },
                new Dinner { Title = "Test Dinner Four", Address = "Chicago", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Four", EventDate = DateTime.Now.AddDays(3), UserName = "user1@xxx.com", Latitude = 41.878114, Longitude = -87.629798 },
                new Dinner { Title = "Test Dinner Five", Address = "San Francisco", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Five", EventDate = DateTime.Now.AddDays(4), UserName = "user2@xxx.com", Latitude = 33.748995, Longitude = -84.387982 },
                new Dinner { Title = "Test Dinner Six", Address = "Atlanta", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Six", EventDate = DateTime.Now.AddDays(5), UserName = "user3@xxx.com", Latitude = 39.739236, Longitude = -104.990251 },
                new Dinner { Title = "Test Dinner Seven", Address = "Denver", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Seven", EventDate = DateTime.Now.AddDays(6), UserName = "user1@xxx.com", Latitude = 39.952584, Longitude = -75.165222 },
                new Dinner { Title = "Test Dinner Eight", Address = "Pittsburgh", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Eight", EventDate = DateTime.Now.AddDays(7), UserName = "user2@xxx.com", Latitude = 37.774929, Longitude = -122.4194160 },
                new Dinner { Title = "Test Dinner Nine", Address = "Kirkland, WA", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Nine", EventDate = DateTime.Now.AddDays(8), UserName = "user3@xxx.com", Latitude = 31.968599, Longitude = -99.901813 },
                new Dinner { Title = "Test Dinner Ten", Address = "Bellevue, WA", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Ten", EventDate = DateTime.Now.AddDays(9), UserName = "user1@xxx.com", Latitude = 33.748995, Longitude = 74.005941 },
                };

            return dinners;
        }
    }
}