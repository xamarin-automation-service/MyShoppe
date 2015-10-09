require "benchmark"
require "date"

### PROPERTIES TO SET
SLN_FILE = "MyShop.sln"

ANDROID_DIR = "MyShop.Android"
IOS_DIR = "MyShop.iOS"
TEST_DIR = "MyShop.Tests"

APK_FILE = "MyShop.Android/bin/Debug/com.refractored.myshoppe.apk"
IPA_FILE = "MyShop.iOS/bin/iPhone/Debug/MyShopiOS-1.2.ipa"
DSYM_FILE = "MyShop.iOS/bin/iPhone/Debug/MyShopiOS.app.dSYM"

# ANDROID_KEYSTORE = "debug.keystore"

NUGET_VERSION = "1.1.1.255-dev"
APP_NAME = "My Shop"
### END

task :default => ['build:android', 'build:ios', 'build:tests']

desc "Get more information tasks and variables"
task :help do
  puts 'Available tasks:'
  puts 'help'
  puts 'default => build:android, build:ios, build:tests'
  puts 'build:android => build:restore_packages'
  puts 'build:ios => build:restore_packages'
  puts 'build:tests => build:restore_packages'
  puts 'build:restore_packages'
  puts 'submit:android[user, api_key, (series, device_set)] => build:android, build:tests'
  puts 'submit:ios[user, api_key, (series, device_set)] => build:ios, build:tests'
  puts 'clean'
  puts
  puts 'Optional environment variables:'
  puts 'USER_ACCOUNT'
  puts 'API_KEY'
  puts 'SERIES'
  puts 'DEVICE_SET'
  puts 'CATEGORY'
  puts 'FIXTURE'
end

desc "Removes bin and obj directories for each Android, iOS, and test projects."
task :clean do
  [ANDROID_DIR, IOS_DIR, TEST_DIR].each do |dir|
    rm_rf "#{dir}/bin"
    rm_rf "#{dir}/obj"
  end
end

namespace :build do
  desc "Builds the Android project"
  task :android => [:restore_packages] do
    puts "building Android project with:"
    time = time_cmd "xbuild #{ANDROID_DIR}/*.csproj /p:Configuration=Debug /t:SignAndroidPackage /verbosity:quiet /flp:LogFile=build_android.log" # /verbosity:quiet
    size = (File.size(APK_FILE)/1000000.0).round(1)
    log_data "Android", time, size, "build_android.log"
  end

  desc "Builds the iOS project"
  task :ios => [:restore_packages] do
    puts "building iOS project with:"
    time = time_cmd "xbuild #{IOS_DIR}/*.csproj /p:Configuration=Debug /p:Platform=iPhone /p:OutputPath='bin/iPhone/Debug/' /verbosity:quiet /flp:LogFile=build_ios.log" # /verbosity:quiet
    size = (File.size(IPA_FILE) / 1000000.0).round(1)
    log_data "iOS", time, size, "build_ios.log"
  end

  desc "Builds the test project"
  task :tests => [:restore_packages] do
    puts "building UITest project with:"
  	sh "xbuild #{TEST_DIR}/*.csproj /p:Configuration=Debug /verbosity:quiet"
  end

  desc "Restores packages for all projects"
  task :restore_packages do
    puts "restoring packages with:"
    sh "nuget restore #{SLN_FILE}"
  end

  def time_cmd(cmd)
    time = Benchmark.realtime do
      sh cmd
    end
    min = (time / 60).to_i.to_s
    sec = (time % 60).to_i.to_s
    sec = sec.length < 2 ? "0" + sec : sec
    return "#{min}:#{sec}"
  end

  def log_data(platform, time, size, log_file)
    date = DateTime.now.strftime("%m/%d/%Y %I:%M%p")
    version = /\d+\.\d+\.\d+\.\d+/.match(`mdls -name kMDItemVersion /Applications/Xamarin\\ Studio.app`)
    user = /\w+$/.match(ENV['HOME'])[0].capitalize

    tail = `tail -n 6 #{log_file}`
    warnings = /(\d+) Warning\(s\)/.match(tail).captures[0]
    errors = /(\d+) Error\(s\)/.match(tail).captures[0]

    puts "*** date: #{date}"
    puts "*** platform: #{platform}"
    puts "*** origin: #{user}"
    puts "*** version: #{version}"
    puts "*** size (MB): #{size}"
    puts "*** time: #{time}"
    puts "*** warnings: #{warnings}"
    puts "*** errors: #{errors}"

    puts "#{date}, #{platform}, #{user}, #{version}, #{size}, #{time}, #{warnings}, #{errors}"
  end

  def addMaptoManifest(xml_file)
      xml_text = File.read(xml_file)

      newContent = xml_text.gsub("\t\t<meta-data android:name=\"com.google.android.maps.v2.API_KEY\" android:value=\"@string/GoogleMapsKey\" />\n",
          "\t\t<meta-data android:name=\"com.google.android.maps.v2.API_KEY\" android:value=\"AIzaSyBmRuR-M2PV8bF_ljjAQBNzkzSDpmkStfI\" />\n")

      File.open(xml_file, "w"){|newFile| newFile.puts newContent}
  end
end

namespace :submit do
  desc "Submits Android app to Test Cloud, \"user_account\" and \"api_key\" are required"
  task :android, [:user_account, :api_key, :series, :device_set] => ['build:android', 'build:tests'] do |t, args|
    args = verify_args args, "fe5e138d" # small device set

    puts "uploading Android tests to Test Cloud with:"
    submit_file_with_extra_params APK_FILE, args
  end

  desc "Submits iOS app to Test Cloud, \"user_account\" and \"api_key\" are required"
  task :ios, [:user_account, :api_key, :series, :device_set] => ['build:ios', 'build:tests'] do |t, args|
    args = verify_args args, "2f802e3f" # small device set
    extras = "--dsym #{DSYM_FILE}"

    puts "uploading iOS tests to Test Cloud with:"
    submit_file_with_extra_params IPA_FILE, args, extras
  end

  def submit_file_with_extra_params(file, args, extras="")
    cmd = "mono packages/Xamarin.UITest.#{NUGET_VERSION}/tools/test-cloud.exe submit #{file} #{args[:api_key]} --devices #{args[:device_set]} --series '#{args[:series]}' --locale en_US --app-name '#{APP_NAME}' --user #{args[:user_account]} --assembly-dir #{TEST_DIR}/bin/Debug"
    cmd += " #{extras}"

    if !ENV['CATEGORY'].nil?
      cmd += " --category #{ENV['CATEGORY']}"
    end
    if !ENV['FIXTURE'].nil?
      cmd += " --fixture #{ENV['FIXTURE']}"
    end
    sh cmd
  end

  def verify_args(args, default_device_set)
    newArgs = {}
    if args[:user_account].nil? && ENV['USER_ACCOUNT'].nil?
      error = "\nERROR: You must specify a user's email address under which to upload."
      error += "\nFormat: [user, api_key, (series, device_set)]"
      error += "\nOr set the USER_ACCOUNT environment variable"
      raise error
    else
      newArgs[:user_account] =  args[:user_account] || ENV['USER_ACCOUNT']
    end
    if args[:api_key].nil? && ENV['API_KEY'].nil?
      error = "\nERROR: You must specify a Test Cloud API key."
      error += "\nFormat: [user, api_key, (series, device_set)]"
      error += "\nOr set the API_KEY environment variable"
      raise error
    else
      newArgs[:api_key] = args[:api_key] || ENV['API_KEY']
    end
    if !args[:series].nil? && args[:device_set].nil?
      error = "\nERROR: If you specify a series or device set you must specify both."
      error += "\nFormat: [user, api_key, (series, device_set)]"
      error += "\nThey default to \"master\" and a small device set"
      raise error
    else
      newArgs[:series] = args[:series] || ENV['SERIES'] || "master"
      newArgs[:device_set] = args[:device_set] || ENV['DEVICE_SET'] || default_device_set
    end
    return newArgs
  end
end
