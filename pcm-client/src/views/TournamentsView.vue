<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useTournamentStore } from "../stores/useTournamentStore";
import { useRouter } from "vue-router";

const tournamentStore = useTournamentStore();
const router = useRouter();

onMounted(async () => {
    await tournamentStore.fetchTournaments();
});

const goToDetail = (id: number) => {
    router.push(`/tournaments/${id}`);
};

const getStatusColor = (status: any) => {
    switch (status) {
        case 0: return 'green'; // Open
        case 1: return 'blue'; // Ongoing
        case 2: return 'gray'; // Finished
        default: return 'black';
    }
};

const getStatusText = (status: any) => {
    switch (status) {
        case 0: return 'Đang mở đăng ký';
        case 1: return 'Đang diễn ra';
        case 2: return 'Đã kết thúc';
        default: return 'Không xác định';
    }
};
</script>

<template>
  <div class="tournaments-container">
    <h1>Giải Đấu Pickleball</h1>
    
    <div v-if="tournamentStore.tournaments.length === 0" class="empty-state">
        Hiện chưa có giải đấu nào.
    </div>

    <div class="tournament-grid">
        <div 
            v-for="tour in tournamentStore.tournaments" 
            :key="tour.id" 
            class="tournament-card"
            @click="goToDetail(tour.id)"
        >
            <div class="card-status" :style="{ backgroundColor: getStatusColor(tour.status) }">
                {{ getStatusText(tour.status) }}
            </div>
            <div class="card-content">
                <h3>{{ tour.name }}</h3>
                <p class="tour-date">📅 {{ new Date(tour.startDate).toLocaleDateString() }}</p>
                <div class="tour-info">
                    <span>🏆 {{ tour.prizePool?.toLocaleString() }} đ</span>
                    <span>🎫 {{ tour.entryFee?.toLocaleString() }} đ</span>
                </div>
            </div>
            <div class="card-footer">
                <button class="btn-detail">Xem Chi Tiết</button>
            </div>
        </div>
    </div>
  </div>
</template>

<style scoped>
.tournaments-container {
    padding: 20px;
    max-width: 1200px;
    margin: 0 auto;
}

.tournament-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 20px;
    margin-top: 20px;
}

.tournament-card {
    background: white;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 4px 10px rgba(0,0,0,0.1);
    transition: transform 0.2s;
    cursor: pointer;
    position: relative;
}

.tournament-card:hover {
    transform: translateY(-5px);
}

.card-status {
    position: absolute;
    top: 10px;
    right: 10px;
    padding: 4px 8px;
    color: white;
    font-size: 12px;
    border-radius: 4px;
    font-weight: bold;
}

.card-content {
    padding: 20px;
    margin-top: 20px;
}

.tour-info {
    display: flex;
    justify-content: space-between;
    margin-top: 15px;
    font-weight: 500;
    color: #2c3e50;
}

.card-footer {
    padding: 15px;
    background: #f8f9fa;
    text-align: center;
}

.btn-detail {
    background: #667eea;
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 4px;
    cursor: pointer;
}
</style>
