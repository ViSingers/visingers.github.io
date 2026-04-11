<script lang="ts">
  import { t } from 'svelte-i18n';
  import { Button } from 'flowbite-svelte';
</script>

<div class="text-center py-32 space-y-8">
  <div class="relative inline-block">
    <h1 class="text-[12rem] font-black text-gray-100 dark:text-gray-800 leading-none">404</h1>
    <div class="absolute inset-0 flex items-center justify-center">
      <h2 class="text-4xl font-bold text-gray-900 dark:text-white">{$t('not-found')}</h2>
    </div>
  </div>
  <div class="pt-8">
    <a href="/" class="text-primary-600 font-bold hover:underline">← {$t('back')}</a>
  </div>
</div>
