<script setup lang="ts">
import JSZip from 'jszip'

const { t } = useI18n()
const settingsStore = useSettingsStore()
const folderContent = ref('')

onMounted(getFolderContent)

async function selectDirectory() {
  try {
    const handle = await window.showDirectoryPicker()
    let openUtauExists = false
    let singersHandle: FileSystemDirectoryHandle | null = null

    for await (const [name, entry] of handle.entries()) {
      if (entry.kind === 'file' && name.toLowerCase() === 'openutau.exe') {
        openUtauExists = true
      }
      if (entry.kind === 'directory' && name.toLowerCase() === 'singers') {
        singersHandle = entry as FileSystemDirectoryHandle
      }
    }

    if (openUtauExists && singersHandle) {
      await settingsStore.setSingersDirectoryHandle(singersHandle)
      await getFolderContent()
    }
  }
  catch (error) {
    console.error('Error selecting directory:', error)
  }
}

async function downloadArchive(url: string) {
  try {
    const response = await fetch(url)
    if (!response.ok) {
      throw new Error(`Failed to fetch ${url}: ${response.statusText}`)
    }
    const blob = await response.blob()
    return blob
  }
  catch (error) {
    console.error('Error downloading archive:', error)
    throw error
  }
}

async function unzipArchive(blob: Blob) {
  const zip = new JSZip()
  const zipContent = await zip.loadAsync(blob)
  return zipContent
}
async function saveFilesToDirectory(directoryHandle: FileSystemDirectoryHandle, zipContent: JSZip) {
  for (const [filename, fileData] of Object.entries(zipContent.files)) {
    if (!fileData.dir) {
      const fileHandle = await directoryHandle.getFileHandle(filename, { create: true })
      const writable = await fileHandle.createWritable()
      const fileBlob = await fileData.async('blob')
      await writable.write(fileBlob)
      await writable.close()
    }
  }
}

async function handleDownloadAndExtract() {
  try {
    const directoryHandle = await window.showDirectoryPicker()
    const archiveBlob = await downloadArchive('https://example.com/path/to/archive.zip')
    const zipContent = await unzipArchive(archiveBlob)
    await saveFilesToDirectory(directoryHandle, zipContent)
    // console.log('Files extracted successfully.')
  }
  catch (error) {
    console.error('Error processing archive:', error)
  }
}

async function getFolderContent() {
  const handle = await settingsStore.getSingersDirectoryHandle()
  if (handle === null) {
    folderContent.value = 'NO DIR'
    return
  }

  if (await handle.queryPermission({ mode: 'readwrite' }) !== 'granted') {
    await handle.requestPermission({ mode: 'readwrite' })
  }

  folderContent.value = (await Array.fromAsync(handle.entries())).map(([_, value]) => `${value.name}`).join(', ')
}
</script>

<template>
  <b-container class="mt-3">
    <b-row class="mb-8">
      <b-button variant="outline-secondary" size="lg" @click="selectDirectory">
        Select OpenUtau directory
      </b-button>
      <p>
        {{ folderContent }}
      </p>
    </b-row>
    <b-row class="mb-8" />
  </b-container>
</template>
