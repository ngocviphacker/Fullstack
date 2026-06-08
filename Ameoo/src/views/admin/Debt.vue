<script setup>
import { debtList } from '../../data/mockData.js'
const fmt = n => n.toLocaleString('vi-VN')
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Theo dõi công nợ</h2><p class="page-desc">Danh sách học viên còn nợ học phí</p></div>
    <div class="btn-actions"><button class="btn-outline">Gửi nhắc nhở</button><button class="btn-primary">+ Thu ngay</button></div>
  </div>
  <div class="card">
    <table class="data-table">
      <thead><tr><th>HỌC VIÊN</th><th>KHÓA HỌC</th><th>TỔNG HỌC PHÍ</th><th>ĐÃ ĐÓNG</th><th>CÒN NỢ</th><th>HẠN ĐÓNG</th><th>HÀNH ĐỘNG</th></tr></thead>
      <tbody>
        <tr v-for="d in debtList" :key="d.code">
          <td><div class="user-cell"><div class="avatar orange">{{ d.name.split(' ').map(w=>w[0]).slice(-2).join('') }}</div><div><div class="name">{{ d.name }}</div><div class="code">{{ d.code }}</div></div></div></td>
          <td>{{ d.course }}</td>
          <td>{{ fmt(d.totalFee) }} đ</td>
          <td><span class="text-green">{{ fmt(d.paid) }} đ</span></td>
          <td><span class="money red">{{ fmt(d.remaining) }} đ</span></td>
          <td><span class="badge" :class="d.urgency==='danger'?'badge-red':'badge-yellow'">{{ d.deadline }}</span></td>
          <td><button :class="d.urgency==='danger'?'btn-primary':'btn-outline'" style="padding:6px 14px;font-size:12px">{{ d.urgency==='danger'?'Thu ngay':'Nhắc nhở' }}</button></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
