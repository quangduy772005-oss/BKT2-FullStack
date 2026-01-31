import { defineStore } from 'pinia';
import api from '../services/api';
import { ref } from 'vue';
import { useAuthStore } from './useAuthStore';

export const useTournamentStore = defineStore('tournament', () => {
    const tournaments = ref<any[]>([]);
    const currentTournament = ref<any | null>(null);
    const bracketData = ref<any | null>(null);
    const authStore = useAuthStore();

    const fetchTournaments = async () => {
        try {
            const response = await api.get('/tournaments');
            tournaments.value = response.data;
        } catch (error) {
            console.error(error);
        }
    };

    const fetchTournamentDetails = async (id: number) => {
        try {
            const response = await api.get(`/tournaments/${id}`);
            currentTournament.value = response.data;
        } catch (error) {
            console.error(error);
        }
    };

    const fetchBracket = async (id: number) => {
        try {
            const response = await api.get(`/tournaments/${id}/bracket`);
            bracketData.value = response.data;
        } catch (error) {
            console.error(error);
        }
    };

    const joinTournament = async (id: number) => {
        try {
            await api.post(`/tournaments/${id}/join`, {});
        } catch (error) {
            throw error;
        }
    };

    return {
        tournaments,
        currentTournament,
        bracketData,
        fetchTournaments,
        fetchTournamentDetails,
        fetchBracket,
        joinTournament
    };
});
