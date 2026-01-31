<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import { useTournamentStore } from "../stores/useTournamentStore";
import BracketComponent from "../components/BracketComponent.vue";

const route = useRoute();
const tournamentStore = useTournamentStore();
const activeTab = ref("info");
const loading = ref(false);

onMounted(async () => {
    const id = Number(route.params.id);
    if (id) {
        await tournamentStore.fetchTournamentDetails(id);
        if (tournamentStore.currentTournament?.type !== 0) { // If bracket type
            await tournamentStore.fetchBracket(id);
        }
    }
});

const handleJoin = async () => {
    if (confirm("Bạn có chắc muốn tham gia giải đấu này? Phí tham gia sẽ bị trừ từ ví.")) {
        loading.value = true;
        try {
            await tournamentStore.joinTournament(tournamentStore.currentTournament.id);
            alert("Đăng ký thành công!");
            await tournamentStore.fetchTournamentDetails(tournamentStore.currentTournament.id);
        } catch (error) {
           alert("Lỗi đăng ký: " + (error as any).response?.data || "Không xác định");
        } finally {
            loading.value = false;
        }
    }
}
</script>

<template>
  <div class="detail-container" v-if="tournamentStore.currentTournament">
    <div class="detail-header">
        <h1>{{ tournamentStore.currentTournament.name }}</h1>
        <div class="tabs">
            <button :class="{ active: activeTab === 'info' }" @click="activeTab = 'info'">Thông tin</button>
            <button :class="{ active: activeTab === 'bracket' }" @click="activeTab = 'bracket'">Sơ đồ thi đấu</button>
        </div>
    </div>

    <div v-if="activeTab === 'info'" class="tab-content">
        <div class="info-card">
            <p><strong>Ngày bắt đầu:</strong> {{ new Date(tournamentStore.currentTournament.startDate).toLocaleDateString() }}</p>
            <p><strong>Phí tham gia:</strong> {{ tournamentStore.currentTournament.entryFee?.toLocaleString() }} đ</p>
            <p><strong>Giải thưởng:</strong> {{ tournamentStore.currentTournament.prizePool?.toLocaleString() }} đ</p>
            <p><strong>Mô tả:</strong> {{ tournamentStore.currentTournament.description || 'Chưa có mô tả' }}</p>

            <div class="actions" v-if="tournamentStore.currentTournament.status === 0">
                <button class="btn-join" @click="handleJoin" :disabled="loading">
                    Đăng Ký Tham Gia Ngay
                </button>
            </div>
            <div v-else class="alert-info">
                Giải đấu đã bắt đầu hoặc kết thúc.
            </div>
        </div>
    </div>

    <div v-if="activeTab === 'bracket'" class="tab-content">
        <BracketComponent v-if="tournamentStore.bracketData" :matches="tournamentStore.bracketData" />
        <div v-else class="empty-state">Chưa có sơ đồ thi đấu</div>
    </div>
  </div>
</template>

<style scoped>
.detail-container {
    max-width: 1000px;
    margin: 0 auto;
    padding: 20px;
}

.detail-header {
    margin-bottom: 20px;
}

.tabs {
    display: flex;
    gap: 10px;
    margin-top: 20px;
    border-bottom: 2px solid #eee;
}

.tabs button {
    padding: 10px 20px;
    border: none;
    background: none;
    cursor: pointer;
    font-size: 16px;
    color: #7f8c8d;
}

.tabs button.active {
    color: #667eea;
    border-bottom: 2px solid #667eea;
    font-weight: bold;
}

.info-card {
    background: white;
    padding: 30px;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.btn-join {
    background: #e67e22;
    color: white;
    border: none;
    padding: 15px 30px;
    border-radius: 8px;
    font-size: 18px;
    font-weight: bold;
    cursor: pointer;
    margin-top: 20px;
    width: 100%;
}
</style>
