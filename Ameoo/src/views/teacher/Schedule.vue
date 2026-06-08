<script setup>
const days = ['Thứ 2','Thứ 3','Thứ 4','Thứ 5','Thứ 6','Thứ 7']
const times = ['07:30\n09:00','09:30\n11:00','14:00\n15:30','17:30\n19:00']
const grid = [
  [{name:'IELTS Int',room:'P.201',color:'#ede9fe'}, null, {name:'IELTS Int',room:'P.201',color:'#ede9fe'}, null, {name:'IELTS Int',room:'P.201',color:'#ede9fe'}, null],
  [{name:'IELTS Adv',room:'P.401',color:'#ccfbf1'}, null, null, {name:'IELTS Adv',room:'P.401',color:'#ccfbf1'}, null, {name:'IELTS Adv',room:'P.401',color:'#ccfbf1'}],
  [null, {name:'TOEIC 600+',room:'P.105',color:'#dcfce7'}, null, {name:'TOEIC 600+',room:'P.105',color:'#dcfce7'}, null, {name:'TOEIC 600+',room:'P.105',color:'#dcfce7'}],
  [{name:'GT Cơ bản',room:'P.302',color:'#fef9c3'}, null, {name:'GT Cơ bản',room:'P.302',color:'#fef9c3'}, null, {name:'GT Cơ bản',room:'P.302',color:'#fef9c3'}, null],
]
</script>
<template>
<div>
  <div class="page-header"><h2 class="page-title">Thời khoá biểu</h2><div class="btn-actions"><button class="btn-outline">←</button><button class="btn-outline">→</button></div></div>
  <div class="card">
    <div class="card-title" style="margin-bottom:16px">Thời khoá biểu — Tuần 22 (26/05 - 01/06)</div>
    <table class="data-table" style="table-layout:fixed">
      <thead><tr><th style="width:80px">Ca</th><th v-for="d in days" :key="d">{{ d }}</th></tr></thead>
      <tbody>
        <tr v-for="(row,ri) in grid" :key="ri">
          <td style="white-space:pre-line;font-size:13px;font-weight:600;color:#64748b">{{ times[ri] }}</td>
          <td v-for="(cell,ci) in row" :key="ci">
            <div v-if="cell" :style="{background:cell.color,padding:'8px',borderRadius:'8px',textAlign:'center',fontSize:'12px'}">
              <div class="text-bold" style="font-size:12px">{{ cell.name }}</div>
              <div class="text-muted" style="font-size:11px">{{ cell.room }}</div>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
    <div class="legend" style="margin-top:16px">
      <span style="display:inline-block;width:12px;height:12px;background:#ede9fe;border-radius:2px"></span> IELTS Intermediate
      <span style="display:inline-block;width:12px;height:12px;background:#dcfce7;border-radius:2px;margin-left:16px"></span> TOEIC 600+
      <span style="display:inline-block;width:12px;height:12px;background:#fef9c3;border-radius:2px;margin-left:16px"></span> Giao tiếp cơ bản
      <span style="display:inline-block;width:12px;height:12px;background:#ccfbf1;border-radius:2px;margin-left:16px"></span> IELTS Advanced
    </div>
  </div>
</div>
</template>
