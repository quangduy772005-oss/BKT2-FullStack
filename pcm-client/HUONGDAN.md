# 🎯 Hướng dẫn sử dụng - PCM (Quản lý Nhân sự)

## 👋 Chào mừng bạn!

Bạn vừa có được một ứng dụng **quản lý nhân sự hoàn chỉnh**. Hãy làm theo các bước dưới để chạy ứng dụng.

---

## 🚀 Bước 1: Cài đặt

Mở Terminal/CMD tại thư mục project và chạy:

```bash
npm install
```

Đợi cho đến khi tất cả packages được cài đặt xong (khoảng 1-2 phút).

---

## ⚙️ Bước 2: Cấu hình Backend URL

### Cách 1: Tạo file `.env.local` (Được khuyến nghị)

Tạo file mới tên `.env.local` trong thư mục gốc:

```env
VITE_API_BASE_URL=http://localhost:8080/api
```

⚠️ Thay `8080` thành port backend của bạn nếu khác.

### Cách 2: Sửa file `src/config/constants.ts`

Mở file `src/config/constants.ts` và thay đổi:

```typescript
export const API_BASE_URL = "http://localhost:8080/api";
```

---

## ▶️ Bước 3: Chạy Ứng dụng

```bash
npm run dev
```

Ứng dụng sẽ tự động mở tại: **http://localhost:5173**

---

## 📝 Bước 4: Đăng nhập

1. Mở trình duyệt: http://localhost:5173
2. Bạn sẽ thấy trang **Đăng nhập**
3. Nhập **tài khoản** và **mật khẩu**
4. Nhấn nút **Đăng Nhập**

⚠️ Lưu ý: Tài khoản phải đã được tạo trong backend!

---

## 📱 Sử dụng Ứng dụng

### Trang Dashboard (Trang chủ)
- Hiển thị chào mừng theo tên bạn
- Thống kê: Tổng số thành viên, năm hiện tại
- Danh sách 5 thành viên gần đây

**Hành động:**
- Nút "Quản lý thành viên" → Đi tới trang Members
- Nút "Đăng xuất" → Đăng xuất & về trang Login

### Trang Members (Quản lý thành viên)
Danh sách tất cả thành viên trong bảng

**Hành động:**
- Nút "+ Thêm thành viên" → Mở form thêm mới
- Nút "Tải lại" → Làm mới danh sách
- Nút "Xóa" (hàng) → Xóa thành viên
- Nút "Đăng xuất" → Đăng xuất

**Thêm thành viên mới:**
1. Nhấn "+ Thêm thành viên"
2. Điền thông tin:
   - **Tên**: Tên đầy đủ (bắt buộc)
   - **Email**: Email (bắt buộc)
   - **Điện thoại**: Số điện thoại
   - **Chức vụ**: Vị trí công việc
3. Nhấn "Lưu"
4. Danh sách sẽ cập nhật tự động

---

## 🔧 Cách thức hoạt động

### Luồng xác thực

```
1. Nhập tài khoản + mật khẩu
   ↓
2. Gửi POST /api/auth/login
   ↓
3. Backend trả về JWT token
   ↓
4. Lưu token vào localStorage
   ↓
5. Chuyển hướng tới Dashboard
```

### Gửi request tới API

```
Mỗi request sẽ tự động:
1. Lấy token từ localStorage
2. Thêm vào header: Authorization: Bearer <token>
3. Gửi tới backend
4. Nhận response & cập nhật UI
```

---

## 📦 Backend API cần thiết

Backend của bạn phải có những endpoint này:

### Đăng nhập
```
POST /api/auth/login

Request:
{
  "username": "user",
  "password": "password"
}

Response (Success):
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "username": "user",
    "email": "user@example.com"
  }
}
```

### Danh sách thành viên
```
GET /api/members
Header: Authorization: Bearer <token>

Response:
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "phone": "0123456789",
    "position": "Developer"
  },
  ...
]
```

### Tạo thành viên
```
POST /api/members
Header: Authorization: Bearer <token>

Request:
{
  "name": "Jane Doe",
  "email": "jane@example.com",
  "phone": "0987654321",
  "position": "Manager"
}

Response:
{ "id": 2, "name": "Jane Doe", ... }
```

### Xóa thành viên
```
DELETE /api/members/:id
Header: Authorization: Bearer <token>

Response: (204 No Content)
```

---

## ⚠️ Lỗi thường gặp

### ❌ "Cannot GET /api/members"
**Nguyên nhân**: Backend không chạy hoặc port sai

**Giải pháp**:
1. Kiểm tra backend đang chạy
2. Kiểm tra port trong `.env.local`
3. Restart ứng dụng

### ❌ "CORS Error"
**Nguyên nhân**: Backend không cho phép cross-origin

**Giải pháp**:
Thêm CORS header trên backend:
```
Access-Control-Allow-Origin: http://localhost:5173
Access-Control-Allow-Methods: GET, POST, PUT, DELETE
Access-Control-Allow-Headers: Content-Type, Authorization
```

### ❌ "401 Unauthorized"
**Nguyên nhân**: Token không hợp lệ hoặc hết hạn

**Giải pháp**:
Đăng nhập lại

### ❌ "Sai tài khoản hoặc mật khẩu"
**Nguyên nhân**: Tài khoản/mật khẩu không đúng

**Giải pháp**:
1. Kiểm tra backend có tài khoản này chưa
2. Kiểm tra đúng tài khoản/mật khẩu
3. Tạo tài khoản mới nếu cần

---

## 🔍 Cách Debug

### Kiểm tra Token
Mở F12 → Console, gõ:
```javascript
localStorage.getItem('token')
```

Nếu có token thì đã lưu thành công.

### Kiểm tra User Info
```javascript
JSON.parse(localStorage.getItem('user'))
```

### Xem Network Request
F12 → Network tab → Xem các request tới `/api/*`

---

## 🛠️ Npm Commands

```bash
npm run dev      # Chạy dev server (port 5173)
npm run build    # Build cho production
npm run preview  # Xem kết quả build
npm run lint     # Check code quality
```

---

## 📁 Cấu trúc thư mục

```
src/
├── views/        ← Các trang (Login, Dashboard, Members)
├── components/   ← Các thành phần (Navbar)
├── services/     ← Logic API & Auth
├── router/       ← Cấu hình routes
├── composables/  ← Tái sử dụng logic
├── utils/        ← Hàm tiện ích
├── types/        ← Type definitions
└── config/       ← Cấu hình
```

---

## 📚 Tài liệu thêm

- **README.md** - Tài liệu hoàn chỉnh
- **QUICKSTART.md** - Bắt đầu nhanh
- **GUIDE.md** - Hướng dẫn chi tiết
- **CHANGES.md** - Danh sách thay đổi
- **SUMMARY.md** - Tóm tắt dự án

---

## ✅ Checklist bắt đầu

- [ ] Cài `npm install`
- [ ] Cấu hình API URL (`.env.local`)
- [ ] Kiểm tra backend chạy
- [ ] Chạy `npm run dev`
- [ ] Đăng nhập thành công
- [ ] Xem danh sách thành viên

---

## 💡 Tips hữu ích

1. **Làm mới trang**: Ctrl+Shift+R (hard refresh)
2. **Xóa localStorage**: Console → `localStorage.clear()`
3. **Kiểm tra API URL**: F12 → Network → Xem URL request

---

## 📞 Nếu có vấn đề

1. ✅ Kiểm tra backend có chạy không
2. ✅ Kiểm tra API URL đúng chưa
3. ✅ Xem console browser (F12)
4. ✅ Xem Network tab (F12)
5. ✅ Restart ứng dụng

---

## 🎉 Chúc mừng!

Bạn đã sẵn sàng sử dụng ứng dụng. 

**Bắt đầu bằng cách:**
```bash
npm run dev
```

**Happy Coding! 🚀**
