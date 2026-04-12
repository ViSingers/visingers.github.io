<script lang="ts">
  import { onMount } from 'svelte';
  import { dataService } from '$lib/dataService';
  import { t } from 'svelte-i18n';
  import { Card, Spinner } from 'flowbite-svelte';
  import { Star, Users } from 'lucide-svelte';
  import { flip } from 'svelte/animate';
  import { crossfade } from 'svelte/transition';
  import tilt from '@savy011/tilt-svelte';

  let data: any = null;
  let loading = true;

  onMount(async () => {
    data = await dataService.getData();
    if (!data) data = await dataService.checkUpdate();
    loading = false;
  });

  $: processedGroups = data ? data.groups.map((group: any) => {
    const langs = new Set<string>();
    const types = new Set<string>();

    const participants = group.participants || [];
    participants.forEach((p: any) => {
      const singer = data.singers.find((s: any) => s.id == p.id);
      
      if (singer && singer.voicebanks) {
        singer.voicebanks.forEach((vb: any) => {
          if (vb.languages) vb.languages.forEach((l: string) => langs.add(l));
          if (vb.type) types.add(vb.type);
        });
      }
    });

    return {
      ...group,
      uniqueLanguages: Array.from(langs),
      uniqueTypes: Array.from(types)
    };
  }) : [];

  const [send, receive] = crossfade({
    duration: 300,
    fallback(node) {
      return {
        duration: 200,
        css: (t) => `opacity: ${t}`
      };
    }
  });

  function getAvatarUrl(group: any) {
    return `https://raw.githubusercontent.com/${group.repositoryPath}/${group.avatarUrl || 'main/image.png'}`;
  }
</script>

<div class="space-y-8">
  {#if loading}
    <div class="flex flex-col justify-center items-center h-64 space-y-4">
      <Spinner size="12" color="gray" />
      <p class="text-gray-500 animate-pulse">Loading groups data...</p>
    </div>
  {:else}
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      {#each (processedGroups || []) as group (group.id)}
        <div class="rounded-xl block group h-full relative " {@attach tilt({ reverse: true, glare: true, "max-glare": 0.2 })} animate:flip={{ duration: 500, delay: 50 }} in:receive={{ key: group.id }} out:send={{ key: group.id }}>
          <a href="/groups/{group.repositoryName}" class="z-0" aria-label="View {group.name}">
            <Card padding="none" class="rounded-xl flex flex-col h-full overflow-hidden hover:shadow-2xl dark:hover:shadow-black/60 transition-all duration-300 border-none bg-gray-50 dark:bg-gray-800">
              
              <div class="relative overflow-hidden aspect-square shrink-0">
                 <img src={getAvatarUrl(group)} alt={group.name} class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105" loading="lazy" />
              </div>
              
              <div class="p-5 flex flex-col flex-grow">
                <div class="mb-3">
                  <h5 class="text-xl font-bold tracking-tight text-gray-900 dark:text-white truncate group-hover:text-gray-500 transition-colors relative z-10 pointer-events-none">
                    {group.name}
                  </h5>
                </div>
                
                <div class="flex flex-wrap gap-1.5 mb-4 relative z-10 pointer-events-none">
                  {#each group.uniqueLanguages as lang}
                    <span class="px-2.5 py-0.5 text-[10px] lowercase font-bold rounded-full text-white " style="background-color: #ff679d;">{lang}</span>
                  {/each}
                  {#each group.uniqueTypes  as type}
                    <span class="px-2.5 py-0.5 text-[10px] lowercase font-bold rounded-full text-white" style="background-color: #4ea6ea;">{type}</span>
                  {/each}
                </div>

                <div class="flex justify-between items-center pt-3 mt-auto border-t border-gray-200 dark:border-gray-700 relative z-10 pointer-events-none">
                  <div class="flex items-center gap-1.5">
                    <Star class="w-4 h-4 text-yellow-400 fill-yellow-400" />
                    <span class="font-medium text-gray-700 dark:text-gray-300">{group.stars || 0}</span>
                  </div>
                  <div class="flex items-center gap-1.5 text-xs font-medium text-gray-500 bg-gray-200 dark:bg-gray-700 px-2 py-1 rounded-md">
                    {group.participants?.length || 0} <Users class="h-4 w-4" />
                  </div>
                </div>
              </div>
            </Card>
          </a>
        </div>
      {/each}
    </div>
  {/if}
</div>