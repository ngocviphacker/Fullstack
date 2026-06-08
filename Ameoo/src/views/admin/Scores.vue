<script setup>
import { ref } from 'vue'
import { scoresAdmin } from '../../data/mockData.js'
const tab = ref('midterm')
const gradeClass = g => g==='Xuất sắc'?'badge-teal':g==='Giỏi'?'badge-green':g==='Khá'?'badge-blue':'badge-yellow'
</script>
<template>
<div>
  <div class="page-header">
    <div><h2 class="page-title">Kết quả học tập</h2><p class="page-desc">Nhập và tra cứu điểm kiểm tra</p></div>
    <div class="btn-actions">
      <select style="border:1px solid #cbd5e1;border-radius:8px;padding:8px 14px;font-size:14px"><option>Tiếng Anh B1 — Lớp A</option></select>
      <button class="btn-primary">Lưu điểm</button>
    </div>
  </div>
  <div class="card">
    <div class="tabs">
      <div class="tab" :class="{active:tab==='midterm'}" @click="tab='midterm'">Điểm giữa kỳ</div>
      <div class="tab" :class="{active:tab==='final'}" @click="tab='final'">Điểm cuối kỳ</div>
      <div class="tab" :class="{active:tab==='summary'}" @click="tab='summary'">Tổng kết</div>
    </div>
    <table class="data-table">
      <thead><tr><th>HỌC VIÊN</th><th>BÀI KIỂM TRA VIẾT</th><th>BÀI NGHE</th><th>BÀI NÓI</th><th>TRUNG BÌNH</th><th>XẾP LOẠI</th></tr></thead>
      <tbody>
        <tr v-for="s in scoresAdmin" :key="s.id">
          <td><div class="user-cell"><div class="avatar orange">{{ s.name.split(' ').map(w=>w[0]).slice(-2).join('') }}</div><div><div class="name">{{ s.name }}</div><div class="code">{{ s.code }}</div></div></div></td>
          <td><input class="input-score" :value="s.writing" /></td>
          <td><input class="input-score" :value="s.listening" /></td>
          <td><input class="input-score" :value="s.speaking" /></td>
          <td><span class="text-bold" :class="s.average>=8?'text-green':s.average>=6?'text-blue':'text-orange'" style="font-size:18px">{{ s.average }}</span></td>
          <td><span class="badge" :class="gradeClass(s.grade)">● {{ s.grade }}</span></td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
</template>
