import { acceptHMRUpdate, defineStore } from 'pinia'
import { get, set } from 'idb-keyval'

export const useSettingsStore = defineStore('settings', () => {
  const language = ref('en')
  const theme = ref<'light' | 'dark'>('light')
  const singersDirectoryHandle = ref<FileSystemDirectoryHandle | null>(null)

  function setLanguage(newLanguage: string) {
    language.value = newLanguage
  }

  function toggleTheme() {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }

  async function setSingersDirectoryHandle(handle: FileSystemDirectoryHandle) {
    singersDirectoryHandle.value = handle
    await set('singersDirectoryHandle', handle)
  }

  async function getSingersDirectoryHandle() {
    if (!(singersDirectoryHandle instanceof FileSystemDirectoryHandle)) {
      singersDirectoryHandle.value = await get<FileSystemDirectoryHandle>('singersDirectoryHandle') || null
    }

    return singersDirectoryHandle.value
  }

  return {
    language,
    theme,
    singersDirectoryHandle,
    setLanguage,
    toggleTheme,
    setSingersDirectoryHandle,
    getSingersDirectoryHandle,
  }
}, {
  persist: true,
})

if (import.meta.hot)
  import.meta.hot.accept(acceptHMRUpdate(useSettingsStore as any, import.meta.hot))
