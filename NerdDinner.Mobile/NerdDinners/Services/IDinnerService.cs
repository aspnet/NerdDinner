using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dinners.Services
{
	public interface IDinnerService
	{
		Task<List<Dinner>> GetDinnersAsync();
		Task<Dinner> GetByIDAsync(int dinnerID);
		List<Dinner> Dinners { get; }
	}
}

