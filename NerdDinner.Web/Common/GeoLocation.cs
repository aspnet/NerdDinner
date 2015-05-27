using NerdDinner.Web.Models;
using System;
using System.Linq;
using System.Xml.Linq;

namespace NerdDinner.Web.Common
{
    public static class GeoLocation
    {
        public static void SearchByPlaceNameOrZip(Dinner dinner)
        {
            string url = "http://ws.geonames.org/postalCodeSearch?{0}={1}&maxRows=1&style=SHORT&username=nerddinner";
            int n;
            bool isNumeric = int.TryParse(dinner.Address, out n);
            url = String.Format(url, isNumeric ? "postalcode" : "placename", dinner.Address);

            var result = XDocument.Load(url);

            if (result.Descendants("code").Any())
            {
                var latLong = (from x in result.Descendants("code")
                               select new LatLong
                               {
                                   Lat = (float)x.Element("lat"),
                                   Long = (float)x.Element("lng")
                               }).First();

                dinner.Latitude = latLong.Lat;
                dinner.Longitude = latLong.Long;
            }
        }
    }

    public class LatLong
    {
        public float Lat { get; set; }
        public float Long { get; set; }
    }
}