// src/config/constants.ts

// Default to the API project's launch URL (update with env VITE_API_BASE_URL if needed)
// Default to the relative proxy path
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "/api";

export const APP_NAME = "PCM - Quản lý Nhân sự";

export const DEFAULT_PAGE_SIZE = 10;

export const TOAST_DURATION = 3000;

// Colors
export const COLORS = {
  primary: "#667eea",
  secondary: "#764ba2",
  success: "#27ae60",
  danger: "#e74c3c",
  warning: "#f39c12",
  info: "#3498db",
  text: "#2c3e50",
  textLight: "#95a5a6",
  background: "#f5f6fa",
  border: "#ecf0f1",
};
