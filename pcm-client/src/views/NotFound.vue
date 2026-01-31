<script setup lang="ts">
import { useRouter } from "vue-router";

const router = useRouter();

const goHome = () => {
  router.push("/");
};
</script>

<template>
  <div class="not-found">
    <div class="not-found-container">
      <div class="error-code">404</div>
      <h1>Trang không tìm thấy</h1>
      <p>Xin lỗi, trang bạn tìm kiếm không tồn tại.</p>
      <button @click="goHome" class="btn-back">← Quay lại trang chủ</button>
    </div>
  </div>
</template>

<style scoped>
.not-found {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.not-found-container {
  text-align: center;
  color: white;
}

.error-code {
  font-size: 120px;
  font-weight: 700;
  margin-bottom: 20px;
  text-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

h1 {
  font-size: 36px;
  margin-bottom: 10px;
}

p {
  font-size: 18px;
  margin-bottom: 30px;
  opacity: 0.9;
}

.btn-back {
  padding: 12px 30px;
  background-color: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-back:hover {
  background-color: white;
  color: #667eea;
  transform: translateY(-2px);
}
</style>
