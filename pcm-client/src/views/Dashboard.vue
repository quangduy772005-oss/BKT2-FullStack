<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../stores/useAuthStore";
import { useWalletStore } from "../stores/useWalletStore";
import { useNewsStore } from "../stores/useNewsStore";
import api from "../api/axios"; // Fixed import path

const router = useRouter();
const authStore = useAuthStore();
const walletStore = useWalletStore();
const newsStore = useNewsStore();

const topMembers = ref<any[]>([]);
const loading = ref(false);

onMounted(async () => {
  if (!authStore.isAuthenticated) {
    router.push("/login");
    return;
  }
  
  loading.value = true;
  try {
    await Promise.all([
        authStore.fetchUserProfile(),
        walletStore.fetchBalance(),
        newsStore.fetchNews(),
        fetchTopRanking()
    ]);
  } catch (error) {
    console.error("Lỗi tải dashboard:", error);
  } finally {
    loading.value = false;
  }
});

const fetchTopRanking = async () => {
    try {
        const res = await api.get('/members');
        if (res.data && Array.isArray(res.data)) {
             // Sort by rankELO descending and take top 5
             topMembers.value = [...res.data]
                .sort((a: any, b: any) => (b.rankELO || 0) - (a.rankELO || 0))
                .slice(0, 5);
        }
    } catch (e) {
        console.error(e);
    }
}

const logout = () => {
    authStore.logout();
    router.push("/login");
};

const goTo = (path: string) => router.push(path);
</script>

<template>
  <div class="page-container">
    <div class="glass-background"></div>
    
    <main class="main-layout">
      <!-- Welcome Header -->
      <section class="welcome-banner">
        <div class="banner-content">
          <h1>Chào mừng trở lại, <span class="gradient-text">{{ authStore.user?.fullName || "Bạn" }}</span>! 👋</h1>
          <p class="subtitle">Hôm nay là một ngày tuyệt vời để chơi Pickleball.</p>
        </div>
        
        <div v-if="walletStore.balance < 100000" class="low-balance-alert">
            <span class="icon">󰀦</span>
            <div class="alert-text">
                <strong>Số dư ví thấp!</strong>
                <span>Vui lòng nạp thêm để đảm bảo việc đặt sân không bị gián đoạn.</span>
            </div>
            <button @click="goTo('/wallet')" class="btn-deposit-small">Nạp ngay</button>
        </div>
      </section>

      <!-- Stats Overview -->
      <div class="stats-grid">
        <div class="stat-glass-card wallet" @click="goTo('/wallet')">
          <div class="card-glow"></div>
          <div class="stat-icon-box">󰍽</div>
          <div class="stat-details">
            <span class="label">Số dư hiện tại</span>
            <div class="value">{{ walletStore.balance.toLocaleString() }} <span class="currency">VNĐ</span></div>
          </div>
        </div>

        <div class="stat-glass-card rank">
          <div class="card-glow"></div>
          <div class="stat-icon-box">󰓵</div>
          <div class="stat-details">
            <span class="label">Chỉ số DUPR</span>
            <div class="value">{{ authStore.user?.rankELO || 1200 }} <span class="unit">Points</span></div>
          </div>
        </div>

        <div class="stat-glass-card news">
          <div class="card-glow"></div>
          <div class="stat-icon-box">󰐣</div>
          <div class="stat-details">
            <span class="label">Tin tức cộng đồng</span>
            <div class="value">{{ newsStore.newsList.length }} <span class="unit">Cập nhật</span></div>
          </div>
        </div>
      </div>

      <!-- Main Dashboard Content -->
      <div class="dashboard-grid">
        <!-- Left: News & Events -->
        <div class="content-section news-panel">
            <div class="section-header">
                <h3>󰐣 Tin tức mới nhất</h3>
                <button class="btn-link" @click="goTo('/news')">Xem tất cả</button>
            </div>
            
            <div v-if="newsStore.newsList.length === 0" class="empty-state">
                <span class="icon">󰓵</span>
                <p>Không có tin tức nào mới nhất lúc này.</p>
            </div>
            
            <div v-else class="news-scroll">
                <div v-for="news in newsStore.newsList.slice(0, 3)" :key="news.id" class="premium-news-card">
                    <div class="news-line"></div>
                    <div class="news-body">
                        <h4>{{ news.title }}</h4>
                        <p>{{ news.summary || news.content.substring(0, 120) }}...</p>
                        <span class="news-date">Hôm nay</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- Right: Leaderboard -->
        <div class="content-section ranking-panel">
            <div class="section-header">
                <h3>󰓰 Bảng xếp hạng</h3>
                <span class="count-badge">TOP 5</span>
            </div>

            <div class="ranking-container">
                <div v-for="(member, index) in topMembers" :key="member.id" class="leader-item" :class="'rank-' + (index + 1)">
                    <div class="rank-pos">
                        <span v-if="index < 3" class="trophy">󰓰</span>
                        <span v-else>{{ index + 1 }}</span>
                    </div>
                    <div class="leader-avatar">{{ member.fullName?.charAt(0) }}</div>
                    <div class="leader-name">{{ member.fullName }}</div>
                    <div class="leader-points">{{ (member.rankELO || 0).toFixed(0) }}</div>
                </div>
            </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <section class="quick-actions">
          <h3>Thao tác nhanh</h3>
          <div class="actions-row">
              <button @click="goTo('/booking')" class="action-btn booking">
                  <span class="icon">󰐖</span>
                  <span>Đặt sân mới</span>
              </button>
              <button @click="goTo('/matches')" class="action-btn matches">
                  <span class="icon">󰐚</span>
                  <span>Tìm trận đấu</span>
              </button>
              <button @click="goTo('/members')" class="action-btn club">
                  <span class="icon">󰐠</span>
                  <span>Kết nối thành viên</span>
              </button>
          </div>
      </section>
    </main>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

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
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 10% 20%, rgba(59, 130, 246, 0.15), transparent 40%),
              radial-gradient(circle at 90% 80%, rgba(139, 92, 246, 0.15), transparent 40%);
  pointer-events: none;
}

.main-layout {
  max-width: 1200px;
  margin: 0 auto;
  padding: 3rem 1.5rem;
  position: relative;
  z-index: 1;
}

/* Welcome Banner */
.welcome-banner {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 3rem;
  flex-wrap: wrap;
  gap: 2rem;
}

.banner-content h1 {
  font-size: 2.25rem;
  font-weight: 800;
  margin-bottom: 0.5rem;
  letter-spacing: -0.025em;
}

.gradient-text {
  background: linear-gradient(135deg, #60a5fa, #a78bfa);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.subtitle {
  color: #94a3b8;
  font-size: 1.125rem;
}

.low-balance-alert {
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 12px;
  padding: 1rem 1.5rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  animation: fadeIn 0.5s ease-out;
}

.alert-text {
    display: flex;
    flex-direction: column;
}

.alert-text strong { color: #ef4444; font-size: 0.875rem; }
.alert-text span { color: #f87171; font-size: 0.75rem; }

.btn-deposit-small {
    background: #ef4444;
    color: white;
    border: none;
    padding: 0.4rem 1rem;
    border-radius: 6px;
    font-size: 0.75rem;
    font-weight: 700;
    cursor: pointer;
}

/* Stats Cards */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.stat-glass-card {
  position: relative;
  background: rgba(30, 41, 59, 0.6);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 2rem;
  display: flex;
  align-items: center;
  gap: 1.5rem;
  cursor: pointer;
  transition: all 0.3s;
  overflow: hidden;
}

.stat-glass-card:hover {
  transform: translateY(-5px);
  border-color: rgba(255, 255, 255, 0.2);
  background: rgba(30, 41, 59, 0.8);
}

.card-glow {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle at center, rgba(59, 130, 246, 0.1), transparent 50%);
  pointer-events: none;
}

.stat-icon-box {
  width: 64px;
  height: 64px;
  background: rgba(59, 130, 246, 0.1);
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  color: #3b82f6;
}

.wallet .stat-icon-box { color: #10b981; background: rgba(16, 185, 129, 0.1); }
.rank .stat-icon-box { color: #f59e0b; background: rgba(245, 158, 11, 0.1); }

.stat-details .label {
  display: block;
  font-size: 0.8125rem;
  color: #94a3b8;
  margin-bottom: 0.25rem;
  font-weight: 500;
}

.stat-details .value {
  font-size: 1.75rem;
  font-weight: 700;
}

.stat-details .currency, .stat-details .unit {
  font-size: 0.875rem;
  color: #64748b;
  margin-left: 0.25rem;
}

/* Dashboard Grid */
.dashboard-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 2rem;
  margin-bottom: 3rem;
}

.content-section {
  background: rgba(30, 41, 59, 0.4);
  backdrop-filter: blur(8px);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 20px;
  padding: 1.5rem;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-header h3 {
  font-size: 1.125rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-link {
  background: none;
  border: none;
  color: #3b82f6;
  font-size: 0.8125rem;
  font-weight: 600;
  cursor: pointer;
}

/* News Cards */
.news-scroll {
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.premium-news-card {
    display: flex;
    background: rgba(15, 23, 42, 0.3);
    border-radius: 12px;
    overflow: hidden;
    transition: transform 0.2s;
    cursor: pointer;
}

.premium-news-card:hover {
    transform: scale(1.02);
    background: rgba(15, 23, 42, 0.5);
}

.news-line {
    width: 4px;
    background: #3b82f6;
}

.news-body { padding: 1rem; }
.news-body h4 { font-size: 0.9375rem; margin-bottom: 0.4rem; color: #f1f5f9; }
.news-body p { font-size: 0.8125rem; color: #94a3b8; line-height: 1.5; margin-bottom: 0.5rem; }
.news-date { font-size: 0.75rem; color: #64748b; font-style: italic; }

/* Leaderboard */
.ranking-container {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.leader-item {
    display: flex;
    align-items: center;
    background: rgba(255, 255, 255, 0.02);
    padding: 0.75rem 1rem;
    border-radius: 12px;
    transition: all 0.2s;
}

.leader-item:hover { background: rgba(255, 255, 255, 0.05); }

.rank-pos { width: 30px; font-weight: 700; color: #94a3b8; }
.rank-1 .rank-pos { color: #f59e0b; }
.rank-2 .rank-pos { color: #94a3b8; }
.rank-3 .rank-pos { color: #b45309; }

.leader-avatar {
    width: 32px;
    height: 32px;
    background: #334155;
    border-radius: 50%;
    margin: 0 1rem;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 0.75rem;
    font-weight: 700;
    color: #60a5fa;
}

.leader-name { flex: 1; font-size: 0.875rem; font-weight: 500; }
.leader-points { font-weight: 700; color: #f59e0b; font-size: 0.9375rem; }

/* Quick Actions */
.quick-actions h3 { font-size: 1.125rem; margin-bottom: 1.5rem; font-weight: 700; }
.actions-row {
    display: flex;
    gap: 1.25rem;
    flex-wrap: wrap;
}

.action-btn {
    flex: 1;
    min-width: 200px;
    background: rgba(30, 41, 59, 0.6);
    border: 1px solid rgba(255, 255, 255, 0.08);
    border-radius: 16px;
    padding: 1.5rem;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.75rem;
    color: white;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
}

.action-btn .icon { font-size: 2rem; }
.action-btn:hover { background: #3b82f6; transform: translateY(-4px); box-shadow: 0 10px 20px rgba(59, 130, 246, 0.2); }

@media (max-width: 1024px) {
    .dashboard-grid { grid-template-columns: 1fr; }
}

@keyframes fadeIn {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}
</style>
