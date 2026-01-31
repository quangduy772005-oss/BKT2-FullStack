<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getClubs, createClub, deleteClub } from "../api/clubApi";
import { useAuthStore } from "../stores/useAuthStore";

const authStore = useAuthStore();
const clubs = ref<any[]>([]);
const showForm = ref(false);
const loading = ref(false);

const formData = ref({
  name: "",
  address: "",
  description: ""
});

onMounted(() => {
  fetchClubs();
});

const fetchClubs = async () => {
  loading.value = true;
  try {
    const res = await getClubs();
    clubs.value = res.data;
  } catch (err) {
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const openForm = () => { showForm.value = true; resetForm(); };
const closeForm = () => { showForm.value = false; };
const resetForm = () => { formData.value = { name: "", address: "", description: "" }; };

const saveClub = async () => {
    try {
        await createClub(formData.value);
        await fetchClubs();
        closeForm();
    } catch (err) {
        console.error(err);
    }
}

const handleDelete = async (id: number) => {
    if(confirm("Xóa câu lạc bộ này?")) {
        try {
            await deleteClub(id);
            await fetchClubs();
        } catch (err) {
            console.error(err);
        }
    }
}
</script>

<template>
  <div class="page-container">
    <div class="glass-background"></div>
    
    <main class="main-layout">
      <div class="page-header">
        <div class="title-section">
          <h2>Hệ thống Câu lạc bộ</h2>
          <p class="subtitle">Quản lý mạng lưới câu lạc bộ Pickleball</p>
        </div>
        <div class="action-section">
          <button @click="openForm" class="btn-add">
            <span class="icon">󰐕</span> Thêm câu lạc bộ
          </button>
        </div>
      </div>

      <div class="content-card">
        <div v-if="loading" class="loading-state">
            <div class="spinner"></div>
            <p>Đang tải dữ liệu...</p>
        </div>

        <div v-else class="clubs-grid">
            <div v-for="club in clubs" :key="club.id" class="club-card">
                <div class="club-banner">
                    <div class="club-logo-mini">󰊫</div>
                </div>
                <div class="club-content">
                    <h3>{{ club.name }}</h3>
                    <p class="club-address">󰐚 {{ club.address }}</p>
                    <p class="club-desc">{{ club.description || 'Không có mô tả cho câu lạc bộ này.' }}</p>
                    
                    <div class="club-footer">
                        <span class="member-count">󰐠 {{ club.members?.length || 0 }} Thành viên</span>
                        <button @click="handleDelete(club.id)" class="btn-delete-plain">󰆴</button>
                    </div>
                </div>
            </div>
            
            <div v-if="clubs.length === 0" class="empty-state">
                <span class="icon">󰓵</span>
                <p>Chưa có câu lạc bộ nào được tạo.</p>
            </div>
        </div>
      </div>

      <!-- Modal -->
      <Transition name="fade">
        <div v-if="showForm" class="modal-backdrop" @click.self="closeForm">
          <div class="modal-glass">
            <div class="modal-header">
              <h3>Thêm Câu lạc bộ mới</h3>
              <button @click="closeForm" class="btn-close">✕</button>
            </div>
            <div class="modal-body">
                <div class="input-group">
                    <label>Tên câu lạc bộ</label>
                    <input v-model="formData.name" type="text" placeholder="Ví dụ: PCM Hà Nội" />
                </div>
                <div class="input-group">
                    <label>Địa chỉ</label>
                    <input v-model="formData.address" type="text" placeholder="Nhập địa chỉ đầy đủ" />
                </div>
                <div class="input-group">
                    <label>Mô tả</label>
                    <textarea v-model="formData.description" placeholder="Giới thiệu về CLB..." rows="3"></textarea>
                </div>
            </div>
            <div class="modal-footer">
              <button @click="closeForm" class="btn-ghost">Hủy</button>
              <button @click="saveClub" class="btn-gradient">Tạo ngay</button>
            </div>
          </div>
        </div>
      </Transition>
    </main>
  </div>
</template>

<style scoped>
.page-container {
  min-height: 100vh;
  background: #0f172a;
  color: #f8fafc;
  position: relative;
  overflow-x: hidden;
}

.glass-background {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 5% 5%, rgba(59, 130, 246, 0.1), transparent 30%),
              radial-gradient(circle at 95% 95%, rgba(139, 92, 246, 0.1), transparent 30%);
  pointer-events: none;
}

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
  margin-bottom: 2.5rem;
}

.title-section h2 { font-size: 2rem; font-weight: 800; margin-bottom: 0.25rem; }
.subtitle { color: #94a3b8; font-size: 1rem; }

.btn-add {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 10px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
}

/* Clubs Grid */
.clubs-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 2rem;
}

.club-card {
    background: rgba(30, 41, 59, 0.4);
    backdrop-filter: blur(12px);
    border: 1px solid rgba(255, 255, 255, 0.05);
    border-radius: 20px;
    overflow: hidden;
    transition: all 0.3s;
}

.club-card:hover {
    transform: translateY(-8px);
    border-color: rgba(255, 255, 255, 0.15);
    background: rgba(30, 41, 59, 0.6);
}

.club-banner {
    height: 120px;
    background: linear-gradient(135deg, #1e293b, #334155);
    display: flex;
    align-items: center;
    justify-content: center;
}

.club-logo-mini {
    width: 60px;
    height: 60px;
    background: rgba(59, 130, 246, 0.1);
    border-radius: 15px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 2rem;
    color: #3b82f6;
    border: 1px solid rgba(59, 130, 246, 0.3);
}

.club-content { padding: 1.5rem; }
.club-content h3 { font-size: 1.25rem; margin-bottom: 0.5rem; font-weight: 700; }
.club-address { font-size: 0.8125rem; color: #94a3b8; margin-bottom: 1rem; }
.club-desc { font-size: 0.875rem; color: #cbd5e1; line-height: 1.6; height: 3.2em; overflow: hidden; margin-bottom: 1.5rem; }

.club-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-top: 1rem;
    border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.member-count { font-size: 0.75rem; color: #64748b; font-weight: 600; }
.btn-delete-plain { background: none; border: none; color: #64748b; cursor: pointer; }
.btn-delete-plain:hover { color: #ef4444; }

/* Modal Stlyes - Shared with Members.vue */
.modal-backdrop {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(2, 6, 23, 0.9);
  display: flex; align-items: center; justify-content: center;
  z-index: 1000; padding: 1.5rem;
}

.modal-glass {
  background: #1e293b;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  width: 100%; max-width: 500px;
  overflow: hidden;
}

.modal-header { padding: 1.5rem; border-bottom: 1px solid rgba(255, 255, 255, 0.05); display: flex; justify-content: space-between; align-items: center; }
.modal-body { padding: 1.5rem; }
.input-group { margin-bottom: 1.25rem; }
.input-group label { display: block; font-size: 0.8125rem; color: #94a3b8; margin-bottom: 0.5rem; }
.input-group input, .input-group textarea {
  width: 100%; background: rgba(15, 23, 42, 0.5); border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px; padding: 0.75rem 1rem; color: white;
}
.modal-footer { padding: 1.5rem; background: rgba(15, 23, 42, 0.3); display: flex; justify-content: flex-end; gap: 1rem; }
.btn-ghost { background: none; border: none; color: #94a3b8; font-weight: 600; padding: 0.5rem 1rem; cursor: pointer; }
.btn-gradient {
  background: linear-gradient(135deg, #3b82f6, #8b5cf6);
  color: white; border: none; padding: 0.75rem 2rem; border-radius: 8px; font-weight: 700; cursor: pointer;
}

.loading-state { padding: 4rem; text-align: center; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(59, 130, 246, 0.1); border-top-color: #3b82f6; border-radius: 50%; margin: 0 auto 1rem; animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
