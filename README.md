# StringMaster

## What is StringMaster

String master is a plugin for Autodesk Civil 3D 2019-2023 which connects/strings CogoPoints together matching the description keys specified.

This project was developed for personal usage but you may find it useful. Please feel free to discuss or contribute.

## Screenshots

![screenshot](https://user-images.githubusercontent.com/79826944/189561659-b80f27b6-5a05-494e-bd4d-81c13668fb47.png)

## Building from source

### 1: Prerequisites

- [Git](https://git-scm.com)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

### 2: Clone the repository.

```ps
git clone https://github.com/puppetsw/StringMaster
```

This will create a local copy of the repository.

### 3: Build the plugin

To build the plugin make sure that you have references to the following Autodesk DLLs from your Autodesk Civil 3D installation directories. 

- accoremgd.dll
- acdbmgd.dll
- acmgd.dll
- AecBaseMgd.dll
- AeccDbMgd.dll

Make sure to set the `Copy Local` property of each Autodesk reference to `False`.

![Screenshot 2022-09-12 115736](https://user-images.githubusercontent.com/79826944/189563239-1f5d09a9-46d4-4deb-95d2-96b2b2cd4e42.png)

## Contributors

Want to contribute to this project? Feel free to open an [issue](https://github.com/puppetsw/StringMaster/issues) or [pull request](https://github.com/puppetsw/StringMaster/pulls).
