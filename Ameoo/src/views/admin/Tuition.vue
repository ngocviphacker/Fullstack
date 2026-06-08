<script setup>
import { tuitionReceipts } from '../../data/mockData.js'
const fmt = n => n.toLocaleString('vi-VN')
const statusClass = s => s==='Đã thu'?'badge-green':s==='Đóng 1 phần'?'badge-orange':'badge-red'
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Thu học phí</h2><p class="page-desc">Quản lý phiếu thu và ghi nhận thanh toán</p></div>
    <div class="btn-actions"><button class="btn-outline">Xuất phiếu thu</button><button class="btn-primary">+ Tạo phiếu thu</button></div>
  </div>
  <div class="stat-cards cols-3">
    <div class="stat-card green"><div class="label">Đã thu tháng 5</div><div class="value" style="color:#10b981">128.5M đ</div><div class="sub">42 giao dịch</div></div>
    <div class="stat-card red"><div class="label">Chưa thanh toán</div><div class="value" style="color:#ef4444">18.2M đ</div><div class="sub">5 học viên</div></div>
    <div class="stat-card blue"><div class="label">Dự thu tháng 6</div><div class="value">145.0M đ</div><div class="sub">Dự kiến</div></div>
  </div>
  <div class="card">
    <div style="display:flex;justify-content:space-between;margin-bottom:16px"><div class="card-title">Phiếu thu gần đây</div><select style="border:1px solid #cbd5e1;border-radius:6px;padding:6px 10px;font-size:13px"><option>Tháng 5/2026</option></select></div>
    <table class="data-table">
      <thead><tr><th>MÃ PHIẾU</th><th>HỌC VIÊN</th><th>KHÓA HỌC</th><th>SỐ TIỀN</th><th>NGÀY THU</th><th>PHƯƠNG THỨC</th><th>TRẠNG THÁI</th></tr></thead>
      <tbody>
        <tr v-for="t in tuitionReceipts" :key="t.id">
          <td class="text-muted">{{ t.id }}</td><td class="text-bold">{{ t.studentName }}</td><td>{{ t.course }}</td>
          <td><span class="money" :class="t.status==='Đã thu'?'green':'red'">{{ fmt(t.amount) }} đ</span></td>
          <td class="text-muted">{{ t.date || '—' }}</td><td>{{ t.method }}</td>
          <td><span class="badge" :class="statusClass(t.status)">{{ t.status }}</span></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
