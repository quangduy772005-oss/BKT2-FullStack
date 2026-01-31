# PCM - Ứng dụng Quản lý Nhân sự

## 📋 Mô tả

PCM (Personnel Cycle Management) là một ứng dụng web giúp quản lý thông tin nhân viên, bao gồm các tính năng:

- ✅ **Đăng nhập / Đăng xuất** - Xác thực người dùng với JWT
- ✅ **Dashboard** - Hiển thị thống kê tổng quát
- ✅ **Quản lý thành viên (CRUD)** - Thêm, xóa, xem thành viên
- ✅ **Giao diện Responsive** - Tương thích với mọi thiết bị
- ✅ **Bảo mật** - Route guards, token-based authentication

## 🛠️ Công nghệ sử dụng

- **Frontend Framework**: Vue 3 (Composition API)
- **Language**: TypeScript
- **Build Tool**: Vite
- **Router**: Vue Router 4
- **HTTP Client**: Axios
- **CSS**: Tailwind-inspired (Vanilla CSS)

## 📦 Cấu trúc dự án

```
src/
├── api/                    # API calls & axios instance
│   ├── axios.js           # Axios with interceptors
│   └── memberApi.js       # Member endpoints
├── components/            # Reusable Vue components
│   └── Navbar.vue        # Navigation component
├── composables/          # Vue composables (logic reuse)
│   ├── useError.ts       # Error handling logic
│   └── useLoading.ts     # Loading state management
├── config/               # Configuration files
│   └── constants.ts      # Constants & colors
├── router/               # Vue Router configuration
│   └── index.ts         # Route definitions
├── services/            # Business logic
│   ├── api.ts          # API instance with interceptors
│   └── auth.ts         # Authentication functions
├── types/              # TypeScript type definitions
│   └── index.ts       # All types
├── utils/              # Utility functions
│   └── helpers.ts     # Helper functions
├── views/              # Page components
│   ├── Dashboard.vue   # Dashboard page
│   ├── Login.vue       # Login page
│   ├── Members.vue     # Members management
│   └── NotFound.vue    # 404 page
├── App.vue            # Root component
├── main.ts            # Application entry point
└── style.css          # Global styles
```

## 🚀 Cách chạy

### 1. Yêu cầu hệ thống
- Node.js 16+ 
- npm hoặc yarn

### 2. Cài đặt dependencies

```bash
npm install
```

### 3. Cấu hình biến môi trường

Tạo file `.env.local`:

```env
VITE_API_BASE_URL=http://localhost:8080/api
```

### 4. Chạy ứng dụng phát triển

```bash
npm run dev
```

Ứng dụng sẽ chạy tại: `http://localhost:5173`

### 5. Build cho production

```bash
npm run build
```

### 6. Preview build

```bash
npm run preview
```

## 📚 API Endpoints

### Authentication
```
POST /api/auth/login
Request: { "username": "string", "password": "string" }
Response: { "token": "string", "user": { "id": number, "username": "string", "email": "string" } }
```

### Members
```
GET    /api/members              # Get all members
POST   /api/members              # Create member
GET    /api/members/:id          # Get member by ID
PUT    /api/members/:id          # Update member
DELETE /api/members/:id          # Delete member
```

## 🔐 Authentication & Security

- **JWT Token**: Token được lưu trong `localStorage`
- **Interceptors**: Token tự động gửi kèm trong mỗi request
- **Route Guards**: Chỉ người dùng đã đăng nhập mới truy cập được
- **Auto Logout**: Nếu nhận được 401, người dùng sẽ được đưa về login

## 📱 Responsive Design

Hỗ trợ: Mobile (< 768px) | Tablet (768px-1199px) | Desktop (1200px+)

## 🎨 Design System

**Primary Colors**:
- Purple: `#667eea`
- Dark Purple: `#764ba2`

**Status Colors**:
- Success: `#27ae60`
- Danger: `#e74c3c`
- Warning: `#f39c12`
- Info: `#3498db`

## 📝 Pages

| Page | Path | Description |
|------|------|-------------|
| Login | `/login` | Form đăng nhập |
| Dashboard | `/dashboard` | Trang chủ với thống kê |
| Members | `/members` | Quản lý danh sách thành viên |
| Not Found | `/404` | Trang lỗi 404 |

## 💡 Tips

1. Dùng DevTools (F12) để debug
2. Xem token: `localStorage.getItem('token')`
3. Xem user info: `JSON.parse(localStorage.getItem('user'))`

## 🐛 Troubleshooting

| Lỗi | Nguyên nhân | Giải pháp |
|-----|-----------|---------|
| Cannot GET /api/... | Backend không chạy | Kiểm tra URL & backend |
| CORS Error | Backend không cho phép | Thêm CORS headers |
| 401 Unauthorized | Token hết hạn | Đăng nhập lại |

## 📄 License

MIT
