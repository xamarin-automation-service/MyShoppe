using System;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;
using System.Linq;
using System.Threading;

namespace MyShop.Tests
{
    public class FeedbackPage : BasePage
    {
        readonly Query StoreField;
        readonly Query ServiceField;
        readonly Query DateField;
        readonly Query RatingField;
        readonly Query NameField;
        readonly Query PhoneField;
        readonly Query DoneButton;
        readonly Query RatingDetailField;
        readonly Query RequestCallBack;
        readonly Query FeedbackField;
        readonly Query ServiceDetailField;
        readonly string SubmitButton = "Submit";
        readonly string OKButton = "OK";

        enum StoreFields {
            XamarinAPS,
            XamarinArgentina,
            XamarinLondon,
            XamarinBoston,
            XamarinHQ
        }

        enum ServiceFields {
            Platform,TestCloud,Insights,Forms, Studio,VisualStudio,Other
        }

        enum Months { 
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public FeedbackPage(IApp app, Platform platform)
            : base(app, platform, "Leave Feedback", "Leave Feedback")
        {
            if (OnAndroid)
            {
                StoreField = x => x.Class("EditText").Index(0);
                ServiceField = x => x.Class("EditText").Index(1);
                DateField = x => x.Class("EditText").Index(2);
                RatingField = x => x.Class("EditText").Index(3);
                NameField = x => x.ClassFull("EntryEditText").Index(0);
                PhoneField = x => x.ClassFull("EntryEditText").Index(1);
                RequestCallBack = x => x.Class("Switch");
                RatingDetailField = x => x.Marked("custom");
                FeedbackField = x => x.Class("EditorEditText");
            }

            if (OniOS)
            {
                StoreField = x => x.ClassFull("_UITextFieldRoundedRectBackgroundViewNeue").Index(0);
                ServiceField = x => x.Class("UITextFieldLabel").Index(1);
                DateField = x => x.Class("UITextFieldLabel").Index(2);
                RatingField = x => x.Class("UITextFieldLabel").Index(3);
                NameField = x => x.Marked("First and Last");
                PhoneField = x => x.Marked("555-555-5555");
                DoneButton = x => x.Marked("Done");
                RatingDetailField = x => x.Class("UIPickerTableViewTitledCell").Index(2);
                RequestCallBack =  x => x.Class("UISwitch");
                FeedbackField = x => x.ClassFull("_UITextContainerView");
            }

            Thread.Sleep(5000);
            app.Screenshot("On Feedback Page");
        }


        public FeedbackPage ChangeStore(string StoreName)
        {
            app.Tap(StoreField);
            app.Screenshot("Store Options Open");

            if (OnAndroid)
            {
                switch (StoreName)
                {
                    case "Xamarin Inc. Argentina":
                        PickerScroll("Android", (int)StoreFields.XamarinArgentina);
                        break;

                    case "Xamarin Denmark APS":
                        PickerScroll("Android", (int)StoreFields.XamarinAPS);
                        break;
                    
                    case "Xamarin Inc. London":
                        PickerScroll("Android", (int)StoreFields.XamarinLondon);
                        break;

                    case "Xamarin Inc. Boston":
                        PickerScroll("Android", (int)StoreFields.XamarinBoston);
                        break;

                    case "Xamarin HQ":
                        PickerScroll("Android", (int)StoreFields.XamarinHQ);
                        break;
                       
                    case "Other":
                    default:
                        break;
                }

                app.Tap(OKButton);
            }

            if (OniOS)
            {
                app.Tap(StoreName);
                app.Tap(DoneButton);
            }

            app.Screenshot("Store Changed!");
            return this;
        }

        public FeedbackPage ChangeService(string ServiceName)
        {
            app.Tap(ServiceField);
            app.Screenshot("Service Picker Open");

            if (OnAndroid)
            {
                var view = app.Query(ServiceDetailField)[0].Rect;

                switch (ServiceName)
                {
                    case "Xamarin Platform":
                        PickerScroll("Android", (int)ServiceFields.Platform);
                        break;

                    case "Xamarin Test Cloud":
                        PickerScroll("Android", (int)ServiceFields.TestCloud);
                        break;

                    case "Xamarin Insights":
                        PickerScroll("Android", (int)ServiceFields.Insights);
                        break;

                    case "Xamarin.Forms":
                        PickerScroll("Android", (int)ServiceFields.Forms);
                        break;

                    case "Xamarin Studio":
                        PickerScroll("Android", (int)ServiceFields.Studio);
                        break;

                    case "Xamarin for Visual Studio":
                        PickerScroll("Android", (int)ServiceFields.VisualStudio);
                        break;

                    case "Other":
                        PickerScroll("Android", (int)ServiceFields.Other);
                        break;

                    default:
                        break;
                }

                app.Tap(OKButton);
            }

            if (OniOS)
            {
                app.Tap(ServiceName);
                app.Tap(DoneButton);
            }

            app.Screenshot("Service Changed");

            return this;
        }

        public FeedbackPage ChangeDate(string Month, string Date, string Year)
        {
            app.Tap(DateField);
            app.Screenshot("Date Picker Open");

            int month = (int)Enum.Parse(typeof(Months), Month, true);
            int year = Int32.Parse(Year);
            int date = Int32.Parse(Date);

            if (OnAndroid)
            {
                Thread.Sleep(5000);
                app.Query(x => x.Id("datePicker").Invoke("updateDate", year, (month-1), date)); 

                if (app.Query("Done").Any())
                    app.Tap("Done");
                else if (app.Query("OK").Any())
                    app.Tap(OKButton);
                else if (app.Query("Set").Any())
                    app.Tap("Set");
            }

            if (OniOS)
            {
                //Month
                app.Query(x => x.Class("UIPickerView").Invoke("selectRow", month, "inComponent", 0, "animated", true));
                app.Tap(Month);

                //Date
                app.Query(x => x.Class("UIPickerView").Invoke("selectRow", date, "inComponent", 1, "animated", true));
                app.Tap(Date);

                //Year
                app.Query(x => x.Class("UIPickerView").Invoke("selectRow", year , "inComponent", 2, "animated", true));
                app.Tap(Year);

                app.Tap(DoneButton);
            }

            app.Screenshot("Date Changed");

            return this;
        }

        public FeedbackPage ChangeRating(string Rating)
        {
            app.Tap(RatingField);

            if (OnAndroid)
            {
                int value = 10 - (Int32.Parse(Rating));
                PickerScrollRating("Android", value);
                app.Tap(OKButton);
            }

            if (OniOS)
            {
                int value = 10 - (Int32.Parse(Rating));
                PickerScrollRating("iOS", value);
                app.Tap(DoneButton);
            }

            app.Screenshot("Rating Changed");

            return this;
        }

        public FeedbackPage AddName(string Name)
        {
            app.Tap(NameField);
            app.EnterText(Name);
            app.DismissKeyboard();
            app.Screenshot("Name Entered");

            return this;
        }

        public FeedbackPage AddPhone(string PhoneNumber)
        {
            app.WaitForElement(PhoneField);
            app.Tap(PhoneField);
            app.EnterText(PhoneNumber);
            app.DismissKeyboard();
            app.Screenshot("Phone Number Entered");

            return this;
        }

        public FeedbackPage RequestCallBackOption(string Option)
        {
            if (Option.Equals("Yes"))
            {
                app.Tap(RequestCallBack);
            }

            app.Screenshot("Request Option Set");

            return this;
        }

        public FeedbackPage AddFeedback(string FeedbackText)
        {
            app.Tap(FeedbackField);
            app.EnterText(FeedbackText);

            if (OnAndroid)
                app.DismissKeyboard();
            if(OniOS)
                app.Tap(DoneButton);

            app.Screenshot("Feedback Text Entered");

            return this;
        }

        public void SubmitFeedback()
        {
            app.Tap(SubmitButton);
            app.WaitForNoElement(SubmitButton);
            app.Screenshot("Submitted the Feedback");
        }

        private void PickerScrollRating(string platform, int count)
        {
            if(platform.Equals("Android"))
            {
                var view = app.Query(RatingDetailField)[0].Rect;
                while (count > 0)
                {
                    app.DragCoordinates(view.X, view.Y, view.CenterX, view.CenterY);
                    count--;
                }
            }

            if (platform.Equals("iOS"))
            {
                while (count > 0)
                {
                    app.Tap(RatingDetailField);
                    count --;
                }
            }
        }

        private void PickerScroll(string platform, int count)
        {
            if(platform.Equals("Android"))
            {
                var view = app.Query(RatingDetailField)[0].Rect;
                while (count > 0)
                {
                    app.DragCoordinates(view.CenterX, view.CenterY, view.X, view.Y);
                    count--;
                }
            }

            if (platform.Equals("iOS"))
            {
                while (count > 0)
                {
                    app.Tap(RatingDetailField);
                    count --;
                }
            }
        }
    }
}

