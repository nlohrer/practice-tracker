<script setup>
const props = defineProps({
    session: Object
})
</script>

<template>
    <tr>
        <td>{{ session.task }}</td>
        <td>{{ session.duration.hours }}</td>
        <td>{{ session.duration.minutes }}</td>
        <td>{{ session.date }}</td>
        <td>{{ session.time }}</td>
    </tr>
</template>

<style scoped>
tr {
    margin: 2px 10px;
    text-align: left;
}
</style>