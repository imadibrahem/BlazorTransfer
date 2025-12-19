📦 BlazorTransfer

BlazorTransfer is a large-file transfer web app built with Blazor Server and ASP.NET Core Web API.
It supports uploading multiple multi-GB files, tracks upload progress, and generates a shareable download link that delivers all files as a ZIP archive.

✨ Key Features

🚀 Multi-file uploads (GB-scale)

📊 Per-file upload progress

🔗 Shareable download page (/download/{id})

📦 On-the-fly ZIP download

🧹 Automatic cleanup of expired transfers

🚫 Per-file and total upload size limits

⚡ Streaming uploads (no memory buffering)

🏗 Tech Stack

Blazor Server (UI)

ASP.NET Core Web API (Backend)

C# / .NET 8

HTTP streaming & multipart uploads

Background services

🔄 How It Works

Files are uploaded sequentially from the Blazor UI

The API stores files under a generated TransferId

A shareable link (/download/{id}) is created

The download page shows file info and a ZIP download button

ZIP files are streamed directly from disk

Old transfers are automatically deleted

📂 Architecture

Client (Blazor Server) – upload UI, progress tracking, download pages

API (ASP.NET Core) – file storage, ZIP streaming, cleanup worker

Shared models – transfer metadata

🚀 Run Locally
# API
cd BlazorTransfer.Api
dotnet run

# UI
cd BlazorTransfer.Client
dotnet run


UI: http://localhost:6501

API: http://localhost:6500

💡 What This Project Demonstrates

Handling large file uploads in ASP.NET

Streaming data efficiently

Clean separation between UI and API

Background processing with hosted services

Real-world Blazor Server patterns
