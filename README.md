# Library Management System

A C# .NET WinForms Desktop application built using a dynamic code-only layout approach.

## ✨ Features
- **Dynamic UI:** No designer files, completely styled via code.
- **Local Storage:** Data is persisted in JSON files.
- **Architecture:** Clean separation between the UI and backend logic.

## 🚀 How to Build (.exe)
Run this command in the main folder to generate a single executable file:

```bash
dotnet publish LibraryWinForms/LibraryWinForms.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishReadyToRun=true
