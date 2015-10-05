using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyShop.Tests
{
    public class LocationTests : AbstractSetup
    {
        public LocationTests(Platform platform)
            : base(platform)
        {
        }

        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            new StartPage(app, platform)
                .OpenLocations();

            new LocationsPage(app, platform)
                .SelectLocation(GetLocationName());
        }

        [Test]
        public void CallTest()
        {
            new LocationDetailsPage(app, platform)
                .Call();
        }

        [Test]
        public void StoreHoursTest()
        {
            new LocationDetailsPage(app, platform)
                .ConfirmStoreHours();
        }

        [Test]
        public void NavigateTest()
        {
            new LocationDetailsPage(app, platform)
                .Navigate();
        }

        [Test]
        public void MapTest()
        {
            new LocationDetailsPage(app, platform)
                .DragMap();
        }
    }
}

