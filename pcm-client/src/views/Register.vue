<template>
  <div class="register-container">
    <div class="card">
      <h2>Đăng ký</h2>

      <form @submit.prevent="registerUser">
        <div class="form-group">
          <label>Email</label>
          <input v-model="email" type="email" required />
        </div>

        <div class="form-group">
          <label>Họ tên</label>
          <input v-model="fullName" type="text" />
        </div>

        <div class="form-group">
          <label>Mật khẩu</label>
          <input v-model="password" type="password" required />
        </div>

        <button class="btn" type="submit">Đăng ký</button>
      </form>

      <div v-if="message" class="message">{{ message }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../stores/useAuthStore";

const router = useRouter();
const authStore = useAuthStore();
const email = ref("");
const fullName = ref("");
const password = ref("");
const message = ref("");

const registerUser = async () => {
  message.value = "";
  try {
    await authStore.register({
        email: email.value,
        password: password.value,
        fullName: fullName.value
    });
    message.value = "Đăng ký thành công. Vui lòng đăng nhập.";
    setTimeout(() => router.push('/login'), 1200);
  } catch (err: any) {
    message.value = err?.response?.data || 'Lỗi khi đăng ký';
  }
};
</script>

<style scoped>
.register-container{min-height:100vh;display:flex;align-items:center;justify-content:center;background:#f5f6fa}
.card{background:white;padding:28px;border-radius:8px;box-shadow:0 8px 30px rgba(0,0,0,0.08);width:360px}
.form-group{display:flex;flex-direction:column;margin-bottom:12px}
.form-group input{padding:10px;border:1px solid #e6e9ee;border-radius:6px}
.btn{padding:10px 14px;background:#667eea;color:white;border:none;border-radius:6px;cursor:pointer}
.message{margin-top:12px;color:#2c3e50}
</style>
