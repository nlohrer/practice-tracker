<script setup>
import SessionRow from "./SessionRow.vue";

const props = defineProps({
    username: String
})

async function getAllSessionData() {
    const sessionData = await fetch(`${import.meta.env.VITE_API_URL}?username=${props.username}`, { cache: "reload" });
    const data = await sessionData.json()
    return data;
}
const allSessionData = await getAllSessionData();
</script>

<template>
    <table>
        <tr>
            <th>task</th>
            <th>Duration (min)</th>
            <th>Duration (h)</th>
            <th>Date</th>
            <th>Time</th>
        </tr>
        <SessionRow v-for="session in allSessionData" :session="session" />
    </table>
</template>

<style>
table {
    border-collapse: collapse;
    width: 100%;
}

td, th {
    border: 1px solid #c9c9c9;
    text-align: left;
    padding: 8px;
}

tr:nth-child(even) {
    background-color: #e4e4e4;
}
</style>