import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7154/api", // URL backend của bạn
});

// 🟢 TỰ ĐỘNG GẮN TOKEN
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export default api;
