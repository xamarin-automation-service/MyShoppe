using System;
using Xamarin.UITest;

namespace MyShop.Tests
{
    public class StartPage : BasePage
    {
        readonly string LocationsButton = "Locations";
        readonly string FeedbackButton = "Leave Feedback";

        public StartPage(IApp app, Platform platform)
            : base(app, platform, "Locations", "Locations")
        {
        }

        public void OpenLocations()
        {
            app.Tap(LocationsButton);
        }

        public void OpenFeedback()
        {
            app.Tap(FeedbackButton);
        }
    }
}

