# application-portal
Here is a **clean, step-by-step `README.md`** written so **even a non-technical user can follow it**.
You can **copy–paste this directly**.

---

# CSE Department Application Service Portal

This project contains **two parts**:

1. **API (Backend)** – handles database and business logic
2. **WebApp (Frontend)** – user interface using Razor + Tailwind CSS

Follow the steps below **in order**.

---

## ✅ Requirements (Install First)

Make sure these are installed on your computer:

* **.NET SDK 7 or later**
  [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

* **XAMPP** (for MySQL database)
  [https://www.apachefriends.org/index.html](https://www.apachefriends.org/index.html)

* **Git (optional, but recommended)**
  [https://git-scm.com/downloads](https://git-scm.com/downloads)

---

## 📁 Step 1: Download Tailwind CSS

1. Open **Command Prompt**
2. Go to the **WebApp** folder

```bash
cd WebApp
```

3. Download Tailwind CSS

```bash
curl -sLO https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-windows-x64.exe
```

4. Rename the file

```bash
rename tailwindcss-windows-x64.exe tailwindcss.exe
```

---

## 🗄️ Step 2: Setup Database (MySQL)

1. Open **XAMPP Control Panel**
2. Start **Apache** and **MySQL**
3. Open **phpMyAdmin**
4. Create a database (example name):

```
university_service
```

5. Import the file:

```
university_service.sql
```

---

### 🔧 Database Configuration

Open this file:

```
Api/appsettings.json
```

Update these values if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3307;Database=university_service;User=root;Password=;"
}
```

> ⚠️ If your MySQL port is **3306**, change `Port=3307` to `Port=3306`

---

## 🚀 Step 3: Run the API (Backend)

From the **project root folder**, run:

```bash
dotnet run --project Api/Api.csproj
```

After running, open your browser and visit:

```
http://localhost:5101/swagger/index.html
```

> Your port number may be different.
> If so, copy that port number.

---

## 🌐 Step 4: Update WebApp API Port (If Needed)

If your API port is **not 5101**, open:

```
WebApp/Program.cs
```

Update the API base URL to match your port.

---

## 🎨 Step 5: Run WebApp + Tailwind

1. Open **two Command Prompt windows**
2. In both, go to the **WebApp** folder

```bash
cd WebApp
```

### 🖥️ Terminal 1 – Start Web Server

```bash
dev.bat
```

### 🎨 Terminal 2 – Tailwind CSS Watcher

```bash
tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
```

⚠️ **Do NOT close either window**

---

## 🔄 Useful Commands

* Restart server:
  **Ctrl + R**

* Stop server:
  **Ctrl + C**

---

## 🌍 Access the Application

Open browser and go to:

```
http://localhost:5273
```

(Port may vary slightly)

---

## ✅ You’re Done!

The **CSE Department Application Service Portal** is now running successfully 🎉

If something doesn’t work:

* Check database port
* Ensure both CMD windows are running
* Restart the API and WebApp
