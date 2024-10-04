import { acceptHMRUpdate, defineStore } from 'pinia'

export const useSettingsStore = defineStore('settings', () => {
  const language = ref('en')
  const theme = ref<'light' | 'dark'>('light')

  function setLanguage(newLanguage: string) {
    language.value = newLanguage
  }

  function toggleTheme() {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }

  return {
    language,
    theme,
    setLanguage,
    toggleTheme,
  }
}, {
  persist: true,
})

if (import.meta.hot)
  import.meta.hot.accept(acceptHMRUpdate(useSettingsStore as any, import.meta.hot))
