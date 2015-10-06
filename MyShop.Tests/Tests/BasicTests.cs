using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyShop.Tests
{
    public class BasicTests : AbstractSetup
    {
        public BasicTests(Platform platform)
            : base(platform)
        {
        }

        //        [Test]
        public void AppLaunches()
        {
            app.Screenshot("First screen.");
            app.Repl();
        }
    }
}

