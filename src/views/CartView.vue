<template>
  <div class="container cart-page">
    <h1 class="heading-display page-title">GIỎ HÀNG</h1>

    <div v-if="cart.loading" class="state-msg">Đang tải giỏ hàng...</div>

    <div v-else-if="cart.items.length === 0" class="empty-cart">
      <p>Giỏ hàng của bạn đang trống.</p>
      <router-link to="/products" class="btn btn-primary">Tiếp tục mua sắm</router-link>
    </div>

    <div v-else class="cart-layout">
      <div class="cart-items">
        <div v-for="item in cart.items" :key="item.cartItemId" class="cart-item card">
          <img :src="item.imageUrl || placeholder" :alt="item.productName" class="item-img" />
          <div class="item-info">
            <h3>{{ item.productName }}</h3>
            <p class="variant-info">Size {{ item.size }} · {{ item.color }}</p>
            <p class="unit-price">{{ formatPrice(item.unitPrice) }}</p>
          </div>
          <div class="qty-control">
            <button @click="decreaseQty(item)">−</button>
            <span>{{ item.quantity }}</span>
            <button @click="increaseQty(item)" :disabled="item.quantity >= item.availableStock">+</button>
          </div>
          <div class="item-subtotal">{{ formatPrice(item.subtotal) }}</div>
          <button class="remove-btn" @click="removeItem(item)" aria-label="Xóa sản phẩm">✕</button>
        </div>
      </div>

      <aside class="cart-summary card">
        <h3>Tóm tắt đơn hàng</h3>
        <div class="summary-row">
          <span>Tạm tính ({{ cart.totalItems }} sản phẩm)</span>
          <span>{{ formatPrice(cart.totalAmount) }}</span>
        </div>
        <div class="summary-row">
          <span>Phí vận chuyển</span>
          <span>{{ cart.totalAmount >= 1000000 ? 'Miễn phí' : formatPrice(30000) }}</span>
        </div>
        <div class="summary-row total">
          <span>Tổng cộng</span>
          <span>{{ formatPrice(finalTotal) }}</span>
        </div>
        <router-link to="/checkout" class="btn btn-primary btn-block">Tiến hành thanh toán</router-link>
      </aside>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useCartStore } from '../stores/cart'

const cart = useCartStore()
const placeholder = 'https://placehold.co/200x200/161B26/6B7280?text=MaxVerse'

const finalTotal = computed(() => {
  const shipping = cart.totalAmount >= 1000000 ? 0 : 30000
  return cart.totalAmount + shipping
})

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

async function increaseQty(item) {
  if (item.quantity >= item.availableStock) return
  await cart.updateQuantity(item.cartItemId, item.quantity + 1)
}

async function decreaseQty(item) {
  if (item.quantity <= 1) return
  await cart.updateQuantity(item.cartItemId, item.quantity - 1)
}

async function removeItem(item) {
  await cart.removeItem(item.cartItemId)
}

onMounted(() => cart.fetchCart())
</script>

<style scoped>
.cart-page { padding: 32px 24px 80px; }
.page-title { font-size: 32px; margin-bottom: 24px; }

.empty-cart {
  text-align: center;
  padding: 80px 0;
  color: var(--text-secondary);
}
.empty-cart p { margin-bottom: 20px; }

.cart-layout {
  display: grid;
  grid-template-columns: 1fr 340px;
  gap: 32px;
  align-items: start;
}

.cart-item {
  display: grid;
  grid-template-columns: 80px 1fr auto auto auto;
  align-items: center;
  gap: 16px;
  padding: 16px;
  margin-bottom: 12px;
}
.item-img { width: 80px; height: 80px; object-fit: cover; border-radius: var(--radius-sm); }
.item-info h3 { font-size: 15px; margin-bottom: 4px; }
.variant-info { font-size: 13px; color: var(--text-muted); margin-bottom: 4px; }
.unit-price { font-size: 14px; color: var(--accent-orange); font-weight: 700; }

.qty-control {
  display: flex;
  align-items: center;
  border: 1.5px solid var(--border-strong);
  border-radius: var(--radius-sm);
}
.qty-control button { width: 32px; height: 32px; color: var(--text-primary); }
.qty-control button:disabled { opacity: 0.3; cursor: not-allowed; }
.qty-control span { width: 36px; text-align: center; font-size: 14px; }

.item-subtotal { font-weight: 700; white-space: nowrap; }
.remove-btn { color: var(--text-muted); font-size: 16px; padding: 8px; }
.remove-btn:hover { color: var(--danger); }

.cart-summary { padding: 24px; position: sticky; top: 96px; }
.cart-summary h3 { margin-bottom: 20px; font-size: 17px; }
.summary-row {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  color: var(--text-secondary);
  margin-bottom: 14px;
}
.summary-row.total {
  font-size: 17px;
  font-weight: 800;
  color: var(--text-primary);
  border-top: 1px solid var(--border-subtle);
  padding-top: 14px;
  margin-top: 6px;
  margin-bottom: 20px;
}

.state-msg { text-align: center; padding: 60px 0; color: var(--text-muted); }

@media (max-width: 800px) {
  .cart-layout { grid-template-columns: 1fr; }
  .cart-item { grid-template-columns: 60px 1fr; grid-template-areas: "img info" "img qty" "img sub"; }
}
</style>
