<script setup lang="ts">
import { defineProps } from "vue";

const props = defineProps<{
    matches: any[];
}>();

// Simple visualization logic: Group matches by round
const rounds = computed(() => {
    if (!props.matches) return {};
    const grouped: any = {};
    props.matches.forEach(m => {
        if (!grouped[m.round]) grouped[m.round] = [];
        grouped[m.round].push(m);
    });
    return grouped;
});

import { computed } from "vue";
</script>

<template>
  <div class="bracket-container">
    <div v-for="(roundMatches, round) in rounds" :key="round" class="bracket-round">
        <h4 class="round-title">Vòng {{ round }}</h4>
        <div class="matches-column">
            <div v-for="match in roundMatches" :key="match.id" class="match-box">
                <div class="team team-1" :class="{'winner': match.match?.winningSide === 1}">
                    <span>{{ match.match?.team1Player1?.fullName || 'TBD' }}</span>
                    <span class="score">{{ match.match?.team1Score ?? '-' }}</span>
                </div>
                <div class="team team-2" :class="{'winner': match.match?.winningSide === 2}">
                     <span>{{ match.match?.team2Player1?.fullName || 'TBD' }}</span>
                     <span class="score">{{ match.match?.team2Score ?? '-' }}</span>
                </div>
            </div>
        </div>
    </div>
  </div>
</template>

<style scoped>
.bracket-container {
    display: flex;
    overflow-x: auto;
    padding: 20px;
    gap: 40px;
}

.bracket-round {
    min-width: 200px;
    display: flex;
    flex-direction: column;
}

.round-title {
    text-align: center;
    margin-bottom: 20px;
    color: #7f8c8d;
}

.matches-column {
    display: flex;
    flex-direction: column;
    justify-content: center;
    gap: 20px;
    flex: 1;
}

.match-box {
    background: white;
    border: 1px solid #ddd;
    border-radius: 4px;
    padding: 10px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
}

.team {
    display: flex;
    justify-content: space-between;
    padding: 5px;
}

.team.winner {
    background-color: #d5f5e3;
    font-weight: bold;
}

.score {
    font-weight: bold;
}
</style>
