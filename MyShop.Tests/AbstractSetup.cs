using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using System.Collections.Generic;

namespace MyShop.Tests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public abstract class AbstractSetup
    {
        protected IApp app;
        protected Platform platform;

        private static int LocationIndex = 0;
        private static string[] Locations = new string[]
        {
            "Xamarin Inc. Argentina",
            "Xamarin Denmark APS",
            "Xamarin Inc. London",
            "Xamarin Inc. Boston",
            "Xamarin HQ"
        };

        public AbstractSetup(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public virtual void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        public string GetLocationName()
        {
            if (LocationIndex < Locations.Length)
                return Locations[LocationIndex++];
            else
                return Locations[LocationIndex = 0];
        }
    }
}

