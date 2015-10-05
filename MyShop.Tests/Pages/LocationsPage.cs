using System;
using Xamarin.UITest;

namespace MyShop.Tests
{
    public class LocationsPage : BasePage
    {
        public LocationsPage(IApp app, Platform platform)
            : base(app, platform, x => x.Class("ConditionalFocusLayout"), x => x.Class("UITableViewCellContentView"))
        {
        }

        public void SelectLocation(string name)
        {
            app.ScrollTo(name);
            app.Screenshot($"Found {name}, tapping");
            app.Tap(name);
        }
    }
}

