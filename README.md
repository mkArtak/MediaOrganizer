# 📂 Media Organizer

**Media Organizer** is a small utility to help tidy up your collection of photos, videos, and other media files. If you've ever had a folder full of files like `IMG_1234.JPG` or `VID_20220101.mp4` and wished they were neatly organized — this tool is for you.

It scans your media files and organizes them into folders based on their metadata (like date taken). Simple idea, but super handy in practice.

🎉 **Media Organizer is now available on the Microsoft Store!**

Simplify your media file management experience by installing directly from the [Microsoft Store](<LINK_TO_STORE>). Get automatic updates, seamless integration, and an enhanced user experience.

[👉 Install from Microsoft Store](https://apps.microsoft.com/detail/9NNX2GFB7HMQ?hl=en-us&gl=US&ocid=pdpshare)

---

## How to use it?

The app comes in two forms: a command-line utility and a desktop application to run on a Windows PC.

---
## 🖼️ Desktop App in Action

Here’s what the app looks like:

![Media Organizer screenshot](https://github.com/user-attachments/assets/57c73879-0dc1-4ea3-b6c7-bcc312310366)

You can customize many options to control how exactly to organize your media.

![Options](https://github.com/user-attachments/assets/4659af2d-7856-45a3-b3ab-7f8d0c57814e)

### ⚙️ Options Explained

#### 🔤 Folder Naming

| Option                     | Description                                                                                       |
|---------------------------|---------------------------------------------------------------------------------------------------|
| **Movies Root Folder Name** | Base folder name under which all organized video files will be placed (e.g., `Movies`).            |
| **Photos Root Folder Name** | Base folder name under which all organized photo files will be placed (e.g., `Photos`).            |
| **Destination Folder Structure** | Folder hierarchy pattern using placeholders:<br/>- `{Year}` → e.g., `2023`<br/>- `{Month}` → `03`<br/>- `{MonthName}` → `March`<br/>- `{Day}` → `15`<br/><br/>Example: `{Year}/{MonthName}` results in folders like `2023/March`. |

---

#### 🖼 File Type Settings

| Option             | Description                                                                                     |
|--------------------|-------------------------------------------------------------------------------------------------|
| **Photo Extensions** | List of file extensions treated as photo files. Default includes:<br/>`.jpg`, `.jpeg`, `.png`, `.heic`, `.bmp`, `.gif`, `.tiff`, `.tif`, `.webp` |
| **Movie Extensions** | List of file extensions treated as video files. Default includes:<br/>`.mov`, `.mp4`, `.avi`, `.3gp`, `.mpg`, `.mkv`, `.wmv`, `.flv`, `.m4v`, `.webm` |

---

#### 🗃 File Handling Options

| Option                                  | Description                                                                 |
|----------------------------------------|-----------------------------------------------------------------------------|
| ✅ **Delete source files after organizing** | Deletes original files from the source after successful organization.       |
| ⬜ **Skip files that already exist at destination** | Skips files that already exist in the destination folder to prevent duplication. |
| ⬜ **Delete empty folders after organizing** | Cleans up empty directories left behind in the source location.             |

---

# For advanced users

## How to build the app (from source code)?
1. Install .NET 9 if you don't have it yet [Download .NET 9](https://get.dot.net).
2. Clone the repository
   ```bash
   git clone https://github.com/mkArtak/mediaorganizer.git
   cd mediaorganizer
   ```
3. Open the terminal / console window
4. Decide how you'd like to run the app (Command line utility or the Desktop app - see the next chapters).
  - **If you'd like to use the command-line utility**, navigate to the MediaOrganizer.CLI folder in the terminal
  - **If you'd like to use the desktop app**, navigateto the MediaOrganizer folder in the terminal
5. Run `dotnet build` command

This will build the project and produce the app under the `bin/debug` folder of the project that you built.
Navigate to that folder and start the app.

---

## Command line utility
The MediaOrganizer.CLI project can be used to build the command-line utillity that you can use for organizing your media library.
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
---

## 🤝 Contributions

Found a bug? Have an idea? Feel free to open an issue or PR. I'm happy to collaborate.

