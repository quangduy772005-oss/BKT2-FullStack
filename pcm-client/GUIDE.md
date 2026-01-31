# PCM - Ứng dụng Quản lý Nhân sự

## 📋 Mô tả

PCM (Personnel Cycle Management) là một ứng dụng web giúp quản lý thông tin nhân viên, bao gồm các tính năng:

- ✅ Đăng nhập / Đăng xuất
- ✅ Dashboard hiển thị thống kê
- ✅ Quản lý danh sách thành viên (CRUD)
- ✅ Giao diện responsive

## 🛠️ Công nghệ sử dụng

- **Frontend**: Vue 3 + TypeScript + Vite
- **Router**: Vue Router 4
- **HTTP Client**: Axios
- **Styling**: CSS3

## 📦 Cấu trúc dự án

```
src/
├── api/              # API calls
│   ├── axios.js      # Axios config
│   └── memberApi.js  # Member endpoints
├── components/       # Reusable components
│   └── Navbar.vue   # Navigation bar
├── router/          # Vue Router config
├── services/        # Business logic
│   ├── api.ts       # API base config
│   └── auth.ts      # Auth functions
├── views/           # Page components
│   ├── Login.vue    # Login page
│   ├── Dashboard.vue # Dashboard
│   ├── Members.vue  # Members management
│   └── NotFound.vue # 404 page
├── App.vue          # Root component
├── main.ts          # Entry point
└── style.css        # Global styles
```

## 🚀 Cách chạy

### 1. Cài đặt dependencies

```bash
npm install
```

### 2. Cấu hình API endpoint

Mở file `src/services/api.ts` và thay đổi `baseURL` thành URL backend của bạn:

```typescript
const api = axios.create({
  baseURL: "http://localhost:8080/api", // ⚠️ Đổi port của backend
  headers: {
    "Content-Type": "application/json",
  },
});
```

### 3. Chạy ứng dụng

```bash
npm run dev
```

### 4. Build cho production

```bash
npm run build
```

## 📚 API Endpoints

Ứng dụng yêu cầu backend cung cấp các endpoints sau:

### Authentication
- `POST /api/auth/login` - Đăng nhập
  ```json
  {
    "username": "user",
    "password": "password"
  }
  ```
  Response:
  ```json
  {
    "token": "jwt_token",
    "user": { "id": 1, "username": "user", "email": "user@example.com" }
  }
  ```

### Members
- `GET /api/members` - Lấy danh sách thành viên
- `GET /api/members/:id` - Lấy chi tiết thành viên
- `POST /api/members` - Tạo thành viên mới
- `PUT /api/members/:id` - Cập nhật thành viên
- `DELETE /api/members/:id` - Xóa thành viên

## 🔐 Authentication

Ứng dụng sử dụng JWT token để xác thực:
- Token được lưu trong `localStorage`
- Token tự động gửi kèm trong header `Authorization: Bearer <token>`
- Nếu không có token, người dùng sẽ bị chuyển hướng về trang login

## 📱 Responsive Design

Ứng dụng hỗ trợ:
- Desktop (1200px+)
- Tablet (768px - 1199px)
- Mobile (< 768px)

## 🎨 Màu sắc chính

- Primary: `#667eea` (Purple)
- Secondary: `#764ba2` (Dark Purple)
- Text: `#2c3e50` (Dark Blue)
- Background: `#f5f6fa` (Light Gray)
- Danger: `#e74c3c` (Red)

## 📝 Pages

### Login (`/login`)
- Form đăng nhập với tài khoản và mật khẩu
- Hiển thị thông báo lỗi nếu đăng nhập thất bại

### Dashboard (`/dashboard`)
- Hiển thị thống kê chung
- Danh sách 5 thành viên gần đây
- Nút điều hướng nhanh

### Members (`/members`)
- Bảng danh sách tất cả thành viên
- Nút thêm thành viên mới
- Nút xóa thành viên
- Modal form để thêm thành viên

### Not Found (`/404`)
- Trang lỗi 404 cho các route không tồn tại

## 💡 Tips

1. **Đăng nhập**: Dùng tài khoản do backend cung cấp
2. **Thêm thành viên**: Click nút "Thêm thành viên" trong trang Members
3. **Đăng xuất**: Sử dụng nút "Đăng xuất" ở navbar

## 🐛 Troubleshooting

### Không thể kết nối đến backend
- Kiểm tra URL backend trong `src/services/api.ts`
- Đảm bảo backend đang chạy
- Kiểm tra CORS settings trên backend

### Lỗi 401 Unauthorized
- Token có thể đã hết hạn
- Thử đăng nhập lại

### CORS Error
- Thêm CORS headers trên backend
- Hoặc sử dụng proxy trong Vite config

## 📄 License

MIT

## 👥 Support

Nếu có vấn đề, vui lòng liên hệ quản trị viên.
