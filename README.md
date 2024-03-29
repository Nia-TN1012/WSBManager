# WSBManager

![Logo](https://raw.githubusercontent.com/Nia-TN1012/WSBManager/master/Assets/AppLogo.png)

**To easier customization of Windows Sandbox!**

WSB Manager can create and edit Windows Sandbox configuration files on the GUI.

![Top](https://raw.githubusercontent.com/Nia-TN1012/WSBManager/master/Assets/MainPage_Light.en-us.PNG)

> This application supports Windows dark mode. The above image is in **Light** mode and the below image is in **Dark** mode.
>
> ![Top-dark](https://raw.githubusercontent.com/Nia-TN1012/WSBManager/master/Assets/MainPage_Dark.en-us.PNG)

## Application summary

|||
|---|---|
|Application name|WSB Manager|
|Version|1.2.16|
|Developer|Nia Tomonaka (@nia_tn1012)|
|Released day|September 13, 2019|
|Last updated day|January 22, 2022|
|Available on|Windows 10 (Version 1803 or later)(*) / 11|
|Using capabilities|Folder and file access|
|Language supported|Japanese (ja, ja-JP), English (en, en-US)|
|Licence|Apache-2.0 Licence|
|GitHub|[https://github.com/Nia-TN1012/WSBManager](https://github.com/Nia-TN1012/WSBManager)|
|Programing languages / Frameworks|C# 7.3 / XAML / .NET Core / .NET Framework 4.7.2 / Windows 10 SDK|
|Using libraries|[Windows Community Toolkit](https://github.com/windows-toolkit/WindowsCommunityToolkit)|
|Development environment|Visual Studio 2022|

> ### **Note**
>
> * This app is compatible with **Windows 10 / 11 desktop with x86 or x64 architecture**. Not supported for ARM architecture or Windows 10 Mobile etc.
> * To launch the Windows sandbox, it requires **Windows 10 / 11 Pro or Enterprise version 1903 or later and hardware support**. Also, enable the Windows sandbox in advance from “Turn Windows features on or off”.

The application can be downloaded from the Windows store.

* [Store page](https://www.microsoft.com/store/apps/9P09B9DSPB95)
* [Microsoft Store App](ms-windows-store://pdp/?productid=9P09B9DSPB95)

# How to use

## Main page

When you start the app, the main page is displayed. The left half of the page is displayed a list of created and imported sandbox configuration items, and the right half of the page is displayed the properties of the item selected in the list.

![Mainpage](https://raw.githubusercontent.com/Nia-TN1012/WSBManager/master/WSBManager/Assets/UserGuide/en-us/MainPageGuide_Light.png)

|No.|Name|Summary|
|:---:|---|---|
|**1**|Add Sandbox Configuration|Navigates to the new creation page of the sandbox configuration item.|
|**2**|Import Sandbox Configuration file|Selects the sandbox configuration file from the dialog and imports it.|
|**3**|Sandbox configuration item name search box|Searches from the list by the name of the sandbox configuration item (forward match). Selecting from the suggestion list selects the corresponding sandbox configuration item.|
|**4**|(Context menu)||
|->|User Guide|Navigates to the User Guide page.|
|->|About|Navigates to the About page.|
|**5**|Sandbox configuration item|Displays sandbox configuration item names and configuration information indicators.|
|**6**|Launch Sandbox|Launches Windows Sandbox from the specified sandbox configuration item.|
|**7**|(Context menu)||
|->|Move up|Moves the specified sandbox configuration item up one level. If at the top of the list, go to the end of the list.|
|->|Move down|Moves the specified sandbox configuration item down by one. If at the end of the list, move to the top of the list.|
|->|Export to file|Exports the specified sandbox configuration item to a file.|
|->|Delete|Deletes the specified sandbox configuration item from the list.|
|**8**|Sandbox configuration item properties|Displays the properties of the sandbox configuration item selected from the list.|
|**9**|Edit Sandbox Configuration|Navigates to the edit page of the sandbox configuration item.|

> ### **Note**
>
> * When launching the sandbox from this app, a console window will be displayed temporarily, because the console app included in this app's package is in the process of launching the Windows Sandbox.
> * If the "Open With" dialog is displayed, select "Windows Sandbox" from there to start it.

## New creation / edit page for configuration items

Creates new or edits sandbox configuration items.

![Mainpage](https://raw.githubusercontent.com/Nia-TN1012/WSBManager/master/WSBManager/Assets/UserGuide/en-us/EditPageGuide_Light.png)

|No.|Name|Summary|
|:---:|---|---|
|**1**|Back|Discards the changes and back to the main page.|
|**2**|Save|Saves the changes and back to the main page.|
|**3**|Name|Enter the name of the sandbox configuration item. This value is used only by this application.|
|**4**|Functions|Select whether to enable or disable the functions on the sandbox.|
|->|vGPU|Select whether to enable or disable the vGPU on the sandbox.|
|->|Networking|Select whether to enable or disable the networking on the sandbox.|
|->|Audio input|Select whether to enable or disable the audio input on the sandbox.|
|->|Video input|Select whether to enable or disable the video input on the sandbox.|
|->|Protected client|Select whether to enable or disable the protected client mode on the sandbox.|
|->|Printer redirection|Select whether to enable or disable the printer redirection on the sandbox.|
|->|Clipboard redirection|Select whether to enable or disable the clipboard redirection on the sandbox.|
|**5**|Mapped Folders|Specify a folder on the host to share with the sandbox. The specified folder is mapped to the desktop on the sandbox.|
|->|Host Folder|Enter the absolute path of the folder on the host. If you press the browse button on the right, the folder selection dialog will appear.|
|->|Read Only|To be read-only the mapped folder on the sandbox select the check box.|
|**6**|Logon Command|Enter the command to be executed when the sandbox starts.|
|**7**|Memory usage specification|Select whether to enable or disable the memory usage specification and specifies amount in MB on the sandbox.|

> ### **Note**
>
> * The mapping folders is validated before saving. If there is a validation error, a dialog is displayed and saving is stopped.
> * Mapped Folder names cannot be duplicated. (If there is a duplicate folder name in the validation check, an error will occur.)

# About WSB Manager

The copyright of WSB Manager is possessed by Chronoir.net.

```
(C)2019-2022 Chronoir.net
```

# Legal Disclaimer

The author and Chronoir.net accept no any responsibility for any obstacles or damages caused by using this application. Please be understanding of this beforehand.

# Release note

* 2022/01/22 (Ver. 1.2.16):
  * Changed the processing method when starting the sandbox from this app and fixed the problem that Windows Sandbox fails to launch from this app.
  * Fixed a bug when overwriting a file with a sandbox configuration.
  * Added backup and restore functions for sandbox configuration list.
* 2022/01/12 (Ver. 1.1.0): Customization items added to the app because new options have been added to the sandbox configuration file item.
* 2019/09/13 (Ver. 1.0.0): First release