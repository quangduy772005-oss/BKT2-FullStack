# 🚀 Quick Start Guide

## Bắt đầu nhanh - 5 phút

### 1️⃣ Cài đặt

```bash
npm install
```

### 2️⃣ Cấu hình API (Quan trọng!)

**Option A**: Sử dụng `.env.local` file

Tạo file `.env.local` trong thư mục gốc:

```env
VITE_API_BASE_URL=http://localhost:8080/api
```

**Option B**: Chỉnh sửa constants

Edit `src/config/constants.ts`:

```typescript
export const API_BASE_URL = "http://localhost:8080/api"; // 👈 Đổi port của backend
```

### 3️⃣ Chạy ứng dụng

```bash
npm run dev
```

Ứng dụng sẽ mở tại: **http://localhost:5173**

### 4️⃣ Đăng nhập

Dùng tài khoản từ backend:
- **Username**: (Do backend cung cấp)
- **Password**: (Do backend cung cấp)

## 📱 Tính năng chính

| Tính năng | URL | Mô tả |
|----------|-----|-------|
| Đăng nhập | `/login` | Đăng nhập với tài khoản |
| Dashboard | `/dashboard` | Trang chủ - Thống kê |
| Quản lý | `/members` | Danh sách thành viên |

## 🔌 API Cần thiết

Backend của bạn phải có:

```bash
# Đăng nhập
POST /api/auth/login
{
  "username": "user",
  "password": "password"
}

# Thành viên
GET    /api/members
POST   /api/members
DELETE /api/members/:id
```

## ⚙️ Npm Scripts

```bash
npm run dev       # Chạy dev server
npm run build     # Build production
npm run preview   # Xem build
npm run type-check # TypeScript check
```

## 📂 Thư mục quan trọng

```
src/
├── views/        📄 Pages (Login, Dashboard, Members)
├── components/   🧩 Reusable components (Navbar)
├── services/     🔧 API & Auth logic
├── router/       🛣️ Route config
└── assets/       🎨 Images, fonts
```

## 🆘 Lỗi phổ biến

### "Cannot GET /api/members"
❌ Backend không chạy hoặc URL sai
✅ Kiểm tra port trong `.env.local`

### CORS Error
❌ Backend không cho phép cross-origin
✅ Thêm header `Access-Control-Allow-Origin` trên backend

### 401 Unauthorized
❌ Token không hợp lệ hoặc hết hạn
✅ Đăng nhập lại

## 💾 Lưu trữ

Token & user info được lưu trong:
```javascript
localStorage.getItem('token')    // JWT token
localStorage.getItem('user')     // User info (JSON)
```

## 🔐 Bảo mật

⚠️ **KHÔNG** để lộ token trong console log
⚠️ Luôn sử dụng HTTPS trên production
⚠️ Đặt token expiry trên backend

## 📚 File tham khảo

- `README.md` - Tài liệu đầy đủ
- `GUIDE.md` - Hướng dẫn chi tiết
- `CHANGES.md` - Tóm tắt thay đổi

## ✅ Checklist

- [ ] Cài đặt dependencies: `npm install`
- [ ] Cấu hình API URL: `.env.local`
- [ ] Chạy dev server: `npm run dev`
- [ ] Backend đang chạy: `localhost:8080`
- [ ] Đăng nhập thành công

---

**Được rồi! Ready to go? 🎉**
