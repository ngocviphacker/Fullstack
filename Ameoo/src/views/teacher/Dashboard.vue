<script setup>
import { ref } from 'vue'
import { leaveRequests } from '../../data/mockData.js'
const leaves = ref([...leaveRequests])
</script>
<template>
<div>
  <div class="page-header">
    <h2 class="page-title">Dashboard</h2>
  </div>
  <div class="stat-cards cols-4">
    <div class="stat-card blue"><div class="label">LỚP ĐANG DẠY</div><div class="value">4</div><div class="sub">📚 2 lớp hôm nay</div></div>
    <div class="stat-card green"><div class="label">TỔNG HỌC VIÊN</div><div class="value">87</div><div class="sub">↗ +5 tháng này</div></div>
    <div class="stat-card orange"><div class="label">TỶ LỆ ĐI HỌC TB</div><div class="value">88%</div><div class="sub">↓ -2% tháng trước</div></div>
    <div class="stat-card red"><div class="label">ĐƠN XIN NGHỈ CHỜ DUYỆT</div><div class="value">2</div><div class="sub">⏰ Cần xử lý</div></div>
  </div>
  <div class="grid-2">
    <div class="card">
      <div style="display:flex;justify-content:space-between;margin-bottom:16px"><div class="card-title">Lịch dạy hôm nay — Thứ 6, 30/05</div><a class="text-blue" style="font-size:13px;cursor:pointer">Xem TKB →</a></div>
      <div style="display:flex;align-items:center;padding:12px 0;border-bottom:1px solid #f1f5f9">
        <span style="width:12px;height:12px;border-radius:50%;background:#3b82f6;margin-right:12px"></span>
        <div style="flex:1"><div class="text-bold">IELTS Intermediate — Phòng 201</div><div class="text-muted">07:30 – 09:00 · 22 học viên · <span class="text-green">Đã điểm danh</span></div></div>
        <span class="badge badge-green">Xong</span>
      </div>
      <div style="display:flex;align-items:center;padding:12px 0;border-bottom:1px solid #f1f5f9">
        <span style="width:12px;height:12px;border-radius:50%;background:#f59e0b;margin-right:12px"></span>
        <div style="flex:1"><div class="text-bold">TOEIC 600+ — Phòng 105</div><div class="text-muted">14:00 – 15:30 · 18 học viên · <span class="text-red">Chưa điểm danh</span></div></div>
        <button class="btn-primary" style="padding:6px 14px;font-size:12px">✓ Điểm danh</button>
      </div>
      <div style="display:flex;align-items:center;padding:12px 0">
        <span style="width:12px;height:12px;border-radius:50%;background:#94a3b8;margin-right:12px"></span>
        <div style="flex:1"><div class="text-bold">Giao tiếp cơ bản — Phòng 302</div><div class="text-muted">17:30 – 19:00 · 25 học viên · Sắp tới</div></div>
        <span class="badge badge-gray">Sắp tới</span>
      </div>
    </div>
    <div class="card">
      <div class="card-title" style="margin-bottom:16px">Thống kê lớp</div>
      <div v-for="(item,i) in [{name:'IELTS Intermediate',rate:91,color:'purple'},{name:'TOEIC 600+',rate:88,color:'orange'},{name:'Giao tiếp cơ bản',rate:82,color:'orange'},{name:'IELTS Advanced',rate:95,color:'green'}]" :key="i" style="margin-bottom:12px">
        <div style="display:flex;justify-content:space-between;font-size:13px;margin-bottom:4px"><span>{{ item.name }}</span><span class="text-bold" :class="'text-'+item.color">{{ item.rate }}%</span></div>
        <div class="progress-bar"><div class="progress-fill" :class="item.color" :style="{width:item.rate+'%'}"></div></div>
      </div>
      <div class="text-muted" style="margin-top:8px">Tỷ lệ điểm danh trung bình tháng 5</div>
    </div>
  </div>
  <div class="card">
    <div style="display:flex;justify-content:space-between;margin-bottom:12px"><div class="card-title">Đơn xin nghỉ chờ duyệt</div><a class="text-blue" style="font-size:13px;cursor:pointer">Xem tất cả →</a></div>
    <table class="data-table">
      <thead><tr><th>HỌC VIÊN</th><th>LỚP</th><th>NGÀY NGHỈ</th><th>LÝ DO</th><th>GỬI LÚC</th><th>HÀNH ĐỘNG</th></tr></thead>
      <tbody>
        <tr v-for="l in leaves.filter(x=>x.status==='Chờ duyệt')" :key="l.id">
          <td><div class="user-cell"><div class="avatar orange">{{ l.studentName.split(' ').map(w=>w[0]).slice(-2).join('') }}</div><span class="text-bold">{{ l.studentName }}</span></div></td>
          <td>{{ l.course }}</td><td>{{ l.leaveDate }}</td><td>{{ l.reason }}</td><td class="text-muted">{{ l.sentAt }}</td>
          <td><button class="btn-approve" @click="l.status='Đã duyệt'">✓ Duyệt</button><button class="btn-reject" @click="l.status='Từ chối'">✗ Từ chối</button></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
