using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyShop.Tests
{
    public class FeedbackTests : AbstractSetup
    {
        public FeedbackTests(Platform platform)
            : base(platform)
        {
        }

        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            new StartPage(app, platform)
                .OpenFeedback();
        }


        [Test]
        public void AddFeedbackSimpleTest()
        {
            new FeedbackPage(app, platform)
                .AddName("First Lastname")
                .AddPhone("1234567890")
                .RequestCallBackOption("Yes")
                .AddFeedback("Test Feedback Check 1.")
                .SubmitFeedback();
        }

        [Test]
        public void AddFeedbackTest2()
        {
            new FeedbackPage(app, platform)
                .ChangeStore("Xamarin Inc. London")
                .ChangeService("Xamarin Insights")
                .ChangeDate("February", "28", "2019")
                .ChangeRating("5")
                .AddName("Last Firstname")
                .AddPhone("1234567890")
                .RequestCallBackOption("Yes")
                .AddFeedback("Test Feedback Check 2")
                .SubmitFeedback();
        }
    }
}

