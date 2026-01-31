<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h1>Đăng Nhập</h1>
        <p>Quản lý Nhân sự</p>
      </div>

      <form @submit.prevent="login" class="login-form">
        <div class="form-group">
          <label for="email">Email</label>
          <input
            id="email"
            v-model="email"
            type="email"
            placeholder="Nhập email của bạn"
            required
          />
        </div>

        <div class="form-group">
          <label for="password">Mật khẩu</label>
          <input
            id="password"
            v-model="password"
            type="password"
            placeholder="Nhập mật khẩu của bạn"
            required
          />
        </div>

        <button type="submit" class="btn-login">Đăng Nhập</button>

        <div v-if="error" class="error-message">
          <span>⚠️ {{ error }}</span>
        </div>
      </form>

      <div class="login-footer">
        <p>Bản demo - Vui lòng liên hệ quản trị viên để cấp tài khoản</p>
      </div>
    </div>

    <div class="login-background">
      <div class="shape shape-1"></div>
      <div class="shape shape-2"></div>
      <div class="shape shape-3"></div>
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
const password = ref("");
const error = ref("");

const login = async () => {
  error.value = "";

  try {
    await authStore.login({
      email: email.value,
      password: password.value,
    });
    router.push("/dashboard");
  } catch (err: any) {
    error.value = err.response?.data?.message || err.response?.data || "Sai tài khoản hoặc mật khẩu";
    console.error(err);
  }
};
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  position: relative;
  overflow: hidden;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}

.login-background {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  z-index: 0;
}

.shape {
  position: absolute;
  opacity: 0.1;
  border-radius: 50%;
}

.shape-1 {
  width: 300px;
  height: 300px;
  background: white;
  top: -100px;
  right: -100px;
}

.shape-2 {
  width: 200px;
  height: 200px;
  background: white;
  bottom: -50px;
  left: -50px;
}

.shape-3 {
  width: 250px;
  height: 250px;
  background: white;
  top: 50%;
  left: 20%;
}

.login-card {
  background: white;
  border-radius: 10px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
  padding: 40px;
  position: relative;
  z-index: 1;
  animation: slideUp 0.5s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h1 {
  color: #2c3e50;
  font-size: 28px;
  margin-bottom: 5px;
}

.login-header p {
  color: #95a5a6;
  font-size: 14px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 8px;
  font-size: 14px;
}

.form-group input {
  padding: 12px 15px;
  border: 1px solid #ecf0f1;
  border-radius: 5px;
  font-size: 14px;
  transition: all 0.3s ease;
  background-color: #f9f9f9;
}

.form-group input:focus {
  outline: none;
  background-color: white;
  border-color: #667eea;
  box-shadow: 0 0 10px rgba(102, 126, 234, 0.2);
}

.form-group input::placeholder {
  color: #bdc3c7;
}

.btn-login {
  padding: 12px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  margin-top: 10px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.btn-login:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-login:active {
  transform: translateY(0);
}

.error-message {
  background-color: #fadbd8;
  color: #c0392b;
  padding: 12px;
  border-radius: 5px;
  font-size: 14px;
  text-align: center;
  border-left: 4px solid #e74c3c;
}

.login-footer {
  text-align: center;
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #ecf0f1;
}

.login-footer p {
  color: #95a5a6;
  font-size: 12px;
}

@media (max-width: 480px) {
  .login-card {
    padding: 30px 20px;
  }

  .login-header h1 {
    font-size: 24px;
  }

  .shape {
    display: none;
  }
}
</style>
