<script lang="ts">
  import { t } from 'svelte-i18n';
</script>

<div class="text-center py-32 space-y-6">
  <div class="text-8xl animate-bounce">🏗️</div>
  <h1 class="text-5xl font-black text-gray-300 dark:text-gray-700 uppercase tracking-tighter">{$t('in-development')}</h1>
  <div class="pt-8">
    <a href="/" class="text-primary-600 font-bold hover:underline">← {$t('back')}</a>
  </div>
</div>
