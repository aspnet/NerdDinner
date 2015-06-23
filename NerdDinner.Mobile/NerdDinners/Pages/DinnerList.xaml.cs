using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dinners.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XFApp;
using System.Collections.Specialized;

namespace NerdDinners
{
    public partial class DinnerList : ContentPage
    {
        protected IDinnerService _dinnerService;

        private void ShowActivityIndicatory(bool show)
        {
            activityIndicator.IsRunning = activityIndicator.IsVisible = show;
        }


        public DinnerList()
        {
            InitializeComponent();

            LoadDinners();

            btnAddDinner.Clicked += (object sender, EventArgs e) =>
            {
                var dinnerAddEdit = new DinnerAddEdit(new Dinner());
                dinnerAddEdit.DinnerSaved += (s, args) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LoadDinners();
                    });
                };
                Navigation.PushAsync(dinnerAddEdit);
            };

            txtSearch.TextChanged += OnSearchTextChanged;

        }

        private void LoadDinners()
        {
            ShowActivityIndicatory(true);
            _dinnerService = new DinnerService();
            activityIndicator.IsRunning = true;
            var dinnersTask = _dinnerService.GetDinnersAsync();
            dinnersTask.ContinueWith(x =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (x.IsFaulted)
                    {
                        DisplayAlert("Error", x.Exception.InnerExceptions[0].Message, "OK");
                    }
                    else
                    {
                        var dinners = x.Result;

                        if (dinners != null)
                        {
                            PlotMarkers(dinners);
                            BindDinnersToListView(dinners);
                        }
                    }
                    ShowActivityIndicatory(false);
                });
            });
        }

        protected void OnSearchTextChanged(object sender, EventArgs e)
        {
            // Perform search on the cached dinners and refresh the markers
            var searchText = (sender as SearchBar).Text;
            var allDinners = _dinnerService.Dinners;
            var filteredDinners = allDinners;
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredDinners = allDinners.Where(x => x.Title.ToLower().Contains(searchText.ToLower())).ToList();
            }

            PlotMarkers(filteredDinners);
            BindDinnersToListView(filteredDinners);
        }

        protected void PlotMarkers(List<Dinner> dinners)
        {

            foreach (Dinner d in dinners)
            {

                var position = new Position(d.Latitude, d.Longitude);

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = d.Title,
                    Address = d.Address,
                    BindingContext = d
                };

                pin.Clicked += (object sender, EventArgs e) =>
                {
                    ViewDinnerDetails((pin.BindingContext as Dinner).DinnerID);
                };
                mapDinners.Pins.Add(pin);

                mapDinners.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        position, Distance.FromMiles(1)));
            }

        }

        protected void BindDinnersToListView(List<Dinner> dinners)
        {
            lvDinners.ItemsSource = dinners;
            lvDinners.ItemTemplate = new DataTemplate(typeof(DinnerCell));

            lvDinners.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                if (e.SelectedItem != null)
                {
                    var selectedDinner = e.SelectedItem as
                        Dinner;

                    ViewDinnerDetails(selectedDinner.DinnerID);
                    lvDinners.SelectedItem = null;
                }
            };
        }

        protected void ViewDinnerDetails(int dinnerID)
        {
            ShowActivityIndicatory(true);
            var dinnersTask = _dinnerService.GetByIDAsync(dinnerID);
            dinnersTask.ContinueWith(x =>
            {
                Device.BeginInvokeOnMainThread(() =>
                                                   {
                                                       if (x.IsFaulted)
                                                       {
                                                           DisplayAlert("Error", x.Exception.InnerExceptions[0].Message, "OK");
                                                       }
                                                       else
                                                       {
                                                           var dinner = x.Result;

                                                           ShowActivityIndicatory(false);
                                                           var dinnerDetails = new DinnerDetails(dinner);
                                                           dinnerDetails.DinnerChanged += (sender, args) =>
                                                           {
                                                               LoadDinners();
                                                           };
                                                           Navigation.PushAsync(dinnerDetails);

                                                       }
                                                   });
            });
        }
    }
}
