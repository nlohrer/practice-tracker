<script setup>
import { computed, ref } from "vue";
import SessionRow from "./SessionRow.vue";
import SessionForm from "./SessionForm.vue";

const props = defineProps({
    username: String
})

const sessionData = ref(null);

function addSession(newSession) {
    sessionData.value.push(newSession);
}

function deleteSession(id) {
    sessionData.value = sessionData.value.filter(session => session.id != id)
}

async function getSessionData() {
    sessionData.value = null;
    const fetchedData = await fetch(`${import.meta.env.VITE_API_URL}?username=${props.username}`, { cache: "reload" });
    sessionData.value = await fetchedData.json()
}
getSessionData();
</script>

<template>
    <div class="list-container">
        <SessionForm :username="username" @add-new="addSession" />
        <table>
            <tr>
                <th>task</th>
                <th>Duration (min)</th>
                <th>Duration (h)</th>
                <th>Date</th>
                <th>Time</th>
            </tr>
            <SessionRow v-for="session in sessionData" :session="session" @delete-request="deleteSession" />
        </table>
    </div>
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

<style scoped>
.list-container {
    display: flex;
    gap: 1em;
}
</style>