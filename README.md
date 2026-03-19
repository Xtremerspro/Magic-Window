# Magic Window

![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![Kinect v2](https://img.shields.io/badge/Kinect_v2-0078D7?style=for-the-badge&logo=windows&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)

> This is the magic window, it tracks your head with a Kinect v2 and updates the perspective of the screen in real time.

## Features

* **Real-Time Head Tracking:** Utilizes the Kinect v2 sensor to capture and map user movement with low latency.
* **Dynamic Perspective:** Shifts the on-screen camera perspective relative to your physical position, creating a 3D holographic "window" effect.
* **Unity Integration:** A fully functional Unity environment ready for immediate testing or further game development.

## Download

You can clone or download the repository directly from GitHub. If the GitHub release bandwidth is exhausted or you prefer a direct download for the heavier build files, you can use the Google Drive mirror below:

* [Download from Google Drive (Mirror)](https://drive.google.com/drive/folders/1KhHwJh0HwTk_gcoTmXWQXEdul18JAhk9?usp=sharing)
## Hardware Requirements

To run this project out of the box, you will need:
* **Xbox One Kinect v2 Sensor**
* **Kinect v2 to USB Adapter**
* **16:9 Monitor** (for the intended aspect ratio and perspective)
* **A decent PC** (capable of handling the heavier 3D scenes)

## Quick Start Guide

Follow these steps to get the Magic Window running immediately:

1. **Install the Drivers:** Download and install the [Kinect for Windows Runtime 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44559) (or the full SDK if you prefer).
2. **Connect the Hardware:** Plug in your Kinect v2 via the USB adapter. Check to make sure the sensor's power indicator lights up.
3. **Position the Camera:** Mount the Kinect dead center on top of your monitor, matching the setup in this reference photo:
   
| Angle | Example |
| :--- | :--- |
| Front View | <img src="Example%20Images/20260319_133551.jpg" width="400"> |
| Top View | <img src="Example%20Images/20260319_133603.jpg" width="400"> |

4. **Initialize Tracking:** Navigate to the `Kinect Tracker` folder and launch `Kinect Tracker.exe`. Wait a moment until the console successfully displays your head's real-time X, Y, and Z coordinates.
5. **Launch the Window:** Open the `Unity Builds` folder and run any of the `Room Window Kinect v6.2.exe` files. Step back, and the perspective will shift!

## Modding & Development

Want to adjust the Kinect positioning logic, build your own custom scenes, or improve my execution? Everything you need is included in the repository:
* **Unity Project Skeleton:** The core Unity setup ready for modification.
* **Blender Source Files (`.blend`):** Original 3D environment assets for tweaking the scenes.
* **Tracker Source Code:** The raw Kinect tracking scripts so you can see exactly how the coordinate math works under the hood. 

Feel free to fork the repo and submit a pull request!

## Demo & Breakdown

Curious to see how it looks in action or how the code comes together? Check out the full breakdown and demo video:

[![YouTube Video](https://img.shields.io/badge/YouTube-FF0000?style=for-the-badge&logo=youtube&logoColor=white)](https://youtube.com/@digiform80085)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
