using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace NerdDinners
{
	public class LoginPage : ContentPage
	{
		public LoginPage ()
		{

		    /*var webView = new WebView()
		    {
		        Source = 
		            new UrlWebViewSource()
		            {
                        Url = "https://accounts.google.com/o/oauth2/auth?client_id=1085994608615-5t6ltc1quqj79kcb9gn6lkputrv2lsrn.apps.googleusercontent.com&redirect_uri=http://www.kiranbanda.in/search/label/.NET&response_type=code&scope=https://www.googleapis.com/auth/userinfo.email&state=abcdef"
		            },
		        VerticalOptions = LayoutOptions.FillAndExpand
		    };

            webView.Navigated += webView_Navigated;
		    webView.Navigating += (sender, args) =>
		    {
		        string a = string.Empty;
		    };
		    Content = new StackLayout()
		    {
		        Children = {webView}
		    };*/

		}

        void webView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var targetUrl = e.Url;
            if (targetUrl.Contains("code"))
            {
                var url = new Uri(targetUrl);
                var parameters =
                    Regex.Matches(url.Query, "([^?=&]+)(=([^&]*))?")
                        .Cast<Match>()
                        .ToDictionary(x => x.Groups[1].Value, x => x.Groups[3].Value);
                var code = parameters["code"];
                DisplayAlert("Code", code, "Ok");
                App.SaveToken(code);
                App.SuccessfulLoginAction.Invoke();
                
            }
        }
	}
}


