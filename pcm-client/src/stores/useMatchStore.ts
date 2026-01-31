import { defineStore } from 'pinia';
import api from '../services/api';
import { ref } from 'vue';

export const useMatchStore = defineStore('match', () => {
    const dailyMatches = ref<any[]>([]);

    const fetchMatches = async () => {
        try {
            const response = await api.get('/matches');
            dailyMatches.value = response.data;
        } catch (error) {
            console.error(error);
        }
    };

    const createMatch = async (matchData: any) => {
        try {
            await api.post('/matches', matchData);
        } catch (error) {
            throw error;
        }
    };

    return {
        dailyMatches,
        fetchMatches,
        createMatch
    };
});
