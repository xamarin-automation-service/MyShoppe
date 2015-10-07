SLN_FILE = "MyShop.sln"

ANDROID_CSPROJ = "MyShop.Android/MyShop.Android.csproj"
IOS_CSPROJ = "MyShop.iOS/MyShop.iOS.csproj"
TEST_CSPROJ = "MyShop.Tests/MyShop.Tests.csproj"

APK_FILE = "MyShop.Android/bin/Debug/com.refractored.myshoppe.apk"
IPA_FILE = "MyShop.iOS/bin/iPhone/Debug/MyShopiOS-1.2.ipa"
DSYM_FILE = "MyShop.iOS/bin/iPhone/Debug/MyShopiOS.app.dSYM"
# ANDROID_KEYSTORE = "debug.keystore"

NUGET_VERSION = "1.1.1.255-dev"
APP_NAME = "My Shoppe"
TEST_ASSEMBLY_DIR = "MyShop.Tests/bin/Debug"

task :default => [:build_android, :build_ios, :build_tests]

task :help do
  puts 'tasks:'
  puts 'help'
  puts 'default => build_android, build_ios, build_tests'
  puts 'build_android => restore_packages'
  puts 'build_ios => restore_packages'
  puts 'build_tests => restore_packages'
  puts 'restore_packages'
  puts 'submit_android[user, api_key, (series, devices)] => build_android, build_tests'
  puts 'submit_ios[user, api_key, (series, devices)] => build_ios, build_tests'
end

task :build_android => [:restore_packages] do
  puts "building Android project with:"
	sh "xbuild #{ANDROID_CSPROJ} /p:Configuration=Debug /t:SignAndroidPackage /verbosity:quiet"
end

task :build_ios => [:restore_packages] do
  puts "building iOS project with:"
  sh "xbuild #{IOS_CSPROJ} /p:Configuration=Debug /p:Platform=iPhone /p:OutputPath='bin/iPhone/Debug/' /verbosity:quiet"
end

task :build_tests => [:restore_packages] do
  puts "building UITest project with:"
	sh "xbuild #{TEST_CSPROJ} /p:Configuration=Debug /verbosity:quiet"
end

task :restore_packages do
  puts "restoring packages with:"
  sh "nuget restore #{SLN_FILE}"
end

task :submit_android, [:user, :api_key, :series, :device_set] => [:build_android, :build_tests] do |t, args|
  verify_args args

  series = args[:series] || "master"
  device_set = args[:device_set] || "fe5e138d" # small

  cmd = "mono packages/Xamarin.UITest.#{NUGET_VERSION}/tools/test-cloud.exe submit #{APK_FILE} #{args[:api_key]} --devices #{device_set} --series '#{series}' --locale en_US --app-name '#{APP_NAME}' --user #{args[:user]} --assembly-dir #{TEST_ASSEMBLY_DIR}"

  puts "uploading Android tests to Test Cloud with:"
  sh cmd
end

task :submit_ios, [:user, :api_key, :series, :device_set] => [:build_ios, :build_tests] do |t, args|
  verify_args args

  series = args[:series] || "master"
  device_set = args[:device_set] || "2f802e3f" # small

  cmd = "mono packages/Xamarin.UITest.#{NUGET_VERSION}/tools/test-cloud.exe submit #{IPA_FILE} #{args[:api_key]} --devices #{device_set} --series '#{series}' --locale en_US --app-name '#{APP_NAME}' --user #{args[:user]} --assembly-dir #{TEST_ASSEMBLY_DIR} --dsym #{DSYM_FILE}"

  puts "uploading iOS tests to Test Cloud with:"
  sh cmd
end

def verify_args(args)
  if args[:user].nil?
    raise "you must specify a user's email address under which to upload" + '\nformat: [user, api_key, (series, device_set)]'
  end
  if args[:api_key].nil?
    raise 'you must specify a Test Cloud API key' + '\nformat: [user, api_key, (series, device_set)]'
  end
  if !args[:series].nil? && args[:device_set].nil?
    raise 'If you specify a series or device set you must specify both. They default to "master" and a small device set.' + '\nformat: [user, api_key, (series, device_set)]'
  end
end

def addMaptoManifest(xml_file)
    xml_text = File.read(xml_file)

    newContent = xml_text.gsub("\t\t<meta-data android:name=\"com.google.android.maps.v2.API_KEY\" android:value=\"@string/GoogleMapsKey\" />\n",
        "\t\t<meta-data android:name=\"com.google.android.maps.v2.API_KEY\" android:value=\"AIzaSyBmRuR-M2PV8bF_ljjAQBNzkzSDpmkStfI\" />\n")

    File.open(xml_file, "w"){|newFile| newFile.puts newContent}
end
