<template>
  <footer class="footer">
    <div class="container footer-inner">
      <div class="footer-col">
        <div class="logo">MAX<span>VERSE</span></div>
        <p class="footer-desc">Giày thể thao chính hãng cho thế hệ chuyển động không ngừng.</p>
      </div>
      <div class="footer-col">
        <h4>Mua sắm</h4>
        <router-link to="/products">Tất cả sản phẩm</router-link>
        <router-link to="/products?categoryId=1">Giày chạy bộ</router-link>
        <router-link to="/products?categoryId=3">Sneaker thời trang</router-link>
      </div>
      <div class="footer-col">
        <h4>Hỗ trợ</h4>
        <a href="#">Chính sách đổi trả</a>
        <a href="#">Hướng dẫn chọn size</a>
        <a href="#">Liên hệ</a>
      </div>
    </div>
    <div class="footer-bottom">
      <p>© 2026 MaxVerse. Đồ án tốt nghiệp — Website bán giày thể thao.</p>
    </div>
  </footer>
</template>

<script setup></script>

<style scoped>
.footer {
  background: var(--bg-elevated);
  border-top: 1px solid var(--border-subtle);
  margin-top: 64px;
}
.footer-inner {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr;
  gap: 40px;
  padding: 48px 24px;
}
.footer-col .logo {
  font-family: var(--font-display);
  font-size: 22px;
}
.footer-col .logo span { color: var(--accent-orange); }
.footer-desc {
  margin-top: 12px;
  color: var(--text-secondary);
  font-size: 14px;
  max-width: 280px;
}
.footer-col h4 {
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--text-muted);
  margin-bottom: 16px;
}
.footer-col a {
  display: block;
  color: var(--text-secondary);
  font-size: 14px;
  margin-bottom: 10px;
  transition: color 0.15s ease;
}
.footer-col a:hover { color: var(--accent-orange); }

.footer-bottom {
  border-top: 1px solid var(--border-subtle);
  padding: 20px 24px;
  text-align: center;
  font-size: 13px;
  color: var(--text-muted);
}

@media (max-width: 700px) {
  .footer-inner { grid-template-columns: 1fr; }
}
</style>
