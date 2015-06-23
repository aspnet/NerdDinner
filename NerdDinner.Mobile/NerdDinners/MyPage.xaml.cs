using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Dinners.Services;

namespace XFApp
{
	public partial class MyPage : ContentPage
	{
		//IDinnerService _dinnerService;

		public MyPage ()
		{
			InitializeComponent ();

			//_dinnerService = new DinnerService ();
		    var dinners = new List<Dinner>();//_dinnerService.GetDinners ();

			PlotMarkers (dinners);
			BindDinnersToListView (dinners);

			/*var position = new Position(17.3259099,78.5608401); // Latitude, Longitude
			var pin = new Pin {
				Type = PinType.Place,
				Position = position,
				Label = "My Sweet Home",
				Address = "Mothinagar"
			};

			pin.Clicked += (object sender, EventArgs e) => {
				DisplayAlert("Info","You clicked on my home :)","Yahoo!");
			};
			myMap.Pins.Add(pin);

			myMap.MoveToRegion(
				MapSpan.FromCenterAndRadius(
					position, Distance.FromMiles(1)));


			// List view

			_personManager = new PersonManager ();
			lvPersons.ItemsSource = _personManager.Persons;
			lvPersons.ItemTemplate = new DataTemplate (typeof(PersonCell));

			lvPersons.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
				var selectedPerson = e.SelectedItem as Person;
				Navigation.PushAsync (new PersonDetail(selectedPerson));
			};*/
		}

		void PlotMarkers (List<Dinner> dinners)
		{

			dinners.ForEach (d => {

				var position = new Position (d.Latitude, d.Longitude);

				var pin = new Pin {
					Type = PinType.Place,
					Position = position,
					Label = d.Title,
					Address = d.Address
				};

				pin.Clicked += (object sender, EventArgs e) => {
					Navigation.PushAsync (new PersonDetail(null));
				};
				myMap.Pins.Add (pin);

				myMap.MoveToRegion (
					MapSpan.FromCenterAndRadius (
						position, Distance.FromMiles (1)));
			});
		}

		void BindDinnersToListView (List<Dinner> dinners)
		{
			lvPersons.ItemsSource = dinners;
			lvPersons.ItemTemplate = new DataTemplate (typeof(PersonCell));

			lvPersons.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
				var selectedPerson = e.SelectedItem as 
					Dinner;
				Navigation.PushAsync (new PersonDetail(null));
			};
		}
	}
}

