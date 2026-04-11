<script lang="ts">
  import { t } from 'svelte-i18n';
  import { Card, Listgroup, ListgroupItem, Avatar } from 'flowbite-svelte';
  import { ShieldCheck, Info, Users } from 'lucide-svelte';
  import tilt from '@savy011/tilt-svelte';
</script>

<div class="max-w-4xl mx-auto space-y-16 pb-20">
  <section class="space-y-8">
    <h1 class="text-4xl font-black tracking-tight text-gray-900 dark:text-white flex items-center gap-4">
      <Users class="w-10 h-10 text-primary-600" /> {$t('info.creators.title')}
    </h1>
    
    <div class="grid grid-cols-1 md:grid-cols-2 gap-32">
      <Card class="border-none bg-gray-50 dark:bg-gray-800/50 rounded-3xl p-6" {@attach tilt({ max: 8 })}>
        <div class="flex items-center gap-6">
          <Avatar src="/images/avatar1.jpg" size="xl" />
          <div class="space-y-1">
            <h3 class="text-2xl font-bold">msan</h3>
            <p class="text-gray-500 text-sm">{$t('info.creators.first-description')}</p>
            <a href="https://github.com/Megageorgio" target="_blank" class="text-primary-600 hover:underline text-sm font-medium">GitHub Profile</a>
          </div>
        </div>
      </Card>

      <Card class="border-none bg-gray-50 dark:bg-gray-800/50 rounded-3xl p-6" {@attach tilt({ max: 8 })}>
        <div class="flex items-center gap-6">
          <Avatar src="/images/avatar2.jpg" size="xl" />
          <div class="space-y-1">
            <h3 class="text-2xl font-bold">HHS_kt</h3>
            <p class="text-gray-500 text-sm">{$t('info.creators.second-description')}</p>
            <a href="https://www.youtube.com/@HHS_kt" target="_blank" class="text-primary-600 hover:underline text-sm font-medium">YouTube Channel</a>
          </div>
        </div>
      </Card>
    </div>
  </section>

  <section class="space-y-8">
    <h2 class="text-3xl font-bold flex items-center gap-4">
      <ShieldCheck class="w-8 h-8 text-green-500" /> {$t('info.rules.title')}
    </h2>
    <Listgroup class="border-none bg-transparent space-y-3">
      <ListgroupItem class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl border-none p-4">
        {$t('info.rules.no-prohibited-content')}
      </ListgroupItem>
      <ListgroupItem class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl border-none p-4">
        {$t('info.rules.no-disinformation')}
      </ListgroupItem>
      <ListgroupItem class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl border-none p-4">
        {$t('info.rules.no-repository-spam')}
      </ListgroupItem>
      <ListgroupItem class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl border-none p-4">
        {$t('info.rules.no-tags-spam')}
      </ListgroupItem>
    </Listgroup>
  </section>

  <section class="space-y-8">
    <h2 class="text-3xl font-bold flex items-center gap-4">
      <Info class="w-8 h-8 text-blue-500" /> {$t('info.videos.title')}
    </h2>
    <div class="grid grid-cols-1 md:grid-cols-1 gap-8">
      <div class="space-y-4">
        <div class="aspect-video rounded-3xl overflow-hidden shadow-xl">
          <iframe width="100%" height="100%" title="Guide 2" src="https://youtube.com/embed/qnUS5jaFu8c" frameborder="0" allowfullscreen></iframe>
        </div>
      </div>
    </div>
  </section>
</div>
