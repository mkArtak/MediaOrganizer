# 📂 Media Organizer

**Media Organizer** is a small utility I built to help tidy up your collection of photos, videos, and other media files. If you've ever had a folder full of files like `IMG_1234.JPG` or `VID_20220101.mp4` and wished they were neatly organized — this tool is for you.

It scans your media files and organizes them into folders based on their metadata (like date taken). Simple idea, but super handy in practice.

---

## 🖼️ App in Action

Here’s what the app looks like:

![Media Organizer screenshot](https://github.com/user-attachments/assets/4994c9b8-62b2-4b8c-b1f6-e8b6cae6114f)

- **Source Root**: Folder where your messy media lives  
- **Destination Root**: Folder where the organized magic happens  
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

You can use the following tokens to define your folder structure:

| Token         | Description            |
|---------------|------------------------|
| `{Year}`      | Year media was taken   |
| `{Month}`     | Month number (01–12)   |
| `{MonthName}` | Full month name        |
| `{Day}`       | Day of the month       |

Example:  
`{Year}/{MonthName}/{Year}-{Month}-{Day}`  
would create folders like:  
`2023/June/2023-06-15/IMG_1234.JPG`

---

## 📜 License

MIT — do whatever makes you happy with it.

---

## 🤝 Contributions

Found a bug? Have an idea? Feel free to open an issue or PR. I'm happy to collaborate.

