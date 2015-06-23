using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Dinners.Services;

namespace NerdDinners
{
    public delegate void DinnerSavedEventHandler(object sender, EventArgs e);
	public partial class DinnerAddEdit : ContentPage
	{
		string mode = "add";
        const string ADD_EDIT_DINNER_URL = "http://ndinnerdemo.azurewebsites.net/api/dinners";

        
	    public event DinnerSavedEventHandler DinnerSaved;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnDinnerSaved(EventArgs e)
        {
            if (DinnerSaved != null)
                DinnerSaved(this, e);
        }

		public DinnerAddEdit (Dinner dinner)
		{
			InitializeComponent ();
			if (dinner.DinnerID != 0) {
				mode = "edit";
			}

			BindingContext = dinner;

            txtTitle.Text = dinner.Title;
            if ("add".Equals(mode))
            {
                datePicker.Date = DateTime.Today;
                timePicker.Time = DateTime.Today.TimeOfDay;
            }
            else
            {
                datePicker.Date = dinner.EventDate;
                timePicker.Time = dinner.EventDate.TimeOfDay;
            }
            txtDescription.Text = dinner.Description;
            txtOrganizer.Text = dinner.UserName;
		    txtContactPhone.Text = dinner.ContactPhone;
		    txtAddress.Text = dinner.Address;//string.Format ("{0}, {1}", dinner.Address, dinner.Country);
		    txtCountry.Text = dinner.Country;

		    btnSave.Clicked += btnSave_Clicked;
		}

        void btnSave_Clicked(object sender, EventArgs e)
        {
            var restParameters = new Dictionary<string, string>();

            restParameters.Add("title", txtTitle.Text);
            restParameters.Add("description", txtDescription.Text);
            restParameters.Add("eventDate", string.Format("{0}T{1}Z",datePicker.Date.ToString("yyyy-MM-dd"),timePicker.Time.ToString()));
            restParameters.Add("address", txtAddress.Text);
            restParameters.Add("contactPhone", txtContactPhone.Text);

            var responseTask = RestManager.CallRestService(ADD_EDIT_DINNER_URL, "add".Equals(mode) ? HttpMethods.POST : HttpMethods.PUT, restParameters);
            responseTask.ContinueWith(x =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (x.IsFaulted)
                    {
                        DisplayAlert("Error", x.Exception.InnerExceptions[0].Message, "OK");
                    }
                    else
                    {
                        DisplayAlert("Success", "Dinner added successfully.", "OK");
                        OnDinnerSaved(EventArgs.Empty);
                    }
                    this.Navigation.PopAsync();
                    //ShowActivityIndicatory(false);
                });
            });
        }
	}
}

