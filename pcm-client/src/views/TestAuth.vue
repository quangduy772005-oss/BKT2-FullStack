<script setup lang="ts">
import { getMembers, createMember } from "@/services/members";

const testGet = async () => {
  try {
    const res = await getMembers();
    console.log("GET OK:", res.data);
    alert("GET MEMBERS OK – check console");
  } catch (err: any) {
    alert("GET ERROR: " + err.response?.status);
  }
};

const testPost = async () => {
  try {
    await createMember({
      fullName: "Test Role",
      email: "role@test.com",
      joinDate: "2026-01-24",
      clubId: 1,
    });
    alert("POST OK");
  } catch (err: any) {
    alert("POST ERROR: " + err.response?.status);
  }
};
</script>

<template>
  <div>
    <button @click="testGet">GET Members</button>
    <button @click="testPost">POST Member</button>
  </div>
</template>
