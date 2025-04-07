# 📂 Media Organizer

**Media Organizer** is a small utility I built to help tidy up your collection of photos, videos, and other media files. If you've ever had a folder full of files like `IMG_1234.JPG` or `VID_20220101.mp4` and wished they were neatly organized — this tool is for you.

It scans your media files and organizes them into folders based on their metadata (like date taken). Simple idea, but super handy in practice.

---

## 🖼️ App in Action

Here’s what the app looks like:

![Media Organizer screenshot](https://github.com/user-attachments/assets/2560a563-8fd1-4898-a22a-43be96ad32dd)

- **Source Root**: Folder where your messy media lives  
- **Destination Root**: Folder where the organized magic happens
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

