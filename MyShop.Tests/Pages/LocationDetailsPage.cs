using System;
using Xamarin.UITest;
using NUnit.Framework;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyShop.Tests
{
    public class LocationDetailsPage : BasePage
    {
        readonly string CallButton = "Call";
        readonly string NavigateButton = "Navigate";
        readonly string StoreHoursLabel = "Store Hours";

        readonly Query MapView;

        public LocationDetailsPage(IApp app, Platform platform)
            : base(app, platform, "Call", "phone.png")
        {
            if (OnAndroid)
            {
                MapView = x => x.Class("MapRenderer");
            }
            if (OniOS)
            {
                MapView = x => x.Class("MKMapView");
            }
        }

        public void Call()
        {
            app.ScrollTo(CallButton);
            app.Screenshot("Scrolled to call button, tapping");
            app.Tap(CallButton);
            if (OnAndroid)
                app.WaitForNoElement(CallButton);
            app.Screenshot("Call initiated");
        }

        public void Navigate()
        {
            app.ScrollTo(NavigateButton);
            app.Screenshot("Scrolled to navigate button, tapping");
            app.Tap(NavigateButton);
            if (OnAndroid)
                app.WaitForNoElement(NavigateButton);
            app.Screenshot("Navigation initiated");
        }

        public LocationDetailsPage DragMap()
        {
            app.ScrollTo(NavigateButton);
            app.ScrollDown();
            app.ScrollDown();
            app.Screenshot("Map visible");
            var rect = app.Query(MapView)[0].Rect;
            app.DragCoordinates(rect.CenterX, rect.CenterY, rect.X, rect.Y);
            app.Screenshot("Moved map coordinates");

            return this;
        }

        public LocationDetailsPage ConfirmStoreHours()
        {
            app.WaitForElement(StoreHoursLabel);
            app.Screenshot("Store hours present");

            return this;
        }
    }
}

