# 🎵 Rurina Bluetooth Audio Receiver

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Version](https://img.shields.io/badge/Version-1.1-green?style=for-the-badge)

![Visitor Badge](https://api.visitorbadge.io/api/VisitorHit?user=Nattapat2871&repo=Rurina-Bluetooth-Audio-Receiver&countColor=%237B1E7A)

A lightweight, minimalist Windows application that transforms your PC into a Bluetooth Audio Receiver (A2DP Sink). Seamlessly stream music, videos, or game audio from your smartphone or tablet directly to your computer's speakers.

<img width="354" height="407" alt="image" src="https://github.com/user-attachments/assets/482e2a6c-b8f3-4f97-8ccf-5c809ff7792c" />


## ✨ Features

* **One-Click Audio Routing:** Easily establish a direct audio connection with your paired Bluetooth devices.
* **⚡ Smart Auto-Connect:** The application remembers your last successfully connected device and automatically attempts to reconnect upon the next launch.
* **🔄 Automatic Updates:** The app automatically checks for the latest version from GitHub on startup. You can choose to update immediately or skip the version.
* **🌙 Minimalist Dark UI:** A clean, custom-painted dark mode interface designed to be distraction-free and easy on the eyes.
* **📥 Minimize to Tray:** Click the minimize button to hide the app into the system tray (bottom-right corner) so it keeps running quietly in the background.
* **🚀 Run on Startup:** A convenient checkbox allows you to set the app to open automatically every time you turn on your PC.
* **🛡️ Single Instance Protection:** Prevents opening multiple copies. Opening the app again will bring the existing window to the front.
* **🐞 Built-in Crash Logger:** Displays a custom error window with detailed logs if something goes wrong.

## 💻 System Requirements

* **OS:** Windows 10 (Version 2004 or newer) or Windows 11.
* **Hardware:** A working Bluetooth adapter on your PC.
* **Framework:** .NET 10 Desktop Runtime.

## 🚀 How to Use

### 🛑 Prerequisite (Must Read First)
**Please pair your smartphone's Bluetooth with your computer completely in the standard Windows Bluetooth Settings BEFORE launching the program.**

### Option 1: Using the Pre-built Release (Recommended)
1. Ensure your phone is already paired with your PC.
2. Download the latest `.exe` file from the [Releases](../../releases) page.
3. Run `RurinaAudio-Receiver.exe`. The app will automatically detect your device, and you can just click "Open Connection"!

### Option 2: Build from Source
If you want to compile the code yourself:
1. Make sure you have the **.NET 10 SDK** installed.
2. Clone this repository:
   ```bash
   git clone [https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git](https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git)
   ```
3. Run the following command to build:
   ```bash
   dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -p:EnableCompressionInSingleFile=true
   ```

### 🛡️ Troubleshooting: "Smart App Control" or "SmartScreen" Block
Since this is an open-source application without a paid digital signature, Windows might block it. To fix this:
1. **Right-click** the `RurinaAudio-Receiver.exe` file.
2. Select **Properties**.
3. In the **General** tab, look for **Security** at the bottom.
4. Check the **Unblock** box and click **OK**.
5. Run the app again!

---
---

# 🇹🇭 ภาษาไทย (Thai Version)

โปรแกรมขนาดเล็กที่จะเปลี่ยนคอมพิวเตอร์ Windows ของคุณให้กลายเป็น "ตัวรับสัญญาณเสียงบลูทูธ" ช่วยให้คุณส่งเสียงจากมือถือมาออกที่ลำโพงคอมพิวเตอร์ได้โดยตรงแบบไร้สาย!

## ✨ ฟีเจอร์หลักที่น่าสนใจ

* **เชื่อมต่อง่ายในคลิกเดียว:** กดปุ่มเดียวก็ดึงเสียงจากมือถือมาออกคอมได้เลย
* **⚡ จำอุปกรณ์อัตโนมัติ (Auto-Connect):** จำมือถือเครื่องล่าสุดและพยายามเชื่อมต่อให้เองทันทีเมื่อเปิดแอป
* **🔄 ระบบอัปเดตอัตโนมัติ:** แอปจะเช็คเวอร์ชันใหม่จาก GitHub ทุกครั้งที่เปิดเครื่อง หากมีเวอร์ชันใหม่จะเด้งถามให้อัปเดตทันที
* **🌙 ดีไซน์ Dark Mode:** หน้าต่างแอปสีเข้มสไตล์โมเดิร์น สบายตา
* **📥 ซ่อนแอปไว้เบื้องหลัง (Minimize to Tray):** แอปจะไปซ่อนตัวอยู่ที่มุมขวาล่างของจอ (System Tray) ไม่เกะกะ Taskbar
* **🚀 เปิดพร้อมคอม (Run on Startup):** สามารถตั้งค่าให้แอปเปิดตัวเองอัตโนมัติตอนเปิดเครื่องคอมพิวเตอร์ได้
* **🛡️ ป้องกันการเปิดแอปซ้ำ (Single Instance):** หากแอปซ่อนอยู่ การกดเปิดซ้ำจะช่วยดึงหน้าต่างเดิมกลับขึ้นมาให้ใช้งานต่อ
* **🐞 ระบบแจ้งเตือน Error ในตัว (Crash Logger):** หากแอปมีปัญหา จะมีหน้าต่างเด้งขึ้นมาบอกสาเหตุ (Log) อย่างชัดเจน

## 💻 สเปคคอมพิวเตอร์ที่ต้องการ

* **ระบบปฏิบัติการ:** Windows 10 (เวอร์ชัน 2004 ขึ้นไป) หรือ Windows 11
* **ฮาร์ดแวร์:** คอมพิวเตอร์ต้องมีตัวรับสัญญาณบลูทูธ (Bluetooth Adapter)
* **เฟรมเวิร์ก:** .NET 10 Desktop Runtime

## 🚀 วิธีใช้งาน

### 🛑 สิ่งที่ต้องทำก่อนใช้แอป (สำคัญมาก)
**คุณต้องทำการเชื่อมต่อ (Pair) บลูทูธระหว่างมือถือกับคอมพิวเตอร์ผ่านหน้าต่าง Settings ของ Windows ให้เรียบร้อย "ก่อน" ที่จะเปิดโปรแกรมนี้**

### วิธีที่ 1: โหลดตัวพร้อมใช้งาน (แนะนำ)
1. จับคู่บลูทูธมือถือกับคอมให้เรียบร้อย
2. ไปที่หน้า [Releases](../../releases) แล้วดาวน์โหลดไฟล์ `.exe` ล่าสุด
3. ดับเบิ้ลคลิกเปิดแอป `RurinaAudio-Receiver.exe` แล้วกดปุ่ม "Open Connection"

### วิธีที่ 2: สำหรับนักพัฒนา (Build โค้ดเอง)
1. ติดตั้ง **.NET 10 SDK**
2. โหลดโค้ดลงเครื่อง:
   ```bash
   git clone [https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git](https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git)
   ```
3. ใช้คำสั่ง Build:
   ```bash
   dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -p:EnableCompressionInSingleFile=true
   ```

### 🛡️ วิธีแก้ปัญหา: เมื่อโปรแกรมโดนบล็อก (Smart App Control / SmartScreen)
เนื่องจากโปรแกรมเป็น Open Source ที่ไม่มีใบรับรองดิจิทัลแบบชำระเงิน Windows อาจจะแจ้งเตือนความปลอดภัย ให้แก้ไขดังนี้:
1. **คลิกขวา** ที่ไฟล์ `RurinaAudio-Receiver.exe`
2. เลือก **Properties (คุณสมบัติ)**
3. ในแท็บ **General (ทั่วไป)** มองไปด้านล่างตรงหัวข้อ **Security (ความปลอดภัย)**
4. ติ๊กถูกที่ช่อง **Unblock (เลิกบล็อก)** แล้วกด **OK**
5. เปิดโปรแกรมใหม่อีกครั้งครับ!

---
**พัฒนาโดย:** Nattapat Jitsom ([Nattapat2871](https://github.com/Nattapat2871))