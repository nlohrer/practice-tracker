<script setup>
import { ref } from 'vue';

const task = ref("Practice");
const hours = ref("1");
const minutes = ref("15");

const datetime = new Date();
const date = ref(datetime.toISOString().slice(0,10));
const time = ref(datetime.toISOString().slice(11, 16));

const props = defineProps({
    username: String
})

async function submitSession() {
    const request = `{
        "task": "${task.value}",
        "duration":
            {"hours": ${hours.value}, "minutes": ${minutes.value}},
        "date": "${date.value}",
        "time": ${time.value ? `"${time.value}:00"` : null}
    }`;
    const response = await fetch(`${import.meta.env.VITE_API_URL}?username=${props.username}`, {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        body: request
    });
}
</script>

<template>
    <form action="javascript:void(0);" method="post" @submit="submitSession">
        <label for="task">Task</label><br>
        <input type="text" v-model="task" id="task" /><br>
        <label for="hours">Duration (hours)</label><br>
        <input type="text" v-model="hours" id="hours" /><br>
        <label for="minutes">Duration(minutes)</label><br>
        <input type="text" v-model="minutes" id="minutes" /><br>
        <label for="date">Date</label><br>
        <input type="date" v-model="date" id="date" /><br>
        <label for="time">Time</label><br>
        <input type="time" v-model="time" id="time" /><br>
        <input type="submit" value="Submit">
    </form>
</template>

<style scoped>
input {
    margin-bottom: 1em;
}

form {
    margin-bottom:2em;
}
</style>