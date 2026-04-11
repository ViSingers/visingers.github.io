<script lang="ts">
  import '../app.pcss';
  import { onMount } from 'svelte';
  import { fade, fly } from 'svelte/transition';

  import Header from '$lib/components/Header.svelte';
  import { dataService } from '$lib/dataService';
  import { initI18n } from '$lib/i18n';

  let { children, data } = $props();

  let ready = $state(false);

  onMount(async () => {
    try {
      await initI18n();
      ready = true;
      dataService.init();
    } catch (e) {
      console.error('Initialization error:', e);
      ready = true;
    }
  });
</script>

<svelte:head>
  <title>ViSingers</title>
</svelte:head>

<div class="min-h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-white">
  {#if ready}
    <Header />

    {#key data.pathname}
      <main 
        class="container mx-auto px-4 py-8"
        in:fly={{ y: 25, duration: 450, delay: 180, opacity: 0 }}
        out:fade={{ duration: 280 }}
      >
        {@render children()}
      </main>
    {/key}

  {:else}
    <div class="flex items-center justify-center min-h-screen">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
    </div>
  {/if}
</div>