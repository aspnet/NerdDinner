using System;
using System.Collections.Generic;

namespace Dinners.Services
{
    public class RSVP
    {
        public string UserName { get; set; }
    }
	public class Dinner
	{
		public Dinner ()
		{
		}

		public int DinnerID { get; set; }
		public string Title { get; set; }
		public DateTime EventDate { get; set; }
		public string Description { get; set; }
		public string UserName { get; set; }
		public string ContactPhone { get; set; }
		public string Country { get; set; }
		public string Address { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public List<RSVP> RSVPs { get; set; }
	}
}

