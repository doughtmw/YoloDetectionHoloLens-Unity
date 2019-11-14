# HoloLensForCV-Unity
HoloLens research mode streams made available for use in Unity through [IL2CPP Windows Runtime support](https://docs.unity3d.com/2018.4/Documentation/Manual/IL2CPP-WindowsRuntimeSupport.html). 

Incorporates:
- [HoloLensForCV](https://github.com/microsoft/HoloLensForCV) sample from Microsoft 
- My [BoundingBoxUtils-Unity](https://github.com/doughtmw/BoundingBoxUtils-Unity) sample

![Pv depth camera sample](https://github.com/doughtmw/HoloLensForCV-Unity/blob/master/HoloLens-PvDepth-Example.jpg)

## Requirements
- Tested with [Unity 2018.4 LTS](https://unity3d.com/unity/qa/lts-releases
)
- [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/)
- Minimum [RS4](https://docs.microsoft.com/en-us/windows/mixed-reality/release-notes-april-2018), tested with [OS Build 17763.678](https://support.microsoft.com/en-ca/help/4511553/windows-10-update-kb4511553)

## Yolo Detect Sample
1. Open HoloLensForCV sample in VS2017 and install included OpenCV nuget package to HoloLensForCV project
2. Build the HoloLensForCV project (x86, Debug or Release) 
3. Copy all output files from HoloLensForCV output path (dlls and HoloLensForCV.winmd) to the Assets->Plugins->x86 folder of the HoloLensForCVUnity project
4. Open HoloLensForCVUnity Unity project and build using IL2CPP
5. Back to the HoloLensForCV sample, now build the ComputeOnDesktopYolo project (x64, Debug or Release) and deploy to Local Machine
3. Enter the IP of HoloLens which is on the same network as the companion desktop
6. Open VS solution build from Unity, build then deploy to device

Need to set capabilites of both the C++ project and Unity C# project in Package.appxmanifest file.

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
