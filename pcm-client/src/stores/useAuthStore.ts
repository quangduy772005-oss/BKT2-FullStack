import { defineStore } from 'pinia';
import api from '../services/api'; // Use our configured api instance
import { ref, computed } from 'vue';

export const useAuthStore = defineStore('auth', () => {
    const user = ref<any | null>(JSON.parse(localStorage.getItem('user') || 'null'));
    const token = ref<string | null>(localStorage.getItem('token'));
    const isAuthenticated = computed(() => !!token.value);
    const roles = ref<string[]>([]);

    const login = async (credentials: any) => {
        try {
            console.log("Attempting login with:", credentials.email);
            const response = await api.post('/auth/login', credentials);

            // Handle both camelCase and PascalCase (backward compatibility)
            token.value = response.data.accessToken || response.data.AccessToken;
            const userData = response.data.user || response.data.User;

            console.log("Login successful! Token received:", !!token.value);

            if (token.value) {
                // Set header immediately for subsequent calls in this session
                api.defaults.headers.common['Authorization'] = `Bearer ${token.value}`;

                localStorage.setItem('token', token.value);
                if (userData) {
                    user.value = userData;
                    localStorage.setItem('user', JSON.stringify(userData));
                }
                await fetchUserProfile();
            }
        } catch (error: any) {
            console.error("Login Error:", error.response?.data || error.message);
            throw error;
        }
    };

    const register = async (userData: any) => {
        try {
            await api.post('/auth/register', userData);
        } catch (error) {
            throw error;
        }
    };

    const logout = () => {
        token.value = null;
        user.value = null;
        roles.value = [];
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    };

    const fetchUserProfile = async () => {
        if (!token.value) return;
        try {
            const response = await api.get('/auth/me'); // Interceptor handles token
            user.value = response.data;
        } catch (error) {
            logout();
        }
    };

    return {
        user,
        token,
        isAuthenticated,
        roles,
        login,
        register,
        logout,
        fetchUserProfile
    };
});
