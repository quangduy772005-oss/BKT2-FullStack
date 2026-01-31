// src/composables/useError.ts
import { ref } from "vue";

export const useError = (initialValue = "") => {
  const error = ref(initialValue);

  const setError = (value: string) => {
    error.value = value;
  };

  const clearError = () => {
    error.value = "";
  };

  return {
    error,
    setError,
    clearError,
  };
};
