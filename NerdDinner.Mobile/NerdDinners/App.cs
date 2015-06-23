using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XFApp;

namespace NerdDinners
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
			MainPage = new NavigationPage(new DinnerList());	
			_navigationPage = MainPage as NavigationPage;
        }
		private static NavigationPage _navigationPage;
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
		public static bool IsLoggedIn {
			get { return !string.IsNullOrWhiteSpace(_token); }
		}

		static string _token;
		public static string Token {
			get { return _token; }
		}

		public static void SaveToken(string token)
		{
			_token = token;
		}

		public static Action SuccessfulLoginAction
		{
			get {
				return new Action (() => {
					_navigationPage.Navigation.PopModalAsync();
				});
			}
		}
    }
}
