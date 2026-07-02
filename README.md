# 💬 UMD-08: Ứng dụng Chat GUI qua TCP

Ứng dụng trò chuyện trực tuyến dạng Desktop có giao diện đồ họa (Windows Forms), hoạt động theo mô hình Client-Server sử dụng giao thức TCP/IP Socket. Hỗ trợ chat nhóm, chat riêng tư, truyền file và ảnh đại diện (Avatar).

---

## ✨ Tính năng

**Cốt lõi:**
- Kết nối / Ngắt kết nối an toàn từ Client đến Server qua IP và Port
- Chat phòng chung (Public Group Chat): tất cả Client trực tuyến đều nhận tin nhắn
- Chat riêng tư (Private Chat): nhắn tin 1-1 giữa hai tài khoản bất kỳ
- Danh sách người dùng trực tuyến cập nhật theo thời gian thực (Real-time)
- Cài đặt ảnh đại diện người dùng, rely, gửi biểu tượng cảm xúc

**Nâng cao:**
- Gửi và nhận tệp tin (File Transfer) qua luồng byte TCP
- Lịch sử chat: Server lưu trữ và gửi lại tin nhắn cũ cho thành viên mới
- Giao diện điều khiển / giám sát kết nối cho Server

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ lập trình | C# (.NET) |
| Giao diện đồ họa (GUI) | Windows Forms (WinForms) |
| Thư viện mạng | `System.Net.Sockets` |
| Xử lý đa luồng | `System.Threading` |

---

## 📋 Yêu cầu hệ thống

- **Hệ điều hành:** Windows 10 / Windows 11
- **.NET SDK:** Phiên bản **8.0 trở lên** 
- **Visual Studio:** 2026 (khuyến nghị) hoặc 2022

---

## 🚀 Hướng dẫn cài đặt và chạy

###  Bước 1 cài đặt Visual Studio (nếu chưa có)

1. Tải **Visual Studio Community 2026** (miễn phí) tại:
   [https://visualstudio.microsoft.com/fr/vs/community]

2. Chạy trình cài đặt. Khi màn hình chọn Workload hiện ra, tích vào:
   > ✅ **.NET desktop development**

   Workload này bao gồm sẵn Windows Forms, C# compiler và .NET SDK — không cần cài thêm gì khác.

3. Nhấn **Install** và chờ cài xong (có thể mất 10–20 phút tùy tốc độ mạng).

---

### Bước 2 tải mã nguồn về máy

** — Dùng Git (khuyến nghị):**

Mở **Command Prompt** hoặc **PowerShell**, chạy:

```bash
git clone https://github.com/Nhom-Laptrinhmang/Project_Laptrinhmang.git
cd Project_Laptrinhmang
```

---

### Bước 3 — Mở dự án trong Visual Studio

1. Mở **Visual Studio 2**
2. Chọn **Open a project or solution**
3. Điều hướng đến thư mục vừa tải về, chọn file **`.slnx`** (Solution) → nhấn **Open**

   Ví dụ: `Project_Laptrinhmang\ChatApp.slnx`

4. Visual Studio sẽ tự động load toàn bộ project. Chờ thanh tiến trình ở góc dưới bên phải chạy xong

---

### Bước 4 — Chạy Server

1. Trên thanh công cụ chọn tools -> Command line -> Developer PowerShell.
2. Sau khi mở được Developer PowerShell, gõ lệnh cd CODE/Server -> dotnet run 
3. Sau khi Server chạy bấm vào START SERVER để khởi động, chờ Client


---

### Bước 5 — Chạy Client

1. Trên thanh công cụ chọn tools -> Command line -> Developer PowerShell.
2. Sau khi mở được Developer PowerShell, gõ lệnh cd CODE/Server -> dotnet run 
3. Sau khi Client chạy, Nhập tên, nhập
---



## 👥 Thành viên nhóm

| Họ và tên | MSSV | GitHub |
|---|---|---|
| Bùi Ngọc Hải |080206000952| [@buingochai280120-prog](https://github.com/buingochai280120-prog) |
| Đào Tuấn Anh |066206005506 | [@Dtanh06](https://github.com/Dtanh06) |
| Huỳnh Quốc Cường |052206000403| [@CuongCloud169](https://github.com/CuongCloud169) |
| Trịnh Vũ Quang Huy |035206004749 | [@trvuqhuy](https://github.com/trvuqhuy) |
| Tô Nguyễn Khải |080206006982| [@nguyenkhai-hub](https://github.com/nguyenkhai-hub) |
| Nguyễn Văn Khâm |058206001949 | [@makNVor](https://github.com/makNVor) |


---
