<template>
  <router-link :to="`/products/${product.productId}`" class="product-card card">
    <div class="img-wrap">
      <img :src="product.imageUrl || placeholder" :alt="product.productName" />
      <span v-if="product.discountPrice" class="discount-badge">
        -{{ discountPercent }}%
      </span>
      <span v-if="!product.inStock" class="oos-badge">Hết hàng</span>
    </div>
    <div class="info">
      <p class="brand">{{ product.brandName }}</p>
      <h3 class="name">{{ product.productName }}</h3>
      <div class="price-row">
        <span class="price">{{ formatPrice(product.discountPrice || product.price) }}</span>
        <span v-if="product.discountPrice" class="price-old">{{ formatPrice(product.price) }}</span>
      </div>
    </div>
  </router-link>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  product: { type: Object, required: true }
})

const placeholder = 'https://bizweb.dktcdn.net/thumb/1024x1024/100/449/472/products/49ece0fd-03df-49ea-b066-6f9b16f282d5.jpg?v=1652010429983'

const discountPercent = computed(() => {
  if (!props.product.discountPrice) return 0
  return Math.round((1 - props.product.discountPrice / props.product.price) * 100)
})

function formatPrice(value) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'đ'
}
</script>

<style scoped>
.product-card {
  display: block;
  overflow: hidden;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}
.product-card:hover {
  transform: translateY(-6px);
  box-shadow: var(--shadow-hover);
}

.img-wrap {
  position: relative;
  aspect-ratio: 1 / 1;
  background: var(--bg-elevated-2);
  overflow: hidden;
}
.img-wrap img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}
.product-card:hover .img-wrap img {
  transform: scale(1.05);
}

.discount-badge {
  position: absolute;
  top: 12px;
  left: 12px;
  background: var(--accent-orange);
  color: #1A1006;
  font-size: 12px;
  font-weight: 800;
  padding: 4px 8px;
  border-radius: var(--radius-sm);
}

.oos-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  background: rgba(0,0,0,0.7);
  color: var(--text-secondary);
  font-size: 12px;
  font-weight: 700;
  padding: 4px 8px;
  border-radius: var(--radius-sm);
}

.info {
  padding: 16px;
}
.brand {
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  color: var(--text-muted);
  margin-bottom: 4px;
}
.name {
  font-size: 15px;
  font-weight: 600;
  margin-bottom: 10px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  min-height: 40px;
}
.price-row {
  display: flex;
  align-items: baseline;
  gap: 8px;
}
.price {
  font-size: 17px;
  font-weight: 800;
  color: var(--accent-orange);
}
.price-old {
  font-size: 13px;
  color: var(--text-muted);
  text-decoration: line-through;
}
</style>
