# Deployment Instructions

## Download Unity

1. This project uses Unity 2020.3.12f1, make sure to use this specific version to recreate the mobile app.
2. Download Unity 2020.3.12f1 from the [Unity archive](https://unity3d.com/get-unity/download/archive). Select 'Unity Installer' from the dropdown menu for your chosen operating system. Do NOT select Unity Hub. <br><br> 
 ![image](Images/Download_Unity.png) <br><br>
3. Select the following components during installation. This project is built for Android and hence only requires "Android Build Support". However, if you are on a macOS system you can also select "iOS Build Support" to build for iOS devices. <br><br>
![image](Images/component_selection.png) <br><br>

## Open Project and Build

1. After installation, open Unity and click on "Open Project". Select the folder where you cloned this repo.
2. Make sure Unity's build platform is set to Android by going to "File->Build Settings". The Android option should have the Unity logo next to it. If not, select "Android" and then click on "Switch Platform" in the bottom-right corner. <br><br>
   ![image](Images/build_settings.png) <br><br>
3. Click on "Build" in the bottom-right corner and Unity will generate a .apk file. <br><br>
   ![image](Images/build.png) <br><br>
4. Install .apk on an Android device. Enjoy!


