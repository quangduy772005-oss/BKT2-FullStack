import { defineStore } from 'pinia';
import api from '../services/api';
import { ref } from 'vue';

export const useNewsStore = defineStore('news', () => {
    const newsList = ref<any[]>([]);
    const pinnedNews = ref<any[]>([]);

    const fetchNews = async () => {
        try {
            const response = await api.get('/news');
            newsList.value = response.data;
            pinnedNews.value = response.data.filter((n: any) => n.isPinned);
        } catch (error) {
            console.error(error);
        }
    };

    return {
        newsList,
        pinnedNews,
        fetchNews
    };
});
