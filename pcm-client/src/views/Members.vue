<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { getMembers, createMember, deleteMember } from "../api/memberApi";
import { useLoading } from "../composables/useLoading";
import { useError } from "../composables/useError";
import { useAuthStore } from "../stores/useAuthStore";

const router = useRouter();
const authStore = useAuthStore();
const members = ref<any[]>([]);
const showForm = ref(false);

const { loading, setLoading } = useLoading(false);
const { error, setError, clearError } = useError("");

const formData = ref({
  fullName: "",
  email: "",
  phoneNumber: "",
});

onMounted(() => {
  if (!authStore.isAuthenticated) {
    router.push("/login");
    return;
  }
  fetchMembers();
});

const fetchMembers = async () => {
  setLoading(true);
  clearError();
  try {
    const response = await getMembers();
    members.value = response.data;
  } catch (err: any) {
    setError("Không thể tải danh sách thành viên");
    console.error(err);
  } finally {
    setLoading(false);
  }
};

const openForm = () => {
  showForm.value = true;
  resetForm();
};

const closeForm = () => {
  showForm.value = false;
  resetForm();
};

const resetForm = () => {
  formData.value = {
    fullName: "",
    email: "",
    phoneNumber: "",
  };
};

const saveForm = async () => {
  if (!formData.value.fullName || !formData.value.email) {
    setError("Vui lòng nhập tên và email");
    return;
  }

  try {
    await createMember(formData.value);
    await fetchMembers();
    closeForm();
    clearError();
  } catch (err: any) {
    const errorMsg = err.response?.data?.message || err.response?.data || "Lỗi khi tạo thành viên mới. Vui lòng kiểm tra lại thông tin.";
    setError(errorMsg);
    console.error(err);
  }
};

const handleDeleteMember = async (id: number) => {
  if (confirm("Bạn chắc chắn muốn xóa thành viên này?")) {
    try {
      await deleteMember(id);
      await fetchMembers();
    } catch (err: any) {
      setError("Lỗi khi xóa thành viên");
      console.error(err);
    }
  }
};

const logout = () => {
    authStore.logout();
    router.push("/login");
};

const formatDate = (date: string) => {
    if (!date) return "N/A";
    return new Date(date).toLocaleDateString('vi-VN');
}
</script>

<template>
  <div class="page-container">
    <div class="glass-background"></div>
    
    <header class="navbar">
      <div class="nav-content">
        <div class="logo-area">
          <div class="logo-icon">P</div>
          <h1>PCM PRO</h1>
        </div>
        <div class="user-controls">
          <span class="welcome-text">Xin chào, {{ authStore.user?.fullName || 'Admin' }}</span>
          <button @click="logout" class="btn-logout">
            <span class="icon">󰍃</span> Đăng xuất
          </button>
        </div>
      </div>
    </header>

    <main class="main-layout">
      <div class="page-header">
        <div class="title-section">
          <h2>Quản lý Thành viên</h2>
          <p class="subtitle">Quản lý và theo dõi danh sách thành viên câu lạc bộ</p>
        </div>
        <div class="action-section">
          <button @click="openForm" class="btn-add">
            <span class="icon">󰐕</span> Thêm thành viên
          </button>
          <button @click="fetchMembers" class="btn-refresh" :disabled="loading">
             <span class="icon">󰑐</span>
          </button>
        </div>
      </div>

      <div v-if="error" class="error-toast">{{ error }}</div>

      <div class="content-card">
        <div v-if="loading" class="loading-state">
          <div class="spinner"></div>
          <p>Đang xử lý dữ liệu...</p>
        </div>

        <div v-else class="table-container">
          <table class="premium-table">
            <thead>
              <tr>
                <th>Thành viên</th>
                <th>Liên hệ</th>
                <th>Ngày gia nhập</th>
                <th>Sổ dư ví</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="member in members" :key="member.id" class="member-row">
                <td>
                  <div class="member-info">
                    <div class="avatar-circle">{{ member.fullName?.charAt(0) }}</div>
                    <div class="details">
                      <span class="name">{{ member.fullName }}</span>
                      <span class="id">ID: #{{ member.id }}</span>
                    </div>
                  </div>
                </td>
                <td>
                  <div class="contact-info">
                    <div class="info-item"><span class="icon">󰇧</span> {{ member.email }}</div>
                    <div class="info-item"><span class="icon">󰏲</span> {{ member.phoneNumber || 'N/A' }}</div>
                  </div>
                </td>
                <td>
                  <span class="date-badge">{{ formatDate(member.joinDate) }}</span>
                </td>
                <td>
                   <span class="wallet-amount">{{ member.walletBalance?.toLocaleString() || 0 }} VNĐ</span>
                </td>
                <td class="actions">
                  <button @click="handleDeleteMember(member.id)" class="btn-icon-delete" title="Xóa">
                    <span class="icon">󰆴</span>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
          
          <div v-if="members.length === 0" class="empty-placeholder">
            <span class="icon big">󰓵</span>
            <p>Danh sách thành viên trống</p>
          </div>
        </div>
      </div>

      <!-- Modal Add/Edit -->
      <Transition name="fade">
        <div v-if="showForm" class="modal-backdrop" @click.self="closeForm">
          <div class="modal-glass">
            <div class="modal-header">
              <h3>Thêm thành viên mới</h3>
              <button @click="closeForm" class="btn-close">✕</button>
            </div>
            
            <div class="modal-body">
                <p class="modal-note">Hệ thống sẽ tự động tạo tài khoản đăng nhập cho thành viên mới.</p>
                
                <div class="form-grid">
                    <div class="input-group">
                        <label>Họ và tên</label>
                        <input v-model="formData.fullName" type="text" placeholder="Nhập tên đầy đủ" />
                    </div>
                    
                    <div class="input-group">
                        <label>Địa chỉ Email</label>
                        <input v-model="formData.email" type="email" placeholder="example@pcm.com" />
                    </div>

                    <div class="input-group">
                        <label>Số điện thoại</label>
                        <input v-model="formData.phoneNumber" type="tel" placeholder="03XXXXXXXX" />
                    </div>
                </div>
            </div>

            <div class="modal-footer">
              <button @click="closeForm" class="btn-ghost">Hủy bỏ</button>
              <button @click="saveForm" class="btn-gradient">Hoàn tất</button>
            </div>
          </div>
        </div>
      </Transition>
    </main>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

.page-container {
  min-height: 100vh;
  background: #0f172a;
  color: #f8fafc;
  font-family: 'Inter', sans-serif;
  position: relative;
  overflow-x: hidden;
}

.glass-background {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 0% 0%, #3b82f6 0%, transparent 25%),
              radial-gradient(circle at 100% 100%, #8b5cf6 0%, transparent 25%);
  opacity: 0.1;
  pointer-events: none;
  z-index: 0;
}

/* Navbar */
.navbar {
  background: rgba(15, 23, 42, 0.8);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  padding: 1rem 0;
  position: sticky;
  top: 0;
  z-index: 100;
}

.nav-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo-area {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.logo-icon {
  background: linear-gradient(135deg, #3b82f6, #8b5cf6);
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 1.25rem;
}

.logo-area h1 {
  font-size: 1.25rem;
  font-weight: 700;
  letter-spacing: -0.025em;
  background: linear-gradient(to right, #60a5fa, #a78bfa);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.user-controls {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.welcome-text {
  font-size: 0.875rem;
  color: #94a3b8;
}

.btn-logout {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
  border: 1px solid rgba(239, 68, 68, 0.2);
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-logout:hover {
  background: #ef4444;
  color: white;
}

/* Main Layout */
.main-layout {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2.5rem 1.5rem;
  position: relative;
  z-index: 1;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 2rem;
}

.title-section h2 {
  font-size: 1.875rem;
  font-weight: 700;
  margin-bottom: 0.25rem;
}

.subtitle {
  color: #94a3b8;
  font-size: 0.9375rem;
}

.action-section {
  display: flex;
  gap: 0.75rem;
}

.btn-add {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: transform 0.2s, box-shadow 0.2s;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
}

.btn-add:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(37, 99, 235, 0.4);
}

.btn-refresh {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: white;
  width: 42px;
  height: 42px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.btn-refresh:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.1);
}

/* Content Card */
.content-card {
  background: rgba(30, 41, 59, 0.7);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.table-container {
  overflow-x: auto;
}

.premium-table {
  width: 100%;
  border-collapse: collapse;
}

.premium-table th {
  text-align: left;
  padding: 1rem 1.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #94a3b8;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.member-row {
  transition: background 0.2s;
}

.member-row:hover {
  background: rgba(255, 255, 255, 0.02);
}

.premium-table td {
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}

.member-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.avatar-circle {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #1e293b, #334155);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  color: #60a5fa;
  border: 2px solid rgba(96, 165, 250, 0.2);
}

.details .name {
  display: block;
  font-weight: 600;
  color: #f1f5f9;
}

.details .id {
  font-size: 0.75rem;
  color: #64748b;
}

.contact-info .info-item {
  font-size: 0.8125rem;
  color: #cbd5e1;
  display: flex;
  align-items: center;
  gap: 0.4rem;
  margin-bottom: 0.2rem;
}

.date-badge {
  background: rgba(59, 130, 246, 0.1);
  color: #60a5fa;
  padding: 0.25rem 0.6rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
}

.wallet-amount {
  font-weight: 600;
  color: #10b981;
}

.btn-icon-delete {
  background: none;
  border: none;
  color: #64748b;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 6px;
  transition: all 0.2s;
}

.btn-icon-delete:hover {
  color: #ef4444;
  background: rgba(239, 68, 68, 0.1);
}

/* Modal */
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(2, 6, 23, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1.5rem;
}

.modal-glass {
  background: #1e293b;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  width: 100%;
  max-width: 500px;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  font-size: 1.25rem;
  font-weight: 700;
}

.btn-close {
  background: none;
  border: none;
  color: #94a3b8;
  font-size: 1.25rem;
  cursor: pointer;
}

.modal-body {
  padding: 1.5rem;
}

.modal-note {
  font-size: 0.8125rem;
  color: #60a5fa;
  background: rgba(59, 130, 246, 0.1);
  padding: 0.75rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
  border-left: 3px solid #3b82f6;
}

.input-group {
  margin-bottom: 1.25rem;
}

.input-group label {
  display: block;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #94a3b8;
  margin-bottom: 0.5rem;
}

.input-group input {
  width: 100%;
  background: rgba(15, 23, 42, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 0.75rem 1rem;
  color: white;
  transition: border-color 0.2s;
}

.input-group input:focus {
  outline: none;
  border-color: #3b82f6;
  background: rgba(15, 23, 42, 0.8);
}

.modal-footer {
  padding: 1.5rem;
  background: rgba(15, 23, 42, 0.3);
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.btn-ghost {
  background: none;
  border: none;
  color: #94a3b8;
  font-weight: 600;
  padding: 0.5rem 1rem;
}

.btn-gradient {
  background: linear-gradient(135deg, #3b82f6, #8b5cf6);
  color: white;
  border: none;
  padding: 0.75rem 2rem;
  border-radius: 8px;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(139, 92, 246, 0.3);
}

.loading-state {
  padding: 4rem;
  text-align: center;
  color: #94a3b8;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(59, 130, 246, 0.1);
  border-top-color: #3b82f6;
  border-radius: 50%;
  margin: 0 auto 1rem;
  animation: spin 1s linear infinite;
}

.error-toast {
  background: #ef4444;
  color: white;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  animation: shake 0.4s;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-4px); }
  75% { transform: translateX(4px); }
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

.icon {
    font-family: inherit; /* Fallback icon symbols if specialized font not available */
}
</style>
