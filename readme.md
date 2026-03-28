# 🎵 Rurina Bluetooth Audio Receiver

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Windows](https://img.shields.io/badge/Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Version](https://img.shields.io/badge/Version-1.0-green?style=for-the-badge)

![Visitor Badge](https://api.visitorbadge.io/api/VisitorHit?user=Nattapat2871&repo=Rurina-Bluetooth-Audio-Receiver&countColor=%237B1E7A)

A lightweight, minimalist Windows application that transforms your PC into a Bluetooth Audio Receiver (A2DP Sink). Seamlessly stream music, videos, or game audio from your smartphone or tablet directly to your computer's speakers.

<img width="354" height="407" alt="image" src="https://github.com/user-attachments/assets/482e2a6c-b8f3-4f97-8ccf-5c809ff7792c" />


## ✨ Features

* **One-Click Audio Routing:** Easily establish a direct audio connection with your paired Bluetooth devices.
* **⚡ Smart Auto-Connect:** The application remembers your last successfully connected device and automatically attempts to reconnect upon the next launch.
* **🌙 Minimalist Dark UI:** A clean, custom-painted dark mode interface designed to be distraction-free and easy on the eyes.
* **📥 Minimize to Tray:** Click the minimize button to hide the app into the system tray (bottom-right corner) so it keeps running quietly in the background without cluttering your taskbar.
* **🚀 Run on Startup:** A convenient checkbox allows you to set the app to open automatically every time you turn on your PC.
* **🛡️ Single Instance Protection:** Prevents opening multiple copies of the app. If you try to open it again while it's hidden in the tray, it will automatically bring the existing window to the front.
* **🐞 Built-in Crash Logger:** If something goes wrong, the app won't just close silently. It will display a custom error window with detailed logs to help you (or the developer) identify the problem easily.

## 💻 System Requirements

* **OS:** Windows 10 (Version 2004 or newer) or Windows 11.
* **Hardware:** A working Bluetooth adapter on your PC.
* **Framework:** .NET 10 SDK (Only required if you are building from source).

## 🚀 How to Use

### 🛑 Prerequisite (Must Read First)
**Please pair your smartphone's Bluetooth with your computer completely in the standard Windows Bluetooth Settings BEFORE launching the program.**

### Option 1: Using the Pre-built Release (Recommended)
1. Ensure your phone is already paired with your PC.
2. Download the latest `.exe` file from the [Releases](../../releases) page.
3. Run `RurinaAudio-Receiver.exe`. The app will automatically detect your device, and you can just click "Open Connection"!

### Option 2: Build from Source
If you want to compile the code yourself:
1. Make sure you have the **.NET 10 SDK** installed on your machine.
2. Clone this repository to your local machine:
   ```bash
   git clone [https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git](https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git)
   ```
3. Open your terminal or command prompt in the project folder and run this specific command to build a highly optimized, compressed single `.exe` file:
   ```bash
   dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -p:EnableCompressionInSingleFile=true
   ```
   *(Note: This command packages the .NET runtime directly into the `.exe` and compresses it so it can be run on any Windows PC without needing to install .NET, keeping the file size reasonable!)*

---
---

# 🇹🇭 ภาษาไทย (Thai Version)

โปรแกรมขนาดเล็กและใช้งานง่ายที่จะเปลี่ยนคอมพิวเตอร์ Windows ของคุณให้กลายเป็น "ตัวรับสัญญาณเสียงบลูทูธ" (Bluetooth Receiver) ช่วยให้คุณเปิดเพลง ดูคลิป หรือเล่นเกมบนมือถือ/แท็บเล็ต แล้วส่งเสียงมาออกที่ลำโพงคอมพิวเตอร์ได้โดยตรงแบบไร้สาย!

## ✨ ฟีเจอร์หลักที่น่าสนใจ

* **เชื่อมต่อง่ายในคลิกเดียว:** กดปุ่มเดียวก็ดึงเสียงจากมือถือมาออกคอมได้เลย
* **⚡ จำอุปกรณ์อัตโนมัติ (Auto-Connect):** โปรแกรมจะจำมือถือเครื่องล่าสุดที่คุณเคยต่อไว้ พอเปิดแอปครั้งหน้า มันจะพยายามเชื่อมต่อให้เองทันที
* **🌙 ดีไซน์ Dark Mode:** หน้าต่างแอปสีเข้มสไตล์โมเดิร์น สบายตาและดูทันสมัย
* **📥 ซ่อนแอปไว้เบื้องหลัง (Minimize to Tray):** เวลากดย่อหน้าต่าง แอปจะไม่ไปเกะกะบน Taskbar แต่จะไปซ่อนตัวเงียบๆ อยู่ที่มุมขวาล่างของจอ (System Tray) แทน
* **🚀 เปิดพร้อมคอม (Run on Startup):** มีปุ่มติ๊กถูกให้เลือกได้เลยว่าอยากให้แอปเปิดตัวเองอัตโนมัติตอนเปิดเครื่องคอมพิวเตอร์หรือไม่
* **🛡️ ป้องกันการเปิดแอปซ้ำ (Single Instance):** ถ้าแอปซ่อนอยู่แล้วคุณเผลอไปกดเปิดแอปใหม่ซ้ำ มันจะไม่เปิดหน้าต่างซ้อนกันให้รกเครื่อง แต่จะไปดึงหน้าต่างแอปตัวเดิมที่ซ่อนอยู่เด้งกลับขึ้นมาให้ใช้งานต่อได้เลย
* **🐞 ระบบแจ้งเตือน Error ในตัว (Crash Logger):** ถ้าแอปพังหรือมีปัญหา มันจะไม่เด้งปิดตัวเองหายไปดื้อๆ แต่จะมีหน้าต่างเด้งขึ้นมาบอกสาเหตุ (Log) ให้คุณรู้ว่าเกิดอะไรขึ้น เพื่อนำไปแก้ไขได้ง่ายขึ้น

## 💻 สเปคคอมพิวเตอร์ที่ต้องการ

* **ระบบปฏิบัติการ:** Windows 10 (เวอร์ชัน 2004 ขึ้นไป) หรือ Windows 11
* **ฮาร์ดแวร์:** คอมพิวเตอร์ต้องมีตัวรับสัญญาณบลูทูธ (Bluetooth Adapter) ที่ใช้งานได้
* **โปรแกรมที่ต้องมี:** .NET 10 SDK (เฉพาะคนที่ต้องการเอาโค้ดไปคอมไพล์เองเท่านั้น ถ้าโหลดตัวพร้อมใช้ไม่ต้องลง)

## 🚀 วิธีใช้งาน

### 🛑 สิ่งที่ต้องทำก่อนใช้แอป (สำคัญมาก)
**คุณต้องทำการเชื่อมต่อ (Pair) บลูทูธระหว่างมือถือกับคอมพิวเตอร์ผ่านหน้าต่าง Settings ปกติของ Windows ให้เรียบร้อย "ก่อน" ที่จะเปิดโปรแกรมนี้**

### วิธีที่ 1: โหลดตัวพร้อมใช้งาน (แนะนำสำหรับผู้ใช้ทั่วไป)
1. จับคู่บลูทูธมือถือกับคอมให้เรียบร้อย
2. ไปที่หน้า [Releases](../../releases) แล้วดาวน์โหลดไฟล์ `.exe` ล่าสุด
3. ดับเบิ้ลคลิกเปิดแอป `RurinaAudio-Receiver.exe` แอปจะเจอมือถือของคุณทันที ให้กดปุ่ม "Open Connection" เพื่อเริ่มฟังเพลงได้เลย!

### วิธีที่ 2: สำหรับนักพัฒนา (Build โค้ดเอง)
ถ้าคุณอยากเอาโค้ดไปรันเอง ให้ทำตามนี้ครับ:
1. ติดตั้ง **.NET 10 SDK** ลงในเครื่องให้เรียบร้อย
2. โหลดโค้ดลงเครื่องด้วยคำสั่ง:
   ```bash
   git clone [https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git](https://github.com/Nattapat2871/Rurina-Bluetooth-Audio-Receiver.git)
   ```
3. เปิด Terminal หรือ Command Prompt ในโฟลเดอร์โปรเจกต์ แล้วพิมพ์คำสั่งนี้เพื่อสร้างแอป:
   ```bash
   dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -p:EnableCompressionInSingleFile=true
   ```
   *(คำอธิบาย: คำสั่งนี้คือการสั่งให้ .NET ยัดทุกอย่างรวมเป็นไฟล์ `.exe` ไฟล์เดียว และบีบอัดขนาดไฟล์ให้เล็กลง ข้อดีคือไฟล์ที่ได้สามารถเอาไปส่งให้เพื่อน หรือเอาไปเปิดเครื่องไหนก็ได้บนโลกทันที โดยที่เครื่องนั้นๆ ไม่จำเป็นต้องติดตั้ง .NET เอาไว้เลยครับ)*

---
**พัฒนาโดย:** Nattapat Jitsom ([Nattapat2871](https://github.com/Nattapat2871))
