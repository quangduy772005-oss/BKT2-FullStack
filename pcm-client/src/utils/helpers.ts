// src/utils/helpers.ts

export const formatDate = (date: Date | string): string => {
  const d = new Date(date);
  return d.toLocaleDateString("vi-VN");
};

export const formatTime = (date: Date | string): string => {
  const d = new Date(date);
  return d.toLocaleTimeString("vi-VN");
};

export const formatDateTime = (date: Date | string): string => {
  return `${formatDate(date)} ${formatTime(date)}`;
};

export const truncate = (text: string, length: number): string => {
  return text.length > length ? text.substring(0, length) + "..." : text;
};

export const capitalizeFirstLetter = (text: string): string => {
  return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
};

export const generateInitials = (fullName: string): string => {
  const names = fullName.split(" ");
  return names
    .map((name) => name.charAt(0).toUpperCase())
    .join("")
    .substring(0, 2);
};

export const isValidEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

export const isValidPhoneNumber = (phone: string): boolean => {
  const phoneRegex = /^[0-9]{10,11}$/;
  return phoneRegex.test(phone.replace(/\D/g, ""));
};
