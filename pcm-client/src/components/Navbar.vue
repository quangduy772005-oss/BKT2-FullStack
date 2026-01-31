<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAuthStore } from "../stores/useAuthStore";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const isMobileMenuOpen = ref(false);

const menuItems = [
  { label: "Dashboard", icon: "󰕒", path: "/dashboard" },
  { label: "Thành viên", icon: "󰐠", path: "/members" },
  { label: "Câu lạc bộ", icon: "󰊫", path: "/clubs" },
  { label: "Hoạt động", icon: "󰐚", path: "/activities" },
  { label: "Ví tiền", icon: "󰍽", path: "/wallet" },
  { label: "Đặt sân", icon: "󰐖", path: "/booking" },
  { label: "Giải đấu", icon: "󰓵", path: "/tournaments" },
];

const goTo = (path: string) => {
  router.push(path);
  isMobileMenuOpen.value = false;
};

const logout = () => {
  authStore.logout();
  router.push("/login");
};

const isActive = computed(() => {
  return (path: string) => route.path === path;
});
</script>

<template>
  <nav class="premium-nav">
    <div class="nav-container">
      <!-- Logo -->
      <div class="nav-brand" @click="goTo('/dashboard')">
        <div class="brand-logo">P</div>
        <div class="brand-info">
            <span class="brand-name">PCM PRO</span>
            <span class="brand-tag">Pickleball Control</span>
        </div>
      </div>

      <!-- Desktop Menu -->
      <div class="desktop-menu">
        <div v-for="item in menuItems" :key="item.path" class="menu-item-wrapper">
          <button
            @click="goTo(item.path)"
            class="nav-btn"
            :class="{ active: isActive(item.path) }"
          >
            <span class="icon">{{ item.icon }}</span>
            <span class="label">{{ item.label }}</span>
            <div v-if="isActive(item.path)" class="active-dot"></div>
          </button>
        </div>
      </div>

      <!-- Right Actions -->
      <div class="nav-actions">
        <div class="user-pill" v-if="authStore.user">
            <div class="user-avatar">{{ authStore.user.fullName?.charAt(0) }}</div>
            <span class="user-name">{{ authStore.user.fullName }}</span>
        </div>
        
        <button @click="logout" class="btn-logout-circle" title="Đăng xuất">
          <span class="icon">󰍃</span>
        </button>

        <!-- Mobile Toggle -->
        <button class="mobile-toggle" @click="isMobileMenuOpen = !isMobileMenuOpen">
          <span class="icon">{{ isMobileMenuOpen ? '✕' : '󰍜' }}</span>
        </button>
      </div>
    </div>

    <!-- Mobile Drawer -->
    <Transition name="slide">
        <div v-if="isMobileMenuOpen" class="mobile-drawer">
            <div class="drawer-header">
                <h3>Menu</h3>
                <button @click="isMobileMenuOpen = false">✕</button>
            </div>
            <div class="drawer-items">
                <button 
                  v-for="item in menuItems" 
                  :key="item.path"
                  @click="goTo(item.path)"
                  class="drawer-btn"
                  :class="{ active: isActive(item.path) }"
                >
                  <span class="icon">{{ item.icon }}</span>
                  {{ item.label }}
                </button>
            </div>
            <div class="drawer-footer">
                <button @click="logout" class="btn-logout-full">
                    <span class="icon">󰍃</span> Đăng xuất
                </button>
            </div>
        </div>
    </Transition>
  </nav>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

.premium-nav {
  background: rgba(15, 23, 42, 0.8);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  position: sticky;
  top: 0;
  z-index: 1000;
  font-family: 'Inter', sans-serif;
}

.nav-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 1.5rem;
  height: 72px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

/* Brand */
.nav-brand {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
}

.brand-logo {
  width: 38px;
  height: 38px;
  background: linear-gradient(135deg, #3b82f6, #8b5cf6);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 1.25rem;
  color: white;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
}

.brand-info {
    display: flex;
    flex-direction: column;
}

.brand-name {
    font-weight: 800;
    font-size: 1.125rem;
    letter-spacing: -0.02em;
    background: linear-gradient(to right, #60a5fa, #a78bfa);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
}

.brand-tag {
    font-size: 0.625rem;
    color: #64748b;
    text-transform: uppercase;
    font-weight: 600;
    letter-spacing: 0.05em;
}

/* Desktop Menu */
.desktop-menu {
  display: none;
  gap: 0.5rem;
  align-items: center;
}

@media (min-width: 1024px) {
  .desktop-menu { display: flex; }
}

.menu-item-wrapper {
    position: relative;
}

.nav-btn {
  background: none;
  border: none;
  color: #94a3b8;
  padding: 0.6rem 1rem;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.nav-btn:hover {
  background: rgba(255, 255, 255, 0.05);
  color: white;
}

.nav-btn.active {
  background: rgba(59, 130, 246, 0.1);
  color: #60a5fa;
}

.active-dot {
    position: absolute;
    bottom: -15px;
    left: 50%;
    transform: translateX(-50%);
    width: 4px;
    height: 4px;
    background: #3b82f6;
    border-radius: 50%;
    box-shadow: 0 0 8px #3b82f6;
}

/* Actions */
.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-pill {
    display: none;
    align-items: center;
    gap: 0.75rem;
    background: rgba(30, 41, 59, 0.8);
    padding: 0.375rem 0.75rem;
    border-radius: 100px;
    border: 1px solid rgba(255, 255, 255, 0.05);
}

@media (min-width: 640px) {
    .user-pill { display: flex; }
}

.user-avatar {
    width: 24px;
    height: 24px;
    background: #334155;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 0.625rem;
    font-weight: 700;
    color: #60a5fa;
}

.user-name {
    font-size: 0.8125rem;
    font-weight: 600;
    color: #cbd5e1;
}

.btn-logout-circle {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    background: rgba(239, 68, 68, 0.1);
    border: 1px solid rgba(239, 68, 68, 0.2);
    color: #ef4444;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s;
}

.btn-logout-circle:hover {
    background: #ef4444;
    color: white;
}

.mobile-toggle {
    display: flex;
    background: none;
    border: none;
    color: white;
    font-size: 1.5rem;
    cursor: pointer;
}

@media (min-width: 1024px) {
    .mobile-toggle { display: none; }
}

/* Mobile Drawer */
.mobile-drawer {
    position: fixed;
    top: 72px;
    left: 0;
    right: 0;
    bottom: 0;
    background: #0f172a;
    z-index: 999;
    padding: 1.5rem;
    display: flex;
    flex-direction: column;
}

.drawer-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 2rem;
}

.drawer-header h3 { font-size: 1.25rem; font-weight: 700; }
.drawer-header button { background: none; border: none; color: #94a3b8; font-size: 1.25rem; }

.drawer-items {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    flex: 1;
}

.drawer-btn {
    background: none;
    border: none;
    padding: 1rem;
    border-radius: 12px;
    color: #94a3b8;
    display: flex;
    align-items: center;
    gap: 1rem;
    font-size: 1rem;
    font-weight: 500;
    text-align: left;
    transition: all 0.2s;
}

.drawer-btn.active {
    background: rgba(59, 130, 246, 0.1);
    color: #60a5fa;
}

.drawer-footer {
    padding-top: 1rem;
    border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.btn-logout-full {
    width: 100%;
    padding: 1rem;
    background: rgba(239, 68, 68, 0.1);
    border: none;
    border-radius: 12px;
    color: #ef4444;
    font-weight: 600;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
}

/* Transitions */
.slide-enter-active, .slide-leave-active { transition: opacity 0.3s, transform 0.3s; }
.slide-enter-from, .slide-leave-to { opacity: 0; transform: translateY(-10px); }

.icon { font-family: inherit; }
</style>
