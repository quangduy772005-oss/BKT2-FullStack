// src/composables/useLoading.ts
import { ref } from "vue";

export const useLoading = (initialValue = false) => {
  const loading = ref(initialValue);

  const setLoading = (value: boolean) => {
    loading.value = value;
  };

  return {
    loading,
    setLoading,
  };
};
