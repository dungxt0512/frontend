<template>
  <div class="container checkout-page">
    <h1 class="heading-display page-title">THANH TOÁN</h1>

    <div class="checkout-layout">
      <form class="checkout-form card" @submit.prevent="handleSubmit">
        <h3>Thông tin nhận hàng</h3>

        <div class="form-group">
          <label class="form-label">Họ tên người nhận</label>
          <input v-model="form.receiverName" type="text" class="form-input" required />
        </div>

        <div class="form-group">
          <label class="form-label">Số điện thoại</label>
          <input v-model="form.receiverPhone" type="tel" class="form-input" required pattern="[0-9]{9,11}" />
        </div>

        <div class="form-group">
          <label class="form-label">Địa chỉ giao hàng</label>
          <textarea v-model="form.shippingAddress" class="form-input" rows="3" required></textarea>
        </div>

        <h3 class="payment-title">Phương thức thanh toán</h3>
        <div class="payment-options">
          <label class="payment-option" :class="{ active: form.paymentMethod === 'COD' }">
            <input type="radio" v-model="form.paymentMethod" value="COD" />
            <div>
              <strong>Thanh toán khi nhận hàng (COD)</strong>
              <p>Trả tiền mặt cho shipper khi nhận giày.</p>
            </div>
          </label>
          <label class="payment-option" :class="{ active: form.paymentMethod === 'VNPay' }">
            <input type="radio" v-model="form.paymentMethod" value="VNPay" />
            <div>
              <strong>Thanh toán qua VNPay</strong>
              <p>Quét QR hoặc thẻ ATM/Visa/Mastercard qua cổng VNPay.</p>
            </div>
          </label>
        </div>

        <p v-if="error" class="form-error">{{ error }}</p>

        <button type="submit" class="btn btn-primary btn-block submit-btn" :disabled="submitting">
          {{ submitting ? 'Đang xử lý...' : `Đặt hàng — ${formatPrice(finalTotal)}` }}
        </button>
      </form>

      <aside class="order-summary card">
        <h3>Đơn hàng của bạn</h3>
        <div v-for="item in cart.items" :key="item.cartItemId" class="summary-item">
          <span>{{ item.productName }} ({{ item.size }}/{{ item.color }}) x{{ item.quantity }}</span>
          <span>{{ formatPrice(item.subtotal) }}</span>
        </div>
        <div class="summary-row">
          <span>Tạm tính</span>
          <span>{{ formatPrice(cart.totalAmount) }}</span>
        </div>
        <div class="summary-row">
          <span>Vận chuyển</span>
          <span>{{ cart.totalAmount >= 1000000 ? 'Miễn phí' : formatPrice(30000) }}</span>
        </div>
        <div class="summary-row total">
          <span>Tổng cộng</span>
          <span>{{ formatPrice(finalTotal) }}</span>
        </div>
      </aside>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'
import { useCartStore } from '../stores/cart'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const cart = useCartStore()
const auth = useAuthStore()

const submitting = ref(false)
const error = ref('')

const form = reactive({
  receiverName: auth.user?.fullName || '',
  receiverPhone: '',
  shippingAddress: '',
  paymentMethod: 'COD'
})

const finalTotal = computed(() => {
  const shipping = cart.totalAmount >= 1000000 ? 0 : 30000
  return cart.totalAmount + shipping
})

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

async function handleSubmit() {
  if (cart.items.length === 0) {
    error.value = 'Giỏ hàng trống.'
    return
  }

  submitting.value = true
  error.value = ''

  try {
    const { data } = await api.post('/orders', form)

    if (form.paymentMethod === 'VNPay' && data.paymentUrl) {
      // Chuyển hướng người dùng sang cổng VNPay để thanh toán
      window.location.href = data.paymentUrl
    } else {
      cart.clearLocal()
      router.push({ path: '/my-orders', query: { success: '1' } })
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'Đặt hàng thất bại, vui lòng thử lại.'
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  if (cart.items.length === 0) cart.fetchCart()
})
</script>

<style scoped>
.checkout-page { padding: 32px 24px 80px; }
.page-title { font-size: 32px; margin-bottom: 24px; }

.checkout-layout {
  display: grid;
  grid-template-columns: 1fr 360px;
  gap: 32px;
  align-items: start;
}

.checkout-form { padding: 32px; }
.checkout-form h3 { margin-bottom: 20px; font-size: 17px; }
.payment-title { margin-top: 8px; }

.payment-options { display: flex; flex-direction: column; gap: 12px; margin-bottom: 24px; }
.payment-option {
  display: flex;
  gap: 12px;
  padding: 16px;
  border: 1.5px solid var(--border-subtle);
  border-radius: var(--radius-sm);
  cursor: pointer;
  transition: border-color 0.15s ease;
}
.payment-option.active { border-color: var(--accent-orange); background: rgba(255,107,53,0.05); }
.payment-option input { margin-top: 4px; accent-color: var(--accent-orange); }
.payment-option strong { display: block; font-size: 15px; margin-bottom: 4px; }
.payment-option p { font-size: 13px; color: var(--text-secondary); }

.submit-btn { padding: 16px; font-size: 16px; margin-top: 8px; }

.order-summary { padding: 24px; position: sticky; top: 96px; }
.order-summary h3 { margin-bottom: 16px; font-size: 17px; }
.summary-item {
  display: flex;
  justify-content: space-between;
  font-size: 13px;
  color: var(--text-secondary);
  margin-bottom: 10px;
  gap: 12px;
}
.summary-row {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  color: var(--text-secondary);
  border-top: 1px solid var(--border-subtle);
  padding-top: 12px;
  margin-top: 12px;
}
.summary-row.total {
  font-size: 17px;
  font-weight: 800;
  color: var(--text-primary);
}

@media (max-width: 800px) {
  .checkout-layout { grid-template-columns: 1fr; }
}
</style>
