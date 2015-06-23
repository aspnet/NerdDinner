using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Dinners.Services;
using Xamarin.Forms.Maps;

namespace NerdDinners
{
	public partial class DinnerDetails : ContentPage
	{
		protected readonly string currentUser = "user1@xxx.com";

	    private const string RSVP_URL = "http://ndinnerdemo.azurewebsites.net/api/rsvp";
        private const string DELETE_DINNER_URL = "http://ndinnerdemo.azurewebsites.net/api/dinners/{0}";
        public event DinnerSavedEventHandler DinnerChanged;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnDinnerChanged(EventArgs e)
        {
            if (DinnerChanged != null)
                DinnerChanged(this, e);
        }

		public DinnerDetails (Dinner dinner)
		{
			InitializeComponent ();

			BindingContext = dinner;
			lblWhen.SetBinding (Label.TextProperty, "EventDate");

			lblWhere.Text = string.Format ("{0}, {1}", dinner.Address, dinner.Country);

			lblDescription.SetBinding (Label.TextProperty, "Description");
			lblOrganizer.SetBinding (Label.TextProperty, "UserName");

		    lvRSVP.ItemsSource = dinner.RSVPs;

			var position = new Position(dinner.Latitude, dinner.Longitude);

			var pin = new Pin
			{
				Type = PinType.Place,
				Position = position,
				Label = dinner.Title,
				Address = dinner.Address
			};

			dinnerMap.Pins.Add(pin);

			dinnerMap.MoveToRegion(
				MapSpan.FromCenterAndRadius(
					position, Distance.FromMiles(1)));

			/*if (currentUser.Equals (dinner.UserName, StringComparison.OrdinalIgnoreCase)) {
				btnEdit.IsVisible = btnDelete.IsVisible = true;
				btnRSVP.IsVisible = false;

			} else {
				btnEdit.IsVisible = btnDelete.IsVisible = false;
				btnRSVP.IsVisible = true;
                
			}*/
            btnEdit.Clicked += btnEdit_Clicked;
            btnDelete.Clicked += btnDelete_Clicked;
            btnRSVP.Clicked += btnRSVP_Clicked;
		}

        void btnRSVP_Clicked(object sender, EventArgs e)
        {
            var parameters = new Dictionary<string,string>();
            var theDinner = BindingContext as Dinner;
            parameters.Add("dinnerID", theDinner.DinnerID.ToString());
            var t = RestManager.CallRestService(RSVP_URL, HttpMethods.POST, parameters, null, "application/x-www-form-urlencoded");

            t.ContinueWith(x =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (x.IsFaulted)
                    {
                        DisplayAlert("Error", x.Exception.InnerExceptions[0].Message, "OK");
                    }
                    else
                    {
                        DisplayAlert("Success", "RSVP is successful.", "OK");
                        OnDinnerChanged(EventArgs.Empty);
                    }
                    this.Navigation.PopAsync();
                    //ShowActivityIndicatory(false);
                });
            });
        }

        void btnDelete_Clicked(object sender, EventArgs e)
        {
            var theDinner = BindingContext as Dinner;
            string url = string.Format(DELETE_DINNER_URL, theDinner.DinnerID.ToString());
            var t = RestManager.CallRestService(url, HttpMethods.DELETE, null);

            t.ContinueWith(x =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (x.IsFaulted)
                    {
                        DisplayAlert("Error", x.Exception.InnerExceptions[0].Message, "OK");
                    }
                    else
                    {
                        DisplayAlert("Success", "Dinner deleted successfully", "OK");
                        OnDinnerChanged(EventArgs.Empty);
                    }
                    this.Navigation.PopAsync();
                    //ShowActivityIndicatory(false);
                });
            });
        }

        void btnEdit_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new DinnerAddEdit(BindingContext as Dinner));
        }
	}
}

