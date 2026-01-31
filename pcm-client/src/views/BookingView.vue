<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { useBookingStore } from "../stores/useBookingStore";
import { useAuthStore } from "../stores/useAuthStore";
import { useWalletStore } from "../stores/useWalletStore";

const bookingStore = useBookingStore();
const authStore = useAuthStore();
const walletStore = useWalletStore();

const selectedDate = ref(new Date().toISOString().split("T")[0]);
const selectedCourt = ref<any>(null);
const showModal = ref(false);
const loading = ref(false);

const startTime = ref("");
const endTime = ref("");
const notes = ref("");

const timeSlots = Array.from({ length: 15 }, (_, i) => i + 6); // 6h to 20h

onMounted(async () => {
    await bookingStore.fetchCourts();
    if (bookingStore.courts.length > 0) {
        selectedCourt.value = bookingStore.courts[0];
    }
    await loadSlots();
});

watch([selectedDate, selectedCourt], () => {
    loadSlots();
});

const loadSlots = async () => {
    if (selectedCourt.value) {
        await bookingStore.fetchAvailableSlots(selectedDate.value, selectedCourt.value.id);
    }
};

const openBookingModal = (slotHour: number) => {
    startTime.value = `${slotHour.toString().padStart(2, '0')}:00`;
    endTime.value = `${(slotHour + 1).toString().padStart(2, '0')}:00`;
    showModal.value = true;
};

const handleBooking = async () => {
    if (!selectedCourt.value) return;

    const price = selectedCourt.value.pricePerHour;
    if (walletStore.balance < price) {
        alert("Số dư không đủ để đặt sân!");
        return;
    }

    loading.value = true;
    try {
        const startDateTime = `${selectedDate.value}T${startTime.value}`;
        const endDateTime = `${selectedDate.value}T${endTime.value}`;

        await bookingStore.createBooking({
            courtId: selectedCourt.value.id,
            startTime: startDateTime,
            endTime: endDateTime,
            notes: notes.value
        });
        
        alert("Đặt sân thành công!");
        showModal.value = false;
        await loadSlots();
        await walletStore.fetchBalance();
    } catch (error) {
        alert("Đặt sân thất bại: " + (error as any).response?.data || "Lỗi không xác định");
    } finally {
        loading.value = false;
    }
};

const isBooked = (hour: number) => {
    if (!Array.isArray(bookingStore.bookings)) return false; 
    
    return bookingStore.bookings.some((b: any) => {
        const start = new Date(b.startTime).getHours();
        const end = new Date(b.endTime).getHours();
        return hour >= start && hour < end;
    });
};
</script>

<template>
  <div class="page-container">
    <div class="glass-background"></div>
    
    <main class="main-layout">
      <div class="page-header">
        <div class="title-section">
          <h2>Đặt Sân Trực Tuyến</h2>
          <p class="subtitle">Tìm kiếm và giữ chỗ sân chơi của bạn</p>
        </div>
      </div>

      <!-- Filters -->
      <div class="filters-card">
        <div class="filter-group">
            <label>Ngày chơi</label>
            <input v-model="selectedDate" type="date" class="premium-input" />
        </div>
        <div class="filter-group">
            <label>Chọn sân</label>
            <select v-model="selectedCourt" class="premium-select">
                <option v-for="court in bookingStore.courts" :key="court.id" :value="court">
                    {{ court.name }} ({{ court.pricePerHour?.toLocaleString() }}₫ / giờ)
                </option>
            </select>
        </div>
      </div>

      <!-- Schedule -->
      <section class="schedule-section">
        <div class="schedule-header">
            <h3>󰐖 Lịch trống trong ngày</h3>
            <div class="legend">
                <span class="legend-item"><div class="dot available"></div> Trống</span>
                <span class="legend-item"><div class="dot booked"></div> Đã đặt</span>
            </div>
        </div>

        <div class="slots-grid">
            <div v-for="hour in timeSlots" :key="hour" 
                 class="slot-card" 
                 :class="{ 'is-booked': isBooked(hour) }"
                 @click="!isBooked(hour) && openBookingModal(hour)">
                <div class="slot-time">{{ hour }}:00 - {{ hour + 1 }}:00</div>
                <div class="slot-status">
                    <span v-if="!isBooked(hour)" class="status-badge available">ĐẶT NGAY</span>
                    <span v-else class="status-badge booked">HẾT CHỖ</span>
                </div>
            </div>
        </div>
      </section>

      <!-- Booking Modal -->
      <Transition name="fade">
        <div v-if="showModal" class="modal-backdrop" @click.self="showModal = false">
          <div class="modal-glass">
            <div class="modal-header">
              <h3>Xác nhận Đặt sân</h3>
              <button @click="showModal = false" class="btn-close">✕</button>
            </div>
            
            <div class="modal-body">
                <div class="summary-box">
                    <div class="sum-row">
                        <span class="sum-label">Sân:</span>
                        <span class="sum-val">{{ selectedCourt?.name }}</span>
                    </div>
                    <div class="sum-row">
                        <span class="sum-label">Thời gian:</span>
                        <span class="sum-val">{{ startTime }} - {{ endTime }}</span>
                    </div>
                    <div class="sum-row">
                        <span class="sum-label">Ngày:</span>
                        <span class="sum-val">{{ selectedDate }}</span>
                    </div>
                </div>

                <div class="input-group">
                    <label>Ghi chú (tùy chọn)</label>
                    <textarea v-model="notes" placeholder="Yêu cầu đặc biệt..." rows="2"></textarea>
                </div>

                <div class="total-section">
                    <span class="total-label">Tổng thanh toán:</span>
                    <div class="total-amount">
                        {{ selectedCourt?.pricePerHour?.toLocaleString() }} <small>₫</small>
                    </div>
                </div>
            </div>

            <div class="modal-footer">
              <button @click="showModal = false" class="btn-ghost">Hủy bỏ</button>
              <button @click="handleBooking" class="btn-gradient" :disabled="loading">
                <span v-if="loading">Vui lòng chờ...</span>
                <span v-else>Thanh toán & Xác nhận</span>
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </main>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap');

.page-container {
  min-height: 100vh;
  background: #0f172a;
  color: #f1f5f9;
  font-family: 'Inter', sans-serif;
  position: relative;
  overflow-x: hidden;
}

.glass-background {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: radial-gradient(circle at 100% 0%, rgba(59, 130, 246, 0.1), transparent 40%),
              radial-gradient(circle at 0% 100%, rgba(139, 92, 246, 0.1), transparent 40%);
  pointer-events: none;
}

.main-layout {
  max-width: 1100px;
  margin: 0 auto;
  padding: 3rem 1.5rem;
  position: relative;
  z-index: 1;
}

.page-header { margin-bottom: 2.5rem; }
.title-section h2 { font-size: 2.25rem; font-weight: 800; }
.subtitle { color: #94a3b8; margin-top: 0.5rem; }

/* Filters */
.filters-card {
    background: rgba(30, 41, 59, 0.4); backdrop-filter: blur(12px);
    border: 1px solid rgba(255, 255, 255, 0.05); border-radius: 20px;
    padding: 1.5rem 2rem; display: flex; gap: 2rem; margin-bottom: 3rem;
}
.filter-group { flex: 1; }
.filter-group label { display: block; font-size: 0.8125rem; color: #94a3b8; margin-bottom: 0.5rem; font-weight: 500; }
.premium-input, .premium-select {
    width: 100%; background: rgba(15, 23, 42, 0.5); border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 12px; padding: 0.875rem 1.25rem; color: white;
}

/* Schedule */
.schedule-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
.schedule-header h3 { font-size: 1.25rem; font-weight: 700; }
.legend { display: flex; gap: 1.5rem; font-size: 0.8125rem; }
.legend-item { display: flex; align-items: center; gap: 0.5rem; color: #94a3b8; }
.dot { width: 10px; height: 10px; border-radius: 2px; }
.dot.available { background: #3b82f6; }
.dot.booked { background: #334155; }

.slots-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 1rem; }
.slot-card {
    background: rgba(30, 41, 59, 0.4); border: 1px solid rgba(255, 255, 255, 0.05);
    border-radius: 16px; padding: 1.5rem; text-align: center; cursor: pointer; transition: all 0.2s;
}
.slot-card:not(.is-booked):hover {
    transform: translateY(-5px); border-color: #3b82f6; background: rgba(59, 130, 246, 0.05);
}
.slot-card.is-booked { opacity: 0.6; cursor: not-allowed; background: rgba(15, 23, 42, 0.3); }

.slot-time { font-size: 0.875rem; font-weight: 700; color: #f1f5f9; margin-bottom: 1rem; }
.status-badge { font-size: 0.6875rem; font-weight: 800; padding: 0.375rem 0.75rem; border-radius: 100px; }
.status-badge.available { background: rgba(59, 130, 246, 0.1); color: #60a5fa; }
.status-badge.booked { background: rgba(255, 255, 255, 0.05); color: #64748b; }

/* Modal */
.summary-box { background: rgba(15, 23, 42, 0.4); border-radius: 16px; padding: 1.25rem; margin-bottom: 1.5rem; }
.sum-row { display: flex; justify-content: space-between; margin-bottom: 0.75rem; }
.sum-row:last-child { margin-bottom: 0; }
.sum-label { color: #64748b; font-size: 0.875rem; }
.sum-val { font-weight: 600; color: #f1f5f9; }

.total-section { padding-top: 1.5rem; border-top: 1px solid rgba(255, 255, 255, 0.05); text-align: right; }
.total-label { font-size: 0.8125rem; color: #94a3b8; }
.total-amount { font-size: 2rem; font-weight: 800; color: #3b82f6; }
.total-amount small { font-size: 1rem; font-weight: 600; }

/* Modal Shared */
.modal-backdrop { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(2, 6, 23, 0.9); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-glass { background: #1e293b; border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 24px; width: 100%; max-width: 500px; }
.modal-header { padding: 1.5rem; border-bottom: 1px solid rgba(255, 255, 255, 0.05); display: flex; justify-content: space-between; align-items: center; }
.modal-body { padding: 1.5rem; }
.input-group label { display: block; font-size: 0.8125rem; color: #94a3b8; margin-bottom: 0.5rem; }
.input-group textarea { width: 100%; background: rgba(15, 23, 42, 0.5); border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 8px; padding: 0.75rem 1rem; color: white; resize: none; }
.modal-footer { padding: 1.5rem; display: flex; justify-content: flex-end; gap: 1rem; }
.btn-ghost { background: none; border: none; color: #94a3b8; font-weight: 600; padding: 0.5rem 1rem; cursor: pointer; }
.btn-gradient { background: linear-gradient(135deg, #3b82f6, #8b5cf6); color: white; border: none; padding: 0.875rem 2rem; border-radius: 12px; font-weight: 700; cursor: pointer; }
.btn-close { background: none; border: none; color: #64748b; font-size: 1.25rem; cursor: pointer; }

.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
