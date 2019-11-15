# YoloDetectionHoloLens-Unity
Object detection sample using the Yolo framework and HoloLens photo/video sensro stream. C++ project to receive PV sensor frames, and send bounding boxes is made available for use in Unity through [IL2CPP Windows Runtime support](https://docs.unity3d.com/2018.4/Documentation/Manual/IL2CPP-WindowsRuntimeSupport.html). 

Incorporates:
- [HoloLensForCV](https://github.com/microsoft/HoloLensForCV) sample from Microsoft 
- [BoundingBoxUtils-Unity](https://github.com/doughtmw/BoundingBoxUtils-Unity) sample

![Yolo detection sample](https://github.com/doughtmw/YoloDetectionHoloLens-Unity/blob/master/Yolo-Detection-Example.jpg)

## Requirements
- Tested with [Unity 2018.4 LTS](https://unity3d.com/unity/qa/lts-releases
)
- [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/)
- Minimum [RS4](https://docs.microsoft.com/en-us/windows/mixed-reality/release-notes-april-2018), tested with [OS Build 17763.678](https://support.microsoft.com/en-ca/help/4511553/windows-10-update-kb4511553)

## Yolo Detect Sample
### Build HoloLensForCV project and configure YoloDetectionHoloLensUnity Unity project
1. Open HoloLensForCV sample in VS2017 and install included OpenCV nuget package to HoloLensForCV project
2. Build the HoloLensForCV project (x86, Debug or Release) 
3. Copy all output files from HoloLensForCV path (dlls and HoloLensForCV.winmd) to the Assets->Plugins->x86 folder of the YoloDetectionHoloLensUnity project
4. Open YoloDetectionHoloLensUnity Unity project, enter the [IP address of your desktop PC](https://support.microsoft.com/en-ca/help/4026518/windows-10-find-your-ip-address) into the relevant field on the script holder game object
5. Under Unity build settings, switch the platform to Universal Windows Platform and [adjust relevant settings](https://blogs.msdn.microsoft.com/appconsult/2018/11/08/how-to-debug-unity-projects-with-il2cpp-backends-on-the-hololens/). Build project using IL2CPP
6. Open VS solution from Unity build, build then deploy to device

## Build and run ComputeOnDesktopYolo project
1. Open the HoloLensForCV sample and build the ComputeOnDesktopYolo project (x64, Debug or Release) 
2. Deploy project to Local Machine

## Combining the samples to process HoloLens PV camera frames
1. Ensure the HoloLens and PC are on the same network and the HoloLens is currently running the YoloDetectionHoloLensUnity sample
2. When the deployed ComputeOnDesktopYolo desktop app opens, enter the [IP of the HoloLens to connect](https://docs.microsoft.com/en-us/windows/mixed-reality/using-the-windows-device-portal) and click connect
5. Sensor frames from the HoloLens should begin to stream to the desktop and appear in the app window
6. When prompted on the HoloLens, perform the double tap gesture to connect to the host socket and begin receiving information about detected objects in streamed frames (bounding boxes and updated on screen text)

- Be sure to check the Package.appxmanifest files for both the C++ ComputeOnDesktopYolo project and the C# YoloDetectionHoloLensUnity project to ensure permissions are satsified
- Required capabilities are as follows:

**C++**
- Restricted capabilities package:
```xml
  <Package xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10" 
    xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest" 
    xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10" 
    IgnorableNamespaces="uap mp">
```
- Modified capabilities with with new package:
``` xml
  <Capabilities>
    <Capability Name="internetClient" />
    <Capability Name="internetClientServer" />
    <Capability Name="privateNetworkClientServer" />
  </Capabilities>
```

**C#**
- These capabilites should be updated automatically through the Unity project build settings
- Ensure that permissions to internet client, internet client server, private network client server, and webcam are all satisfied
