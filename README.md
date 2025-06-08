# 📂 Media Organizer

**Media Organizer** is a small utility to help tidy up your collection of photos, videos, and other media files. If you've ever had a folder full of files like `IMG_1234.JPG` or `VID_20220101.mp4` and wished they were neatly organized — this tool is for you.

It scans your media files and organizes them into folders based on their metadata (like date taken). Simple idea, but super handy in practice.

🎉 **Media Organizer is now available on the Microsoft Store!**

Simplify your media file management experience by installing directly from the [Microsoft Store](<LINK_TO_STORE>). Get automatic updates, seamless integration, and an enhanced user experience.

[👉 Install from Microsoft Store](https://apps.microsoft.com/detail/9NNX2GFB7HMQ?hl=en-us&gl=US&ocid=pdpshare)

---

## How to use it?

The app comes in two forms: a command-line utility and a desktop application to run on a Windows machine.

---
## 🖼️ Desktop App in Action

Here’s what the app looks like:

![Media Organizer screenshot](https://github.com/user-attachments/assets/57c73879-0dc1-4ea3-b6c7-bcc312310366)

You can customize many options to control how exactly to organize your media.

![Options](https://github.com/user-attachments/assets/ecec7cb6-facf-4697-8037-919712b16cc9)

---

#### 🖼 Media Categories

Categories are a mechanism to differentiate different types of media and give the user ability to organize them differently.
Each category, defines a unique set of file extensions that determine the types of files that will be treated as part of that category.
When defining those in the Category Management UI, separate the extensions with commas `,` and define them in the following pattern: ".jpg, .png, ...".

The Windows app comes with two categories defines - `Movies` and `Photos`. Those are fullly configurable and can even be removed.

---

### ⚙️ Options Explained

The concept of `media` is expandable. Different people may have different expectations from what that media is and what it means to organize it.
That's why the Media Organizer supports custom categories.
Out of the box, the app comes with `Movies` and `Photos` defined as categories. Let's say you want to also organize your music library.
You can do so by adding a new category called `Music` as follows.
1. Expand the `Advanced Options` section
2. Click the `+ Add Category` button
3. Name the new category by populating `Music` in the `Category Name` field.
4. If you want your music library to be organized under a separate root folder, provide a name for that folder in the `Category Root` folder.
5. Populate the list of file extensions that should be considered `music`. Let's say you care about only `.wav` and `.mp3` files.
6. Here what the popualted new category dialog should look like:
![New Music Category](https://github.com/user-attachments/assets/fcc2991d-4c9a-4ad8-8812-25eb25222e57)


#### 🔤 Organizatation structure

The `Destination Folder Structure` field gives the user control over how to structure the organized media within the destination folder.
From a high-level point of view, a given media file is going to be put under the following structure during organization:

**[Destination Root]/[Category Root]/[Destination Folder Structure]**

| Option                     | Description                                                                                       |
|---------------------------|---------------------------------------------------------------------------------------------------|
| **Destination Root** | Path to the folder, under which all the media files will be organized. This is your root level folder, something like `C:\Media\`            |
| **Category Root** | The name of the category-specific folder, under which all media for that category will reside. For your movie collection, this may be called `Movies`. This setting is optional and categories may not define it.            |
| **Destination Folder Structure** | Folder hierarchy pattern using placeholders:<br/>- `{Year}` → e.g., `2023`<br/>- `{Month}` → `03`<br/>- `{MonthName}` → `March`<br/>- `{Day}` → `15`<br/><br/>Example: `{Year}/{MonthName}` results in folders like `2023/March`. |

Note, that the organizer uses each file's original creation date as the parameter for calculating the `Destination Folder Structure`.

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
  --destination-pattern                                      The pattern to use for organizing files in the destination folder. See the `Destination Folder Structure` setting above under the `Organizational Structure` header for details.
  --delete-empty-folders                                     Delete empty folders from the source directory, after moving files.
  --media-extensions                                         The file extensions that will be be organized under the specified destination directory. Use the `".jpg, .png,.pdf"` format.
  --version                                                  Show version information
  -?, -h, --help                                             Show help and usage information
```

Note, that the CLI has no knowledge of categories. It simply takes the list of file extensions as an input and organizes those specific set of files according with the rest of the optinos that are provided.

---

## 🤝 Contributions

Found a bug? Have an idea? Feel free to open an issue or PR. I'm happy to collaborate.

