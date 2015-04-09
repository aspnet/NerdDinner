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

            INerdDinnerRepository repository = new NerdDinnerRepository(dbContext, userManager, null);

            var users = GetUsers();
            var dinners = GetDinners();

            foreach(User user in users)
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = user.UserName
                };

                await repository.UserManager.CreateAsync(applicationUser, user.Password);
            }

            foreach (Dinner dinner in dinners)
            {
                await repository.CreateDinnerAsync(dinner);
                await repository.CreateRsvpAsync(dinner, users[0].UserName);
            }
        }

        private static User[] GetUsers()
        {
            var users = new User[]
                {
                    new User { UserName = "User1", Password = "!QAZ2wsx", ConfirmPassword = "!QAZ2wsx" },
                    new User { UserName = "User2", Password = "!QAZ2wsx", ConfirmPassword = "!QAZ2wsx" },
                    new User { UserName = "User3", Password = "!QAZ2wsx", ConfirmPassword = "!QAZ2wsx" },
                };

            return users;
        }

        private static Dinner[] GetDinners()
        {
            var dinners = new Dinner[]
                {
                new Dinner { Title = "Test Dinner One", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner One", EventDate = DateTime.Now, UserName = "User1", Latitude = 10, Longitude = 100 },
                new Dinner { Title = "Test Dinner Two", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Two", EventDate = DateTime.Now.AddDays(1), UserName = "User2", Latitude = 20, Longitude = 90 },
                new Dinner { Title = "Test Dinner Three", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Three", EventDate = DateTime.Now.AddDays(2), UserName = "User3", Latitude = 30, Longitude = 80 },
                new Dinner { Title = "Test Dinner Four", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Four", EventDate = DateTime.Now.AddDays(3), UserName = "User1", Latitude = 40, Longitude = 70 },
                new Dinner { Title = "Test Dinner Five", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Five", EventDate = DateTime.Now.AddDays(4), UserName = "User2", Latitude = 50, Longitude = 60 },
                new Dinner { Title = "Test Dinner Six", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Six", EventDate = DateTime.Now.AddDays(5), UserName = "User3", Latitude = 60, Longitude = 50 },
                new Dinner { Title = "Test Dinner Seven", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Seven", EventDate = DateTime.Now.AddDays(6), UserName = "User1", Latitude = 70, Longitude = 40 },
                new Dinner { Title = "Test Dinner Eight", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Eight", EventDate = DateTime.Now.AddDays(7), UserName = "User2", Latitude = 80, Longitude = 30 },
                new Dinner { Title = "Test Dinner Nine", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Nine", EventDate = DateTime.Now.AddDays(8), UserName = "User3", Latitude = 90, Longitude = 20 },
                new Dinner { Title = "Test Dinner Ten", Address = "4004 148th Ave SE", ContactPhone = "425-XXX-XXXX", Country = "USA", Description = "Test Dinner Ten", EventDate = DateTime.Now.AddDays(9), UserName = "User1", Latitude = 100, Longitude = 10 },
                };

            return dinners;
        }
    }
}