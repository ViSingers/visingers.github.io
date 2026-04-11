<script lang="ts">
  import { t } from 'svelte-i18n';
  import { Button, Card, Alert } from 'flowbite-svelte';
  import { FolderOpen, Info as InfoIcon } from 'lucide-svelte';
  import { get, set } from 'idb-keyval';
  import { onMount } from 'svelte';

  let directoryHandle: FileSystemDirectoryHandle | null = null;
  let folderContent: string[] = [];
  let error: string | null = null;

  onMount(async () => {
    directoryHandle = await get('singersDirectoryHandle');
    if (directoryHandle) {
      await refreshContent();
    }
  });

  async function selectDirectory() {
    try {
      // @ts-ignore
      directoryHandle = await window.showDirectoryPicker();
      await set('singersDirectoryHandle', directoryHandle);
      await refreshContent();
    } catch (e: any) {
      error = e.message;
    }
  }

  async function refreshContent() {
    if (!directoryHandle) return;
    
    try {
      // @ts-ignore
      if (await directoryHandle.queryPermission({ mode: 'readwrite' }) !== 'granted') {
        // @ts-ignore
        await directoryHandle.requestPermission({ mode: 'readwrite' });
      }

      const entries = [];
      // @ts-ignore
      for await (const entry of directoryHandle.values()) {
        entries.push(entry.name);
      }
      folderContent = entries;
    } catch (e: any) {
      error = e.message;
    }
  }
</script>

<div class="max-w-4xl mx-auto space-y-8 pb-20">
  <h1 class="text-4xl font-black tracking-tight text-gray-900 dark:text-white">{$t('nav.voicebanks')}</h1>

  <Alert color="blue" class="rounded-3xl p-6 border-none shadow-sm">
    <div class="flex items-center gap-3">
      <InfoIcon class="w-6 h-6 text-blue-600" />
      <span class="text-lg font-medium">Manage your local voicebanks directly.</span>
    </div>
    <p class="mt-2 text-blue-700/80 dark:text-blue-300/80">
      This experimental feature uses the File System Access API to allow ViSingers to interact with your OpenUTAU singers folder.
    </p>
  </Alert>

  <Card class="border-none bg-gray-50 dark:bg-gray-800/50 rounded-[2.5rem] p-10 text-center space-y-8 shadow-sm">
    <div class="p-8 bg-white dark:bg-gray-800 rounded-3xl shadow-xl shadow-primary-500/5 inline-block ring-1 ring-black/5">
      <FolderOpen class="w-20 h-20 text-primary-500" />
    </div>
    
    <div class="space-y-3">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white">OpenUTAU Singers Directory</h2>
      <p class="text-gray-500 text-lg max-w-md mx-auto">Select your OpenUTAU installation folder to sync and manage your voicebanks.</p>
    </div>

    <Button on:click={selectDirectory} pill size="xl" class="px-12 py-4 text-lg font-bold shadow-lg shadow-primary-500/30">
      Select Directory
    </Button>

    {#if directoryHandle}
      <div class="pt-10 border-t border-gray-200 dark:border-gray-700 text-left space-y-6">
        <div class="flex justify-between items-center">
          <h3 class="text-xl font-bold flex items-center gap-2">
            <span class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></span>
            Connected: <span class="text-primary-600">{directoryHandle.name}</span>
          </h3>
          <Button size="xs" color="alternative" on:click={refreshContent}>Refresh</Button>
        </div>
        
        <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3">
          {#each folderContent as name}
            <div class="text-sm bg-white dark:bg-gray-700 p-3 rounded-2xl truncate shadow-sm border border-black/5 dark:border-white/5 hover:border-primary-300 transition-colors cursor-default">
              {name}
            </div>
          {/each}
        </div>
      </div>
    {/if}

    {#if error}
      <Alert color="red" class="mt-4 rounded-2xl">
        {error}
      </Alert>
    {/if}
  </Card>
</div>
