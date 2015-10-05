using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyShop.Tests
{
    public class AppInitializer
    {
        const string ApkPath = "../../../MyShop.Android/bin/Debug/com.refractored.myshoppe.apk";
        const string AppFile = "../../../MyShop.iOS/bin/iPhoneSimulator/Debug/MyShopiOS.app";
        const string BundleId = "com.refactored.myshoppe";

        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp
					.Android
                    .ApkFile(ApkPath)
					.StartApp();
            }

            return ConfigureApp
				.iOS
//                .AppBundle(AppFile)
//                .InstalledApp(BundleId)
				.StartApp();
        }
    }
}

