using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Fingerprint.Abstractions;
using Xamarin.Forms;
using Plugin.Fingerprint;
using SMS.Fingerprint.Sample;

namespace SMS.Fingerprint.Sample
{
    public class MainView1 : ContentPage
    {
        private CancellationTokenSource _cancel;
        Switch swAutoCancel = null;
        Label lblStatus = null;
        public MainView1()
        {

            var childStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal

            };

            var button1 = new Button
            {
                Text = "OnAuthenticate"
            };
            button1.Clicked += OnAuthenticate;
            var button2 = new Button
            {
                Text = "Authenticate localized"
            };
            button2.Clicked += OnAuthenticateLocalized;
            lblStatus = new Label { };
            swAutoCancel = new Switch
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            var stackMain = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(10),
                Children = {
                    childStack,
                    button1,
                    button2,
                    lblStatus
                }
            };
            this.Content = stackMain;
        }
        private async void OnAuthenticate(object sender, EventArgs e)
        {
            await AuthenticateAsync("Prove you have fingers!");
        }

        private async void OnAuthenticateLocalized(object sender, EventArgs e)
        {
            await AuthenticateAsync("Beweise, dass du Finger hast!", "Abbrechen", "Anders!");
        }

        private async Task AuthenticateAsync(string reason, string cancel = null, string fallback = null)
        {
            _cancel = swAutoCancel.IsToggled ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
            lblStatus.Text = "";

            var dialogConfig = new AuthenticationRequestConfiguration(reason)
            {
                CancelTitle = cancel,
                FallbackTitle = fallback
                //,AllowAlternativeAuthentication = swAllowAlternative.IsToggled
            };

            var result = await CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

            await SetResultAsync(result);
        }

        private async Task SetResultAsync(FingerprintAuthenticationResult result)
        {
            if (result.Authenticated)
            {
                await Navigation.PushAsync(new SecretView());
            }
            else
            {
                lblStatus.Text = $"{result.Status}: {result.ErrorMessage}";
            }
        }

    }
}
