<script setup>
import { ref } from 'vue'
import { leaveRequests } from '../../data/mockData.js'
const leaves = ref([...leaveRequests])
const statusClass = s => s==='Đã duyệt'?'badge-green':s==='Từ chối'?'badge-red':'badge-yellow'
</script>
<template>
<div>
  <div class="page-header">
    <h2 class="page-title">Đơn xin nghỉ</h2>
    <span class="badge badge-yellow" style="font-size:13px;padding:6px 14px">{{ leaves.filter(l=>l.status==='Chờ duyệt').length }} chờ duyệt</span>
  </div>
  <div class="card">
    <table class="data-table">
      <thead><tr><th>HỌC VIÊN</th><th>LỚP</th><th>NGÀY NGHỈ</th><th>LÝ DO</th><th>TRẠNG THÁI</th><th>HÀNH ĐỘNG</th></tr></thead>
      <tbody>
        <tr v-for="l in leaves" :key="l.id">
          <td><div class="user-cell"><div class="avatar" :class="['green','orange','teal','red'][l.id%4]">{{ l.studentName.split(' ').map(w=>w[0]).slice(-2).join('') }}</div><span class="text-bold">{{ l.studentName }}</span></div></td>
          <td>{{ l.course }}</td><td>{{ l.leaveDate }}</td><td>{{ l.reason }}</td>
          <td><span class="badge" :class="statusClass(l.status)">{{ l.status }}</span></td>
          <td>
            <div v-if="l.status==='Chờ duyệt'" style="display:flex;gap:6px"><button class="btn-approve" @click="l.status='Đã duyệt'">✓ Duyệt</button><button class="btn-reject" @click="l.status='Từ chối'">✗ Từ chối</button></div>
            <span v-else class="text-muted">Đã xử lý</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
