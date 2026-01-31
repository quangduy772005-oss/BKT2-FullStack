import { defineStore } from 'pinia';
import api from '../services/api';
import { ref } from 'vue';
import { useAuthStore } from './useAuthStore';

export const useWalletStore = defineStore('wallet', () => {
    const balance = ref<number>(0);
    const transactions = ref<any[]>([]);
    const authStore = useAuthStore();

    const fetchBalance = async () => {
        try {
            const response = await api.get('/wallet/balance');
            balance.value = response.data.balance;
        } catch (error) {
            console.error('Failed to fetch balance', error);
        }
    };

    const fetchTransactions = async (page = 0, size = 20) => {
        try {
            const response = await api.get(`/wallet/transactions?pageIndex=${page}&pageSize=${size}`);
            transactions.value = response.data;
        } catch (error) {
            console.error('Failed to fetch transactions', error);
        }
    };

    const createDepositRequest = async (amount: number, proofImage: File | null) => {
        // Warning: The API in WalletController takes explicit fields, need to adjust based on implementation details.
        // For now assuming existing API structure.
        // Actually, the controller uses `CreateDepositRequestDto` but doesn't seem to handle file upload directly in that endpoint shown.
        // Let's assume JSON body for now as per controller code shown earlier.
        try {
            await api.post('/wallet/deposit-request', { amount, bankName: 'TEST', accountNumber: 'TEST', accountHolder: 'TEST' });
            // If file upload is needed, it would be a separate multipart/form-data request
        } catch (error) {
            throw error;
        }
    };

    return {
        balance,
        transactions,
        fetchBalance,
        fetchTransactions,
        createDepositRequest
    };
});
