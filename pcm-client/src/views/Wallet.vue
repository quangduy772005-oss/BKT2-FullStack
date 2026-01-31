<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useWalletStore } from "../stores/useWalletStore";

const walletStore = useWalletStore();
const showDepositModal = ref(false);
const depositAmount = ref(50000);
const bankName = ref("Techcombank");
const accountNumber = ref("");
const accountHolder = ref("");
const loading = ref(false);

onMounted(async () => {
    await walletStore.fetchTransactions();
    await walletStore.fetchBalance();
});

const handleDeposit = async () => {
    if (depositAmount.value < 10000) {
        alert("Số tiền nạp tối thiểu là 10.000đ");
        return;
    }
    
    loading.value = true;
    try {
        await walletStore.createDepositRequest(depositAmount.value, null);
        alert("Yêu cầu nạp tiền đã được gửi! Vui lòng chờ Admin xác nhận.");
        showDepositModal.value = false;
        depositAmount.value = 50000;
    } catch (error) {
        alert("Lỗi khi tạo yêu cầu nạp tiền");
    } finally {
        loading.value = false;
    }
}

const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('vi-VN', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}
</script>

<template>
  <div class="page-container">
    <div class="glass-background"></div>
    
    <main class="main-layout">
      <div class="page-header">
        <div class="title-section">
          <h2>Ví của bạn</h2>
          <p class="subtitle">Quản lý số dư và lịch sử giao dịch</p>
        </div>
      </div>

      <!-- Balance Card -->
      <div class="balance-glass-card">
        <div class="card-glow"></div>
        <div class="balance-main">
            <span class="label">Số dư hiện dụng</span>
            <div class="amount-row">
                <span class="symbol">₫</span>
                <span class="value">{{ walletStore.balance.toLocaleString() }}</span>
            </div>
            <div class="status-indicator">
                <div class="dot active"></div>
                <span>Tài khoản đang hoạt động</span>
            </div>
        </div>
        <button @click="showDepositModal = true" class="btn-deposit-premium">
            <span class="icon">󰐕</span> Nạp tiền ngay
        </button>
      </div>

      <!-- Transactions -->
      <section class="transactions-section">
        <div class="section-header">
            <h3>󰍽 Lịch sử giao dịch</h3>
            <span class="count-badge">{{ walletStore.transactions.length }} Giao dịch</span>
        </div>

        <div class="content-card">
            <div v-if="walletStore.transactions.length === 0" class="empty-state">
                <span class="icon">󰓵</span>
                <p>Bạn chưa có giao dịch nào gần đây.</p>
            </div>
            
            <div v-else class="transaction-list">
                <div v-for="tx in walletStore.transactions" :key="tx.id" class="tx-item">
                    <div class="tx-type-icon" :class="tx.type.toLowerCase()">
                        <span class="icon">{{ tx.type === 'Deposit' ? '󰕒' : '󰕓' }}</span>
                    </div>
                    
                    <div class="tx-info">
                        <span class="tx-title">{{ tx.description || (tx.type === 'Deposit' ? 'Nạp tiền vào ví' : 'Thanh toán dịch vụ') }}</span>
                        <span class="tx-time">{{ formatDate(tx.createdDate) }}</span>
                    </div>

                    <div class="tx-amount" :class="tx.type === 'Deposit' ? 'plus' : 'minus'">
                        {{ tx.type === 'Deposit' ? '+' : '-' }}{{ tx.amount.toLocaleString() }} <small>₫</small>
                    </div>

                    <div class="tx-status">
                        <span class="status-pill" :class="tx.status.toLowerCase()">{{ tx.status }}</span>
                    </div>
                </div>
            </div>
        </div>
      </section>

      <!-- Deposit Modal -->
      <Transition name="fade">
        <div v-if="showDepositModal" class="modal-backdrop" @click.self="showDepositModal = false">
            <div class="modal-glass">
                <div class="modal-header">
                    <h3>Nạp tiền vào tài khoản</h3>
                    <button @click="showDepositModal = false" class="btn-close">✕</button>
                </div>
                
                <div class="modal-body">
                    <div class="qr-placeholder">
                        <div class="qr-mock">QR</div>
                        <p class="qr-hint">Quét mã để thanh toán nhanh hơn</p>
                    </div>

                    <div class="input-group">
                        <label>Số tiền nạp (tối thiểu 10.000đ)</label>
                        <div class="input-with-symbol">
                            <input v-model.number="depositAmount" type="number" step="10000" />
                            <span class="unit">₫</span>
                        </div>
                    </div>

                    <div class="form-grid">
                        <div class="input-group">
                            <label>Ngân hàng</label>
                            <input v-model="bankName" type="text" />
                        </div>
                        <div class="input-group">
                            <label>Số tài khoản gửi</label>
                            <input v-model="accountNumber" type="text" placeholder="0123456789" />
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button @click="showDepositModal = false" class="btn-ghost">Hủy bỏ</button>
                    <button @click="handleDeposit" class="btn-gradient" :disabled="loading">
                        {{ loading ? 'Đang gửi...' : 'Xác nhận đã chuyển' }}
                    </button>
                </div>
            </div>
        </div>
      </Transition>
    </main>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap');

.page-container {
  min-height: 100vh;
  background: #0f172a;
  color: #f1f5f9;
  font-family: 'Inter', sans-serif;
  position: relative;
  overflow-x: hidden;
}

.glass-background {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: radial-gradient(circle at 0% 0%, rgba(16, 185, 129, 0.1), transparent 30%),
              radial-gradient(circle at 100% 100%, rgba(59, 130, 246, 0.1), transparent 30%);
  pointer-events: none;
}

.main-layout {
  max-width: 900px;
  margin: 0 auto;
  padding: 3rem 1.5rem;
  position: relative;
  z-index: 1;
}

.page-header { margin-bottom: 2.5rem; }
.title-section h2 { font-size: 2rem; font-weight: 800; margin-bottom: 0.25rem; }
.subtitle { color: #94a3b8; font-size: 1rem; }

/* Balance Card */
.balance-glass-card {
  position: relative;
  background: linear-gradient(135deg, #059669, #10b981);
  border-radius: 24px;
  padding: 2.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 3rem;
  box-shadow: 0 20px 40px -10px rgba(16, 185, 129, 0.3);
  overflow: hidden;
}

.card-glow {
  position: absolute;
  top: -50%; right: -20%;
  width: 300px; height: 300px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.2), transparent 70%);
  pointer-events: none;
}

.balance-main .label { font-size: 0.875rem; color: rgba(255, 255, 255, 0.8); font-weight: 500; }
.amount-row { display: flex; align-items: baseline; gap: 0.5rem; margin: 0.5rem 0 1rem; }
.amount-row .symbol { font-size: 1.5rem; font-weight: 600; }
.amount-row .value { font-size: 3rem; font-weight: 800; letter-spacing: -0.02em; }

.status-indicator { display: flex; align-items: center; gap: 0.5rem; font-size: 0.75rem; color: rgba(255, 255, 255, 0.9); }
.dot { width: 8px; height: 8px; border-radius: 50%; }
.dot.active { background: #34d399; box-shadow: 0 0 8px #34d399; }

.btn-deposit-premium {
    background: white; color: #059669; border: none; padding: 0.875rem 1.75rem;
    border-radius: 12px; font-weight: 700; font-size: 1rem; cursor: pointer;
    display: flex; align-items: center; gap: 0.5rem; transition: all 0.2s;
}
.btn-deposit-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1); }

/* Transactions */
.section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
.section-header h3 { font-size: 1.25rem; font-weight: 700; }
.count-badge { font-size: 0.75rem; background: rgba(255, 255, 255, 0.05); padding: 0.25rem 0.75rem; border-radius: 100px; color: #94a3b8; }

.content-card {
    background: rgba(30, 41, 59, 0.4); backdrop-filter: blur(12px);
    border: 1px solid rgba(255, 255, 255, 0.05); border-radius: 20px;
    overflow: hidden;
}

.tx-item {
    display: flex; align-items: center; padding: 1.25rem 1.5rem;
    border-bottom: 1px solid rgba(255, 255, 255, 0.05); transition: background 0.2s;
}
.tx-item:hover { background: rgba(255, 255, 255, 0.02); }
.tx-item:last-child { border-bottom: none; }

.tx-type-icon {
    width: 48px; height: 48px; border-radius: 12px;
    display: flex; align-items: center; justify-content: center;
    margin-right: 1.25rem;
}
.tx-type-icon.deposit { background: rgba(16, 185, 129, 0.1); color: #10b981; }
.tx-type-icon.withdrawal { background: rgba(239, 68, 68, 0.1); color: #ef4444; }

.tx-info { flex: 1; display: flex; flex-direction: column; }
.tx-title { font-weight: 600; font-size: 0.9375rem; color: #f1f5f9; }
.tx-time { font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; }

.tx-amount { font-weight: 700; font-size: 1rem; margin-right: 2rem; }
.tx-amount.plus { color: #10b981; }
.tx-amount.minus { color: #ef4444; }

.status-pill {
    font-size: 0.625rem; font-weight: 700; text-transform: uppercase;
    padding: 0.25rem 0.6rem; border-radius: 4px;
    background: rgba(255,255,255,0.05); color: #94a3b8;
}
.status-pill.completed, .status-pill.approved { background: rgba(16, 185, 129, 0.1); color: #10b981; }
.status-pill.pending { background: rgba(245, 158, 11, 0.1); color: #f59e0b; }

/* Modal */
.qr-placeholder {
    background: #f8fafc; border-radius: 12px; padding: 1.5rem;
    text-align: center; margin-bottom: 1.5rem;
}
.qr-mock {
    width: 120px; height: 120px; background: #e2e8f0; margin: 0 auto 1rem;
    display: flex; align-items: center; justify-content: center;
    font-weight: 800; color: #94a3b8; border-radius: 8px;
}
.qr-hint { font-size: 0.75rem; color: #64748b; }

.input-with-symbol { position: relative; }
.input-with-symbol .unit { position: absolute; right: 1rem; top: 50%; transform: translateY(-50%); color: #64748b; font-weight: 600; }

.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-top: 1rem; }

/* Modal Shared - Consistent with other views */
.modal-backdrop { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(2, 6, 23, 0.9); display: flex; align-items: center; justify-content: center; z-index: 1000; }
.modal-glass { background: #1e293b; border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 24px; width: 100%; max-width: 500px; overflow: hidden; }
.modal-header { padding: 1.5rem; border-bottom: 1px solid rgba(255, 255, 255, 0.05); display: flex; justify-content: space-between; align-items: center; }
.modal-body { padding: 1.5rem; }
.input-group label { display: block; font-size: 0.8125rem; color: #94a3b8; margin-bottom: 0.5rem; font-weight: 500; }
.input-group input { width: 100%; background: rgba(15, 23, 42, 0.5); border: 1px solid rgba(255, 255, 255, 0.1); border-radius: 8px; padding: 0.75rem 1rem; color: white; }
.modal-footer { padding: 1.5rem; background: rgba(15, 23, 42, 0.3); display: flex; justify-content: flex-end; gap: 1rem; }
.btn-ghost { background: none; border: none; color: #94a3b8; font-weight: 600; padding: 0.5rem 1rem; cursor: pointer; }
.btn-gradient { background: linear-gradient(135deg, #10b981, #3b82f6); color: white; border: none; padding: 0.75rem 2rem; border-radius: 10px; font-weight: 700; cursor: pointer; }
.btn-close { background: none; border: none; color: #64748b; font-size: 1.25rem; cursor: pointer; }

.empty-state { padding: 4rem; text-align: center; color: #64748b; }
.empty-state .icon { font-size: 3rem; margin-bottom: 1rem; display: block; }

@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
