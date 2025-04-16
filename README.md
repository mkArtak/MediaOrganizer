# 📂 Media Organizer

**Media Organizer** is a small utility to help tidy up your collection of photos, videos, and other media files. If you've ever had a folder full of files like `IMG_1234.JPG` or `VID_20220101.mp4` and wished they were neatly organized — this tool is for you.

It scans your media files and organizes them into folders based on their metadata (like date taken). Simple idea, but super handy in practice.

🎉 **Media Organizer is now available on the Microsoft Store!**

Simplify your media file management experience by installing directly from the [Microsoft Store](<LINK_TO_STORE>). Get automatic updates, seamless integration, and an enhanced user experience.

[👉 Install from Microsoft Store](https://apps.microsoft.com/detail/9NNX2GFB7HMQ?hl=en-us&gl=US&ocid=pdpshare)
---

## How to build the app?
1. Clone the repository to your local machine [How To](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository#cloning-a-repository)
2. Install .NET 9 if you don't have it yet [Download .NET 9](https://get.dot.net).
3. Open the terminal / console window
4. Decide how you'd like to run the app (Command line utility or the Desktop app - see the next chapters).
5.**If you'd like to use the command-line utility**, navigate to the MediaOrganizer.CLI folder in the terminal
6.**If you'd like to use the desktop app**, navigateto the MediaOrganizer folder in the terminal
7. Run `dotnet build` command

This will build the project and produce the app under the `bin/debug` folder of the project that you built.

## How to use it?

The app comes in two forms (for now): a command-line utility and a desktop application to run on a Windows PC.

## Command line utility
The MediaOrganizer.CLI project will build the command-line utillity that you can use for organizing your media library.
Here is the quick guide for the options you can use:

```console
C:\MediaOrganizer\MediaOrganizer.CLI.exe /?
Description:
  Organizes media files based on the provided parameters.

Usage:
  MediaOrganizer.CLI [options]

Options:
  --source <source> (REQUIRED)                               The source directory containing media files that need to
                                                             be organized.
  --destination <destination> (REQUIRED)                     The destination directory to organize media files under.
  --remove-source                                            Remove source files after moving. [default: True]
  --skip-existing                                            Skip files if they already exist in the destination
                                                             (comparison is done based on the filename. [default: True]
  --video-subfolder-name <video-subfolder-name>              The name of the subfolder to organize video files under.
                                                             [default: Movies]
  --photos-subfolder-name <photos-subfolder-name>            The name of the subfolder to organize photo files under.
                                                             [default: Photos]
  --image-file-format-patterns <image-file-format-patterns>  The file formats to consider as images. [default:
                                                             .jpg|.jpeg|.png]
  --video-file-format-patterns <video-file-format-patterns>  The file formats to consider as videos. [default:
                                                             .mov|.mp4|.avi|.3gp|.mpg]
  --version                                                  Show version information
  -?, -h, --help                                             Show help and usage information
```

## 🖼️ Desktop App in Action

Here’s what the app looks like:

![Media Organizer screenshot](https://github.com/user-attachments/assets/1429c4cb-9b36-4631-87a3-e96e3c70cd70)

- **Source Root**: Folder where your messy media lives  
- **Destination Root**: Folder where the organized media will live
- **Movies root folder name**: The name of the root folder under which the photos will be organized
- **Photos root folder name**: The name of the root folder under which the photos will be organized
- **Destination Pattern**: Customize your folder structure (e.g. by year, month, day)  
- **Delete on Move**: If checked, files will be *moved* (not copied)  
- **Skip Existing Files**: Prevent overwriting if files already exist  

---

## ✨ Features

- Organizes files using metadata (date taken, created date)
- Customizable folder structure using tokens like `{Year}`, `{Month}`, `{Day}`, `{MonthName}`
- Supports both **copy** and **move** modes
- Option to skip already existing files
- Lightweight and easy to use

---

## 🚀 Getting Started

1. Download and build the project:
   ```bash
   git clone https://github.com/mkArtak/mediaorganizer.git
   cd mediaorganizer
   dotnet build
   ```

2. Run the app:
   ```bash
   dotnet run --project MediaOrganizer
   ```

3. Fill in the fields in the GUI, click **Start**, and let it do its thing!

---

## 🛠 Destination Pattern Tokens

The media will be organized under two root folders (Movies and Photos), following a pattern defined by the user.
The default pattern has the following form: `{Year}/{MonthName}/{Year}-{Month}-{Day}`
You can use the following tokens to define your folder structure:

| Token         | Description                                       |
|---------------|---------------------------------------------------|
| `{Year}`      | Year media was taken (..., 2010, 2025, ...)       |
| `{Month}`     | Month (01, 02, ... 12)                            |
| `{MonthName}` | Full month name (January, February, ... December) |
| `{Day}`       | Day of the month (01, 02, ... 31)                 |

**Example**  
`{Year}/{MonthName}/{Year}-{Month}-{Day}`  
would create folders like:  
`2023/June/2023-06-15/IMG_1234.JPG`

---

## 📜 License

MIT — do whatever makes you happy with it.

---

## 🤝 Contributions

Found a bug? Have an idea? Feel free to open an issue or PR. I'm happy to collaborate.

