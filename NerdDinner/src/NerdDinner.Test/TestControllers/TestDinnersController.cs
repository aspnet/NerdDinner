using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Moq;
using NerdDinner.Test.TestSetup;
using NerdDinner.Web.Controllers;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;
using Xunit;

namespace NerdDinner.Test.TestControllers
{
    /// <summary>
    /// Test Dinners Controller
    /// </summary>
    public class TestDinnersController
    {
        /// <summary>
        /// This function is used to unit test get dinner method
        /// </summary>
        /// <param name="expectedStatusCode">expected status code</param>
        /// <param name="dinnerId">dinner id</param>
        /// <returns></returns>
        [Theory]
        [InlineData(200, 1)] // Dinner Id 1
        [InlineData(404, 10)] // Dinner Id 10
        public async Task TestGetDinnerAsync(int expectedStatusCode, long dinnerId)
        {
            var dinnersController = GetTestDinnersController();
            var result = await dinnersController.GetDinnerAsync(dinnerId);

            switch (expectedStatusCode)
            {
                case 200:
                    var dinner = (Dinner)((JsonResult)result).Value;
                    Assert.Equal(dinner.DinnerId, dinnerId);
                    break;
                case 404:
                    Assert.Equal(((HttpStatusCodeResult)result).StatusCode, expectedStatusCode);
                    break;
            }
        }

        /// <summary>
        /// This function is used to unit test get all dinners method
        /// </summary>
        /// <param name="startInt">start interval</param>
        /// <param name="endInt">end interval</param>
        /// <param name="userId">user id</param>
        /// <param name="q">search query</param>
        /// <param name="sort">sort param</param>
        /// <param name="desc">is descending</param>
        /// <param name="count">expected count</param>
        /// <returns></returns>
        [Theory]
        [InlineData(null, null, 0, null, "dinnerId", false, 2)] // All dinners
        [InlineData(null, null, 0, null, "dinnerId", true, 2)] // All dinners desc sort
        [InlineData(-1d, 1d, 0, null, "dinnerId", false, 1)] // Dinner date today
        [InlineData(1d, 2d, 0, null, "dinnerId", false, 1)] // Dinner date future
        [InlineData(-3d, -2d, 0, null, "dinnerId", false, 0)] // out of bound dates
        [InlineData(null, null, 1, null, "dinnerId", false, 1)] // User Id 1
        [InlineData(null, null, 3, null, "dinnerId", false, 0)] // Non existing user id
        [InlineData(null, null, 0, "1", "dinnerId", false, 1)] // search query one
        [InlineData(null, null, 0, "3", "dinnerId", false, 0)] // non matching search query
        public async Task TestGetAllDinners(double? startInt, double? endInt, long userId, string q, string sort, bool desc, int count)
        {
            DateTime? start = null;
            DateTime? end = null;

            var dinnersController = GetTestDinnersController();
            if (startInt != null && endInt != null)
            {
                var st = (double)startInt;
                var en = (double)endInt;
                start = DateTime.Now.AddDays(st);
                end = DateTime.Now.AddDays(en);
            }

            var result = await dinnersController.GetDinnersAsync(start, end, userId, q, sort, desc);
            Assert.Equal(result.Count(), count);

            if (desc && result.Any())
            {
                Assert.Equal(result.FirstOrDefault().DinnerId, 2);
            }
        }

        /// <summary>
        /// This function is used to unit test Post dinner method
        /// </summary>
        /// <param name="dinnerId">Dinner Id</param>
        [Theory]
        [InlineData(1)] // Dinner Id 1
        public async Task TestAddDinnerAsync(long dinnerId)
        {
            // httpContext response request url need to be mocked for post method
            var httpContext = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            var request = new Mock<HttpRequest>();
            var url = new Mock<IUrlHelper>();

            request.SetupGet(c => c.Host).Returns(new HostString(""));
            request.SetupGet(c => c.Scheme).Returns("");
            response.SetupGet(c => c.Headers["Location"]).Returns("");
            httpContext.SetupGet(c => c.Response).Returns(response.Object);
            httpContext.SetupGet(c => c.Request).Returns(request.Object);

            var dinnersController = GetTestDinnersController(dinnerId);
            dinnersController.ActionContext = new ActionContext(httpContext.Object, new RouteData(), null);
            dinnersController.Url = url.Object;

            var result = await dinnersController.AddDinnerAsync(TestHelper.GetDinner(dinnerId, DateTime.Now, 1));
            var dinner = (Dinner)((JsonResult)result).Value;
            Assert.Equal(dinner.DinnerId, dinnerId);
        }

        /// <summary>
        /// This function is used to unit test Put dinner method
        /// </summary>
        /// <param name="expectedStatusCode">expected status code</param>
        /// <param name="dinnerId">dinner id</param>
        /// <returns></returns>
        [Theory]
        [InlineData(200, 1)] // Dinner Id 1
        [InlineData(404, 10)] // Dinner Id 2
        public async Task TestUpdateDinnerAsync(int expectedStatusCode, long dinnerId)
        {
            var dinnersController = GetTestDinnersController(dinnerId);
            var result = await dinnersController.UpdateDinnerAsync(dinnerId, TestHelper.GetDinner(dinnerId, DateTime.Now, 1));

            switch (expectedStatusCode)
            {
                case 200:
                    var dinner = (Dinner)((JsonResult)result).Value;
                    Assert.Equal(dinner.DinnerId, dinnerId);
                    break;
                case 404:
                    Assert.Equal(((HttpStatusCodeResult)result).StatusCode, expectedStatusCode);
                    break;
            }
        }

        /// <summary>
        /// This function is used to unit test delete dinner method
        /// </summary>
        /// <param name="expectedStatusCode">expected Status code</param>
        /// <param name="dinnerId">dinner Id</param>
        [Theory]
        [InlineData(204, 1)] // Success with not content
        public async Task TestDeleteDinnerAsync(int expectedStatusCode, long dinnerId)
        {
            var dinnersController = GetTestDinnersController();
            var result = await dinnersController.DeleteDinnerAsync(dinnerId) as HttpStatusCodeResult;

            Assert.Equal(result.StatusCode, expectedStatusCode);
        }

        #region Helper Methods

        /// <summary>
        /// Get test dinner controller
        /// </summary>
        /// <returns>dinner controller</returns>
        private DinnersController GetTestDinnersController()
        {
            var mockContext = SetupMockContext();
            var repository = new NerdDinnerRepository(mockContext.Object);
            var dinnersController = new DinnersController(repository);
            return dinnersController;
        }

        /// <summary>
        /// Get test dinner controller for post and put
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <returns>dinner controller</returns>
        private DinnersController GetTestDinnersController(long dinnerId)
        {
            var mockContext = SetupMockContext();
            // For Post and Put Methods
            mockContext.Setup(c => c.UpdateAsync(It.IsAny<Dinner>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(TestHelper.GetDinner(dinnerId, DateTime.Now, 1)));
            mockContext.Setup(c => c.AddAsync(It.IsAny<Dinner>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(TestHelper.GetDinner(dinnerId, DateTime.Now, 1)));

            var repository = new NerdDinnerRepository(mockContext.Object);
            var dinnersController = new DinnersController(repository);
            return dinnersController;
        }

        /// <summary>
        /// Setup mock db context
        /// </summary>
        /// <returns>mock db context</returns>
        private Mock<NerdDinnerDbContext> SetupMockContext()
        {
            var dinners = TestHelper.GetDinners();

            var mockSet = new Mock<DbSet<Dinner>>();
            mockSet.As<IQueryable<Dinner>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Dinner>(dinners.Provider));
            mockSet.As<IQueryable<Dinner>>().Setup(m => m.Expression).Returns(dinners.Expression);
            mockSet.As<IQueryable<Dinner>>().Setup(m => m.ElementType).Returns(dinners.ElementType);
            mockSet.As<IQueryable<Dinner>>().Setup(m => m.GetEnumerator()).Returns(dinners.GetEnumerator());

            var mockConfiguration = new Mock<IConfiguration>();

            var mockContext = new Mock<NerdDinnerDbContext>(mockConfiguration.Object);
            mockContext.Setup(c => c.Dinners).Returns(mockSet.Object);

            return mockContext;
        }

        #endregion
    }
}
