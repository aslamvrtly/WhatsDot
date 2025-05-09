# 📬 WhatsDotLib


[![Live Website](https://img.shields.io/badge/Live_Website-whatsdot.xyz-blue)](https://whatsdot.xyz)  
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)  
[![Open Source](https://img.shields.io/badge/Open%20Source-Yes-brightgreen)](https://opensource.org/)

**WhatsDotLib** is a powerful and easy-to-use .NET library for automating WhatsApp messages using **WhatsApp Web** inside a **WinForms** application — with **zero cost** and **no third-party API**.

> ✅ Built for developers using WinForms  
> ✅ No API required — completely free  
> ✅ Legal and safe — uses WhatsApp Web  
> ✅ Seamless installation and integration  

Developed by **Muhammed Aslam** & **Shijas Hassain**

---

## ✨ Features

- 🚫 **No API keys needed** – No restrictions or monthly limits
- 🛡 **Legal & compliant** – Uses official WhatsApp Web session
- ⚡ **Fast setup** – Integrate and send messages in minutes
- 💬 **Send messages directly** – Supports any valid phone number
- 🔐 **Login & session management** – Built-in QR scan & logout

---

## 📦 Installation

### Via .NET CLI:
```bash
dotnet add package WhatsDotLib
```

### Or via NuGet Package Manager Console:
```bash
Install-Package WhatsDotLib
```

---

## 🧑‍💻 Usage

### 1. Import the library:
```csharp
using WhatsDotLib;
```

### 2. Initialize WhatsDot:
You need to pass a `WebView2` control from your WinForms form.

```csharp
WhatsDot whatsObj = new WhatsDot(webview2);
```

---

### 3. Available Methods

| Method               | Description                                                 |
|---------------------|-------------------------------------------------------------|
| `checkLogin()`       | Returns "connected", "disconnected", or "waiting"          |
| `connectWhatsapp()`  | Opens WhatsApp Web and shows QR code for login             |
| `logoutWhatsapp()`   | Logs out the current WhatsApp session                      |
| `sendMessage(phone, message)` | Sends a WhatsApp message to the given number     |

---

## 🖥 Requirements

- ✅ .NET Framework or .NET Core (WinForms)
- ✅ Microsoft Edge WebView2 Runtime

---

## 👥 Authors

- [Muhammed Aslam](https://github.com/aslamvrtly)
- [Shijas Hassain](https://github.com/ShijasMkt)

---

## 🔗 Links

- 🌐 Website: [whatsdot.xyz](https://whatsdot.xyz)
- 🔹 NuGet Package: [WhatsDotLib on NuGet](https://www.nuget.org/packages/WhatsDotLib)
- 🔹 GitHub Repository: [WhatsDotLib on GitHub](https://github.com/aslamvrtly/WhatsDot)
- ☕ Support Us: [Buy Me a Coffee](https://buymeacoffee.com/aslamvrtly)
