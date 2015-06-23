using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Dinners.Services
{
    public class DinnerService : IDinnerService
    {
        protected static readonly string DINNERS_URL = "https://nerddinnertest.azurewebsites.net/api/dinners";
		protected static readonly string DINNER_BY_ID_URL = "https://nerddinnertest.azurewebsites.net/api/dinners/{0}";
        public DinnerService()
        {
        }

		public List<Dinner> Dinners {
			get;
			private set;
		}

        #region IDinnerService implementation

        public async Task<List<Dinner>>  GetDinnersAsync()
        {
            var responseAsString = await RestManager.CallRestService(DINNERS_URL, "GET", null);

            var jsonSerializer = new JsonSerializer();
            List<Dinner> listOfdinners = (List<Dinner>)jsonSerializer.Deserialize(new StringReader(responseAsString), typeof(List<Dinner>));
			//KB: Let's cache dinners to avoid a network call (esp. for search)
			Dinners = listOfdinners;
			return listOfdinners;
        }

		public async Task<Dinner> GetByIDAsync(int dinnerID)
        {
			var responseAsString = await RestManager.CallRestService(string.Format(DINNER_BY_ID_URL,dinnerID), "GET", null);

			var jsonSerializer = new JsonSerializer();
			Dinner dinner = (Dinner)jsonSerializer.Deserialize(new StringReader(responseAsString), typeof(Dinner));
			return dinner;
        }

        #endregion
    }
}

