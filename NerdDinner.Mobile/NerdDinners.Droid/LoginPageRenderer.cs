using System;
using Xamarin.Forms.Platform.Android;
using NerdDinners;
using NerdDinners.Droid;
using Xamarin.Forms;
using Android.App;
using Xamarin.Auth;


[assembly: ExportRenderer(typeof(LoginPage), typeof(LoginPageRenderer))]
namespace NerdDinners.Droid
{
    public class LoginPageRenderer : PageRenderer
    {
        private bool loginSuccess = false;
        public LoginPageRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            // this is a ViewGroup - so should be able to load an AXML file and FindView<>
            var activity = this.Context as Activity;

            var auth = new OAuth2Authenticator(
                clientId: "432727706889318", // your OAuth2 client id
                           scope: "", // the scopes for the particular API you're accessing, delimited by "+" symbols
                          authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"), // the auth URL for the service
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));// the redirect URL for the service

            /*var auth = new OAuth2Authenticator(
                clientId: "1085994608615-5t6ltc1quqj79kcb9gn6lkputrv2lsrn.apps.googleusercontent.com",
                clientSecret: "HvcJguOE6taumauziFnjofpY",

                // your OAuth2 client id

                scope: "https://www.googleapis.com/auth/userinfo.email",
                // the scopes for the particular API you're accessing, delimited by "+" symbols
                authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
                accessTokenUrl: new Uri("https://accounts.google.com/o/oauth2/token"),
                redirectUrl: new Uri("http://www.kiranbanda.in/search/label/.NET"));
             */

            //auth.ShowUIErrors = false;

            auth.Completed += (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {

                    App.SuccessfulLoginAction.Invoke();

                    // Use eventArgs.Account to do wonderful things
                    App.SaveToken(eventArgs.Account.Properties["access_token"]);
                }
                else
                {
                    // The user cancelled
                }
            };
            auth.Error += (object sender, AuthenticatorErrorEventArgs errorEventArgs) =>
            {
            };

            auth.BrowsingCompleted += (object sender, EventArgs completedEventArgs) =>
            {

            };

            activity.StartActivity(auth.GetUI(activity));
        }

    }
}

