<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getActivities, createActivity, deleteActivity } from "../api/activityApi";
import { getClubs } from "../api/clubApi";
import { useAuthStore } from "../stores/useAuthStore";

const authStore = useAuthStore();
const activities = ref<any[]>([]);
const clubs = ref<any[]>([]);
const showForm = ref(false);
const loading = ref(false);

const formData = ref({
  name: "",
  description: "",
  clubId: null as number | null,
  maxParticipants: 10,
  price: 0
});

onMounted(() => {
  fetchActivities();
  fetchClubs();
});

const fetchActivities = async () => {
  loading.value = true;
  try {
    const res = await getActivities();
    activities.value = res.data;
  } catch (err) {
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const fetchClubs = async () => {
    try {
        const res = await getClubs();
        clubs.value = res.data;
    } catch (err) { console.error(err); }
}

const openForm = () => { showForm.value = true; resetForm(); };
const closeForm = () => { showForm.value = false; };
const resetForm = () => { 
    formData.value = { 
        name: "", 
        description: "", 
        clubId: clubs.value.length > 0 ? clubs.value[0].id : null,
        maxParticipants: 10,
        price: 0
    }; 
};

const saveActivity = async () => {
    try {
        await createActivity(formData.value);
        await fetchActivities();
        closeForm();
    } catch (err) {
        console.error(err);
    }
}

const handleDelete = async (id: number) => {
    if(confirm("Xóa hoạt động này?")) {
        try {
            await deleteActivity(id);
            await fetchActivities();
        } catch (err) {
            console.error(err);
        }
    }
}

const getClubName = (id: number) => {
    const club = clubs.value.find(c => c.id === id);
    return club ? club.name : 'N/A';
}
</script>

<template>
  <div class="page-container">
    <div class="glass-background"></div>
    
    <main class="main-layout">
      <div class="page-header">
        <div class="title-section">
          <h2>Sự kiện & Hoạt động</h2>
          <p class="subtitle">Khám phá và quản lý các hoạt động Pickleball</p>
        </div>
        <div class="action-section">
          <button @click="openForm" class="btn-add">
            <span class="icon">󰐕</span> Tạo hoạt động
          </button>
        </div>
      </div>

      <div class="content-card">
        <div v-if="loading" class="loading-state">
            <div class="spinner"></div>
            <p>Đang chuẩn bị danh sách...</p>
        </div>

        <div v-else class="activities-list">
            <div v-for="act in activities" :key="act.id" class="activity-card">
                <div class="activity-accent" :style="{ background: 'linear-gradient(135deg, #3b82f6, #8b5cf6)' }"></div>
                <div class="activity-main">
                    <div class="info-group">
                        <span class="club-tag">{{ getClubName(act.clubId) }}</span>
                        <h3>{{ act.name }}</h3>
                        <p class="desc">{{ act.description }}</p>
                    </div>
                    
                    <div class="stats-row">
                        <div class="act-stat">
                            <span class="stat-label">Tham gia</span>
                            <span class="stat-val">{{ act.registrations?.length || 0 }}/{{ act.maxParticipants }}</span>
                        </div>
                        <div class="act-stat">
                            <span class="stat-label">Phí tham gia</span>
                            <span class="stat-val primary">{{ act.price.toLocaleString() }} <small>VNĐ</small></span>
                        </div>
                    </div>

                    <div class="act-actions">
                        <button class="btn-register-act">Đăng ký ngay</button>
                        <button @click="handleDelete(act.id)" class="btn-delete-plain">󰆴</button>
                    </div>
                </div>
            </div>
            
            <div v-if="activities.length === 0" class="empty-state">
                <span class="icon">󰓵</span>
                <p>Chưa có hoạt động nào được lên lịch.</p>
            </div>
        </div>
      </div>

      <!-- Modal -->
      <Transition name="fade">
        <div v-if="showForm" class="modal-backdrop" @click.self="closeForm">
          <div class="modal-glass">
            <div class="modal-header">
              <h3>Hoạt động mới</h3>
              <button @click="closeForm" class="btn-close">✕</button>
            </div>
            <div class="modal-body">
                <div class="input-group">
                    <label>Câu lạc bộ</label>
                    <select v-model="formData.clubId" class="premium-select">
                        <option v-for="club in clubs" :key="club.id" :value="club.id">{{ club.name }}</option>
                    </select>
                </div>
                <div class="input-group">
                    <label>Tên hoạt động</label>
                    <input v-model="formData.name" type="text" placeholder="Ví dụ: Giao lưu cuối tuần" />
                </div>
                <div class="input-group">
                    <label>Mô tả chi tiết</label>
                    <textarea v-model="formData.description" placeholder="Nội dung hoạt động..." rows="2"></textarea>
                </div>
                <div class="form-row">
                    <div class="input-group half">
                        <label>Số người tối đa</label>
                        <input v-model.number="formData.maxParticipants" type="number" />
                    </div>
                    <div class="input-group half">
                        <label>Phí (VNĐ)</label>
                        <input v-model.number="formData.price" type="number" />
                    </div>
                </div>
            </div>
            <div class="modal-footer">
              <button @click="closeForm" class="btn-ghost">Bỏ qua</button>
              <button @click="saveActivity" class="btn-gradient">Phát hành</button>
            </div>
          </div>
        </div>
      </Transition>
    </main>
  </div>
</template>

<style scoped>
.page-container { min-height: 100vh; background: #0f172a; color: #f8fafc; position: relative; overflow-x: hidden; }
.glass-background { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: radial-gradient(circle at 50% 0%, rgba(59, 130, 246, 0.1), transparent 50%); pointer-events: none; }
.main-layout { max-width: 1200px; margin: 0 auto; padding: 2.5rem 1.5rem; position: relative; z-index: 1; }

.page-header { display: flex; justify-content: space-between; align-items: flex-end; margin-bottom: 2.5rem; }
.title-section h2 { font-size: 2rem; font-weight: 800; }
.btn-add { background: linear-gradient(135deg, #3b82f6, #8b5cf6); color: white; border: none; padding: 0.75rem 1.5rem; border-radius: 12px; font-weight: 700; box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3); }

/* Activity List */
.activities-list { display: flex; flex-direction: column; gap: 1.5rem; }
.activity-card {
    background: rgba(30, 41, 59, 0.4);
    backdrop-filter: blur(8px);
    border: 1px solid rgba(255, 255, 255, 0.05);
    border-radius: 20px;
    display: flex;
    overflow: hidden;
    transition: all 0.2s;
}
.activity-card:hover { border-color: rgba(59, 130, 246, 0.3); transform: scale(1.01); }
.activity-accent { width: 6px; }

.activity-main { flex: 1; padding: 1.5rem; display: grid; grid-template-columns: 2fr 1fr 1fr; align-items: center; gap: 1.5rem; }

.club-tag { font-size: 0.75rem; font-weight: 700; color: #3b82f6; text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 0.5rem; display: block; }
.activity-main h3 { font-size: 1.25rem; font-weight: 700; margin-bottom: 0.5rem; }
.desc { font-size: 0.875rem; color: #94a3b8; line-height: 1.6; }

.stats-row { display: flex; flex-direction: column; gap: 0.75rem; }
.act-stat { display: flex; flex-direction: column; }
.stat-label { font-size: 0.75rem; color: #64748b; font-weight: 500; }
.stat-val { font-size: 1.125rem; font-weight: 700; }
.stat-val.primary { color: #10b981; }

.act-actions { display: flex; align-items: center; gap: 1.5rem; justify-content: flex-end; }
.btn-register-act {
    background: rgba(59, 130, 246, 0.1); border: 1px solid rgba(59, 130, 246, 0.2); color: #60a5fa;
    padding: 0.6rem 1.5rem; border-radius: 8px; font-weight: 600; cursor: pointer; transition: all 0.2s;
}
.btn-register-act:hover { background: #3b82f6; color: white; }
.btn-delete-plain { background: none; border: none; color: #475569; cursor: pointer; }
.btn-delete-plain:hover { color: #ef4444; }

/* Modal Shared Styles */
.modal-backdrop { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(2, 6, 23, 0.9); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-glass { background: #1e293b; border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 20px; width: 100%; max-width: 500px; overflow: hidden; }
.modal-header { padding: 1.5rem; border-bottom: 1px solid rgba(255, 255, 255, 0.05); display: flex; justify-content: space-between; align-items: center; }
.modal-body { padding: 1.5rem; }
.input-group { margin-bottom: 1.25rem; }
.input-group label { display: block; font-size: 0.8125rem; color: #94a3b8; margin-bottom: 0.5rem; }
.input-group input, .input-group textarea, .premium-select {
  width: 100%; background: rgba(15, 23, 42, 0.5); border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px; padding: 0.75rem 1rem; color: white;
}
.form-row { display: flex; gap: 1rem; }
.half { flex: 1; }
.modal-footer { padding: 1.5rem; background: rgba(15, 23, 42, 0.3); display: flex; justify-content: flex-end; gap: 1rem; }

.loading-state { padding: 4rem; text-align: center; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(59, 130, 246, 0.1); border-top-color: #3b82f6; border-radius: 50%; margin: 0 auto 1rem; animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
</style>
