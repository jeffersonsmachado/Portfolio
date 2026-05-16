import { defineStore } from "pinia";
import { ref } from "vue";

export const useUiStore = defineStore("ui", () => {
    const loading = ref(false);

    function startLoading() { loading.value = true; }
    function stopLoading() { loading.value = false; }

    return {
        loading,
        startLoading,
        stopLoading,
    };
});