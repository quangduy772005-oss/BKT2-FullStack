import { defineStore } from 'pinia';
import api from '../services/api';
import { ref } from 'vue';
import { useAuthStore } from './useAuthStore';

export const useBookingStore = defineStore('booking', () => {
    const courts = ref<any[]>([]);
    const bookings = ref<any[]>([]);
    const authStore = useAuthStore();

    const fetchCourts = async () => {
        try {
            const response = await api.get('/courts');
            courts.value = response.data;
        } catch (error) {
            console.error(error);
        }
    };

    const fetchAvailableSlots = async (date: string, courtId: number) => {
        try {
            const response = await api.get(`/bookings/available-slots?date=${date}&courtId=${courtId}`);
            return response.data;
        } catch (error) {
            console.error(error);
            return [];
        }
    };

    const createBooking = async (bookingData: any) => {
        try {
            await api.post('/bookings', bookingData);
        } catch (error) {
            throw error;
        }
    };

    return {
        courts,
        bookings,
        fetchCourts,
        fetchAvailableSlots,
        createBooking
    };
});
