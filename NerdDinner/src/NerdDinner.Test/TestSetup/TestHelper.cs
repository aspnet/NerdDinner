using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity.ChangeTracking;
using NerdDinner.Web.Models;

namespace NerdDinner.Test.TestSetup
{
    /// <summary>
    /// Test Helper
    /// </summary>
    public class TestHelper
    {
        /// <summary>
        /// Helper method to get dinners
        /// </summary>
        /// <returns>list of dinners</returns>
        public static IQueryable<Dinner> GetDinners()
        {
            var dinners = new List<Dinner>
            {
                GetDinner(1, DateTime.Now, 1),
                GetDinner(2, DateTime.Now.AddDays(2), 2)
            }.AsQueryable();

            return dinners;
        }

        /// <summary>
        /// Helper method to get dinner
        /// </summary>
        /// <param name="dinnerId"></param>
        /// <param name="eventDate"></param>
        /// <param name="userId"></param>
        /// <returns>dinner object</returns>
        public static Dinner GetDinner(int dinnerId, DateTime eventDate, int userId)
        {
            var dinner = new Dinner
            {
                DinnerId = dinnerId,
                UserId = userId,

                Address = "Address " + dinnerId.ToString(),
                Country = "Country " + dinnerId.ToString(),
                Description = "Test Dinner " + dinnerId.ToString(),
                ContactPhone = "123-456-7890",
                EventDate = eventDate,
                Latitude = 10,
                Longitude = 10,
                Title = "Test Dinner Title"
            };

            return dinner;
        }
    }
}
