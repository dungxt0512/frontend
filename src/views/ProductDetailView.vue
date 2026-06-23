<template>
  <div v-if="product" class="container detail-page">
    <div class="detail-grid">
      <!-- IMAGES -->
      <div class="gallery">
        <div class="main-img">
          <img :src="activeImage" :alt="product.productName" />
        </div>
        <div v-if="allImages.length > 1" class="thumb-row">
          <button v-for="(img, i) in allImages" :key="i" class="thumb" :class="{ active: activeImage === img }" @click="activeImage = img">
            <img :src="img" :alt="`${product.productName} ${i + 1}`" />
          </button>
        </div>
      </div>

      <!-- INFO -->
      <div class="info-panel">
        <p class="eyebrow">{{ product.brandName }} · {{ product.categoryName }}</p>
        <h1 class="heading-display product-name">{{ product.productName }}</h1>

        <div class="price-row">
          <span class="price">{{ formatPrice(product.discountPrice || product.price) }}</span>
          <span v-if="product.discountPrice" class="price-old">{{ formatPrice(product.price) }}</span>
        </div>

        <p class="description">{{ product.description }}</p>

        <!-- COLOR SELECT -->
        <div class="select-group">
          <h4>Màu sắc: <span class="selected-value">{{ selectedColor || 'Chọn màu' }}</span></h4>
          <div class="options-row">
            <button
              v-for="color in availableColors"
              :key="color"
              class="size-tag"
              :class="{ active: selectedColor === color }"
              @click="selectColor(color)"
            >{{ color }}</button>
          </div>
        </div>

        <!-- SIZE SELECT -->
        <div class="select-group">
          <h4>Size: <span class="selected-value">{{ selectedSize || 'Chọn size' }}</span></h4>
          <div class="options-row">
            <button
              v-for="variant in variantsForSelectedColor"
              :key="variant.variantId"
              class="size-tag"
              :class="{ active: selectedSize === variant.size, disabled: variant.quantity === 0 }"
              :disabled="variant.quantity === 0"
              @click="selectedSize = variant.size"
            >{{ variant.size }}</button>
          </div>
        </div>

        <p v-if="selectedVariant" class="stock-info">
          Còn {{ selectedVariant.quantity }} sản phẩm
        </p>

        <div class="qty-row">
          <h4>Số lượng</h4>
          <div class="qty-control">
            <button @click="quantity > 1 && quantity--">−</button>
            <span>{{ quantity }}</span>
            <button @click="quantity < (selectedVariant?.quantity || 1) && quantity++">+</button>
          </div>
        </div>

        <button class="btn btn-primary btn-block add-btn" :disabled="!selectedVariant || selectedVariant.quantity === 0 || adding" @click="handleAddToCart">
          {{ adding ? 'Đang thêm...' : 'Thêm vào giỏ hàng' }}
        </button>

        <p v-if="message" class="msg" :class="messageType">{{ message }}</p>
      </div>
    </div>
  </div>
  <div v-else-if="loading" class="container state-msg">Đang tải sản phẩm...</div>
  <div v-else class="container state-msg">Không tìm thấy sản phẩm.</div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'
import { useAuthStore } from '../stores/auth'
import { useCartStore } from '../stores/cart'

const props = defineProps({ id: { type: [String, Number], required: true } })
const router = useRouter()
const auth = useAuthStore()
const cart = useCartStore()

const product = ref(null)
const loading = ref(true)
const selectedColor = ref(null)
const selectedSize = ref(null)
const quantity = ref(1)
const adding = ref(false)
const message = ref('')
const messageType = ref('success')
const activeImage = ref('')

const placeholder = 'https://placehold.co/600x600/161B26/6B7280?text=MaxVerse'

const allImages = computed(() => {
  if (!product.value) return [placeholder]
  const imgs = [product.value.imageUrl, ...product.value.images].filter(Boolean)
  return imgs.length ? imgs : [placeholder]
})

const availableColors = computed(() => {
  if (!product.value) return []
  return [...new Set(product.value.variants.map(v => v.color))]
})

const variantsForSelectedColor = computed(() => {
  if (!product.value || !selectedColor.value) return []
  return product.value.variants.filter(v => v.color === selectedColor.value)
})

const selectedVariant = computed(() => {
  if (!product.value) return null
  return product.value.variants.find(v => v.color === selectedColor.value && v.size === selectedSize.value) || null
})

function selectColor(color) {
  selectedColor.value = color
  selectedSize.value = null
  quantity.value = 1
}

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}

async function loadProduct() {
  loading.value = true
  try {
    const { data } = await api.get(`/products/${props.id}`)
    product.value = data
    activeImage.value = data.imageUrl || placeholder
    if (availableColors.value.length === 1) selectedColor.value = availableColors.value[0]
  } catch (err) {
    product.value = null
  } finally {
    loading.value = false
  }
}

async function handleAddToCart() {
  if (!auth.isLoggedIn) {
    router.push({ path: '/login', query: { redirect: router.currentRoute.value.fullPath } })
    return
  }
  if (!selectedVariant.value) return

  adding.value = true
  message.value = ''
  try {
    await cart.addToCart(selectedVariant.value.variantId, quantity.value)
    message.value = 'Đã thêm vào giỏ hàng!'
    messageType.value = 'success'
  } catch (err) {
    message.value = err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại.'
    messageType.value = 'error'
  } finally {
    adding.value = false
  }
}

watch(() => props.id, loadProduct)
onMounted(loadProduct)
</script>

<style scoped>
.detail-page { padding: 32px 24px 80px; }
.detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 56px;
}

.main-img {
  aspect-ratio: 1/1;
  background: var(--bg-elevated);
  border-radius: var(--radius-lg);
  overflow: hidden;
}
.main-img img { width: 100%; height: 100%; object-fit: cover; }

.thumb-row { display: flex; gap: 10px; margin-top: 12px; }
.thumb {
  width: 72px; height: 72px;
  border-radius: var(--radius-sm);
  overflow: hidden;
  border: 2px solid transparent;
  opacity: 0.6;
}
.thumb.active { border-color: var(--accent-orange); opacity: 1; }
.thumb img { width: 100%; height: 100%; object-fit: cover; }

.product-name { font-size: 38px; margin: 12px 0 16px; }

.price-row { display: flex; align-items: baseline; gap: 12px; margin-bottom: 20px; }
.price { font-size: 28px; font-weight: 800; color: var(--accent-orange); }
.price-old { font-size: 16px; color: var(--text-muted); text-decoration: line-through; }

.description { color: var(--text-secondary); margin-bottom: 28px; line-height: 1.7; }

.select-group { margin-bottom: 24px; }
.select-group h4 { font-size: 14px; font-weight: 600; margin-bottom: 10px; }
.selected-value { color: var(--accent-orange); font-weight: 700; }
.options-row { display: flex; gap: 10px; flex-wrap: wrap; }

.stock-info { font-size: 13px; color: var(--text-muted); margin-bottom: 16px; }

.qty-row { margin-bottom: 24px; }
.qty-control {
  display: inline-flex;
  align-items: center;
  border: 1.5px solid var(--border-strong);
  border-radius: var(--radius-sm);
}
.qty-control button {
  width: 40px; height: 40px;
  font-size: 18px;
  color: var(--text-primary);
}
.qty-control span { width: 48px; text-align: center; font-weight: 600; }

.add-btn { padding: 16px; font-size: 16px; }
.msg { margin-top: 14px; font-size: 14px; font-weight: 600; }
.msg.success { color: var(--success); }
.msg.error { color: var(--danger); }

.state-msg { text-align: center; padding: 80px 0; color: var(--text-muted); }

@media (max-width: 800px) {
  .detail-grid { grid-template-columns: 1fr; gap: 32px; }
}
</style>
