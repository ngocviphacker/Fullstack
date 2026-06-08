<script setup>
import { rules, studyRules, graduationReqs } from '../../data/mockData.js'
</script>
<template>
<div>
  <h2 class="page-title">Nội quy trung tâm</h2>
  <div class="grid-2" style="margin-top:20px">
    <div>
      <div class="card" style="margin-bottom:20px">
        <div class="card-title" style="margin-bottom:16px">📋 Nội quy chung</div>
        <div v-for="r in rules" :key="r.num" style="display:flex;gap:14px;padding:12px 0;border-bottom:1px solid #f1f5f9">
          <div style="width:28px;height:28px;background:#0d9488;border-radius:50%;display:flex;align-items:center;justify-content:center;color:#fff;font-weight:700;font-size:13px;flex-shrink:0">{{ r.num }}</div>
          <div><div class="text-bold">{{ r.title }}</div><div class="text-muted" style="margin-top:2px">{{ r.text }}</div></div>
        </div>
      </div>
      <div class="card">
        <div class="card-title" style="margin-bottom:16px">📚 Quy định học tập</div>
        <div v-for="r in studyRules" :key="r.num" style="display:flex;gap:14px;padding:12px 0;border-bottom:1px solid #f1f5f9">
          <div style="width:28px;height:28px;background:#7c3aed;border-radius:50%;display:flex;align-items:center;justify-content:center;color:#fff;font-weight:700;font-size:13px;flex-shrink:0">{{ r.num }}</div>
          <div class="text-muted">{{ r.text }}</div>
        </div>
      </div>
    </div>
    <div class="card" style="height:fit-content">
      <div class="card-title" style="margin-bottom:16px">🎓 Điều kiện tốt nghiệp</div>
      <div v-for="(req,i) in graduationReqs" :key="i" :style="{display:'flex',alignItems:'center',gap:'12px',padding:'12px 0',borderBottom:'1px solid #f1f5f9'}">
        <div :style="{width:'24px',height:'24px',borderRadius:'50%',background:req.met?'#dcfce7':'#fee2e2',display:'flex',alignItems:'center',justifyContent:'center',color:req.met?'#166534':'#991b1b',fontWeight:'700',fontSize:'14px',flexShrink:0}">{{ req.met?'✓':'✗' }}</div>
        <span :class="req.met?'text-bold':''">{{ req.text }}</span>
      </div>
      <div style="margin-top:16px;padding:14px;background:#f0fdf4;border-radius:10px;border:1px solid #bbf7d0">
        <div style="font-weight:600;color:#166534;margin-bottom:4px">✓ Đủ điều kiện tốt nghiệp</div>
        <div class="text-muted">Hoàn thành tất cả yêu cầu</div>
      </div>
    </div>
  </div>
</div>
</template>
