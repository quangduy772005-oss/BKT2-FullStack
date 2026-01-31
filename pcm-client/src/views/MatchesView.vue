<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useMatchStore } from "../stores/useMatchStore";
import { getMembers } from "../api/memberApi";

const matchStore = useMatchStore();
const members = ref<any[]>([]);
const showModal = ref(false);
const loading = ref(false);

const matchForm = ref({
    isRanked: true,
    matchFormat: 1, // Singles
    team1Player1Id: "",
    team1Player2Id: null,
    team2Player1Id: "",
    team2Player2Id: null,
    winningSide: 1 // Team 1
});

onMounted(async () => {
    await Promise.all([
        matchStore.fetchMatches(),
        fetchMembersList()
    ]);
});

const fetchMembersList = async () => {
    try {
        const res = await getMembers();
        members.value = res.data;
    } catch (e) { console.error(e); }
};

const submitMatch = async () => {
    loading.value = true;
    try {
        await matchStore.createMatch(matchForm.value);
        alert("Ghi nhận kết quả thành công!");
        showModal.value = false;
        await matchStore.fetchMatches();
    } catch (error) {
        alert("Lỗi: " + (error as any).response?.data || "Không xác định");
    } finally {
        loading.value = false;
    }
};

const getWinningText = (match: any) => {
    if (match.winningSide === 1) return "Team 1 Thắng";
    if (match.winningSide === 2) return "Team 2 Thắng";
    return "Hòa/Chưa đấu";
}
</script>

<template>
  <div class="matches-container">
    <div class="header">
        <h1>Sàn Đấu Hằng Ngày</h1>
        <button class="btn-primary" @click="showModal = true">+ Ghi Kết Quả Trận Đấu</button>
    </div>

    <div class="matches-list">
        <div v-for="match in matchStore.dailyMatches" :key="match.id" class="match-card">
            <div class="match-header">
                <span class="date">{{ new Date(match.date).toLocaleString() }}</span>
                <span class="badge" v-if="match.isRanked">Ranked</span>
            </div>
            <div class="match-body">
                <div class="team team-1" :class="{'winner': match.winningSide === 1}">
                    <p>{{ match.team1Player1?.fullName }}</p>
                    <p v-if="match.team1Player2">{{ match.team1Player2?.fullName }}</p>
                </div>
                <div class="vs">VS</div>
                <div class="team team-2" :class="{'winner': match.winningSide === 2}">
                    <p>{{ match.team2Player1?.fullName }}</p>
                    <p v-if="match.team2Player2">{{ match.team2Player2?.fullName }}</p>
                </div>
            </div>
            <div class="match-footer">
                {{ getWinningText(match) }}
            </div>
        </div>
    </div>

    <!-- Match Modal -->
    <div v-if="showModal" class="modal-overlay">
        <div class="modal-content">
            <h3>Ghi Nhận Kết Quả</h3>
            
            <div class="form-group">
                <label>Thể thức:</label>
                <select v-model="matchForm.matchFormat">
                    <option :value="1">Đánh Đơn (1vs1)</option>
                    <option :value="2">Đánh Đôi (2vs2)</option>
                </select>
            </div>

            <div class="team-setup">
                <div class="team-input">
                    <h4>Team 1</h4>
                    <select v-model="matchForm.team1Player1Id">
                        <option value="">Chọn Player 1</option>
                        <option v-for="m in members" :key="m.id" :value="m.id">{{ m.name }}</option>
                    </select>
                    <select v-if="matchForm.matchFormat === 2" v-model="matchForm.team1Player2Id">
                        <option :value="null">Chọn Player 2</option>
                         <option v-for="m in members" :key="m.id" :value="m.id">{{ m.name }}</option>
                    </select>
                </div>
                 <div class="team-input">
                    <h4>Team 2</h4>
                    <select v-model="matchForm.team2Player1Id">
                        <option value="">Chọn Player 1</option>
                         <option v-for="m in members" :key="m.id" :value="m.id">{{ m.name }}</option>
                    </select>
                    <select v-if="matchForm.matchFormat === 2" v-model="matchForm.team2Player2Id">
                        <option :value="null">Chọn Player 2</option>
                         <option v-for="m in members" :key="m.id" :value="m.id">{{ m.name }}</option>
                    </select>
                </div>
            </div>

            <div class="form-group">
                <label>Kết quả:</label>
                <div class="radio-group">
                    <label><input type="radio" v-model="matchForm.winningSide" :value="1" /> Team 1 Thắng</label>
                    <label><input type="radio" v-model="matchForm.winningSide" :value="2" /> Team 2 Thắng</label>
                </div>
            </div>

             <div class="form-group">
                <label><input type="checkbox" v-model="matchForm.isRanked" /> Tính điểm Rank (DUPR)</label>
            </div>

            <div class="modal-actions">
                <button class="btn-cancel" @click="showModal = false">Hủy</button>
                <button class="btn-confirm" @click="submitMatch" :disabled="loading">
                    Lưu Kết Quả
                </button>
            </div>
        </div>
    </div>
  </div>
</template>

<style scoped>
.matches-container {
    max-width: 800px;
    margin: 0 auto;
    padding: 20px;
}

.header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 30px;
}

.match-card {
    background: white;
    border-radius: 8px;
    padding: 20px;
    margin-bottom: 15px;
    box-shadow: 0 2px 5px rgba(0,0,0,0.05);
}

.match-header {
    display: flex;
    justify-content: space-between;
    margin-bottom: 15px;
    font-size: 14px;
    color: #95a5a6;
}

.badge {
    background: #f1c40f;
    color: white;
    padding: 2px 6px;
    border-radius: 4px;
    font-size: 11px;
    font-weight: bold;
}

.match-body {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.team {
    flex: 1;
    text-align: center;
    padding: 10px;
    border-radius: 6px;
}

.team.winner {
    background-color: #d5f5e3;
    font-weight: bold;
    color: #27ae60;
}

.vs {
    font-weight: bold;
    color: #bdc3c7;
    margin: 0 20px;
}

.match-footer {
    margin-top: 15px;
    text-align: center;
    font-weight: 500;
    color: #2c3e50;
    border-top: 1px solid #dae1e7;
    padding-top: 10px;
}

.modal-overlay {
    position: fixed;
    top: 0; left: 0; right: 0; bottom: 0;
    background: rgba(0,0,0,0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
}

.modal-content {
    background: white;
    padding: 30px;
    border-radius: 12px;
    width: 90%;
    max-width: 600px;
}

.team-setup {
    display: flex;
    gap: 20px;
    margin-bottom: 20px;
}

.team-input { flex: 1; display: flex; flex-direction: column; gap: 10px; }

.btn-primary {
    background: #667eea;
    color: white;
    border: none;
    padding: 10px 20px;
    border-radius: 6px;
    cursor: pointer;
}

.btn-cancel {
    padding: 10px 20px;
    background: #fee;
    color: #c0392b;
    border: none;
    border-radius: 6px;
    cursor: pointer;
}

.btn-confirm {
    padding: 10px 20px;
    background: #27ae60;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
}

.form-group select, .form-group input {
    padding: 10px;
    border: 1px solid #ddd;
    border-radius: 5px;
    width: 100%;
}

.modal-actions {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 20px;
}
</style>
