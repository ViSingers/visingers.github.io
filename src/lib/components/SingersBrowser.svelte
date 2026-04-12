<script lang="ts">
  import { onMount } from 'svelte';
  import { dataService } from '$lib/dataService';
  import { t } from 'svelte-i18n';
  import { Select, Label, Checkbox, Card, Spinner, Accordion, AccordionItem, MultiSelect, Badge, Search } from 'flowbite-svelte';
  import { Star, Filter } from 'lucide-svelte';
  import { flip } from 'svelte/animate';
  import { crossfade } from 'svelte/transition';
  import tilt from '@savy011/tilt-svelte';

  export let targetUser: string | null = null;
  export let targetGroup: string | null = null;

  let data: any = null;
  let loading = true;
  let searchQuery = '';
  let selectedSort = 'popular';
  let showIfNoVoicebanks = true;
  
  let selectedLanguages: string[] = [];
  let selectedVbTypes: string[] = [];
  let selectedTags: string[] = [];

  onMount(async () => {
    data = await dataService.getData();
    if (!data) data = await dataService.checkUpdate();
    loading = false;
  });

  $: allLanguages = data ? Array.from(new Set(data.singers.flatMap((s: any) => s.voicebanks?.flatMap((vb: any) => vb.languages || []) || []))).map(l => ({value: l, name: (l as string).toLowerCase()})) : [];
  $: allVbTypes = data ? Array.from(new Set(data.singers.flatMap((s: any) => s.voicebanks?.map((vb: any) => vb.type) || []))).map(t => ({value: t, name: t})) : [];
  $: allTags = data ? Array.from(new Set(data.singers.flatMap((s: any) => s.tags?.map((t: any) => t.name) || []))).map(t => ({value: t, name: t})) : [];

  $: filteredSingers = data?.singers.filter((singer: any) => {
    const matchesSearch = singer.name.toLowerCase().includes(searchQuery.toLowerCase()) || 
                         singer.creatorLogin.toLowerCase().includes(searchQuery.toLowerCase());
    const hasVoicebanks = singer.voicebanks && singer.voicebanks.length > 0;
    const matchesVoicebankFilter = showIfNoVoicebanks || hasVoicebanks;
    
    const matchesUser = targetUser ? singer.creatorLogin.toLowerCase() === targetUser.toLowerCase() : true;
    const matchesGroup = targetGroup ? (singer.groups || []).some((g: any) => g.repositoryName.toLowerCase() === targetGroup.toLowerCase()) : true;
    
    const singerLangs = singer.voicebanks?.flatMap((vb: any) => vb.languages || []) || [];
    const matchesLang = selectedLanguages.length === 0 || selectedLanguages.some(l => singerLangs.includes(l));
    const singerVbTypes = singer.voicebanks?.map((vb: any) => vb.type) || [];
    const matchesVbType = selectedVbTypes.length === 0 || selectedVbTypes.some(t => singerVbTypes.includes(t));
    const singerTags = singer.tags?.map((t: any) => t.name) || [];
    const matchesTags = selectedTags.length === 0 || selectedTags.every(t => singerTags.includes(t));
    
    return matchesSearch && matchesVoicebankFilter && matchesLang && matchesVbType && matchesTags && matchesUser && matchesGroup;
  }) || [];

  $: sortedSingers = [...filteredSingers].sort((a, b) => {
    if (selectedSort === 'popular') return b.stars - a.stars;
    if (selectedSort === 'new') return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    if (selectedSort === 'old') return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
    if (selectedSort === 'recently-updated') return new Date(b.updatedAt || b.createdAt).getTime() - new Date(a.updatedAt || a.createdAt).getTime();
    if (selectedSort === 'voicebank-count') return (b.voicebanks?.length || 0) - (a.voicebanks?.length || 0);
    return 0;
  });

  $: sortOptions = [
    { value: 'popular', name: $t('sort.popular') },
    { value: 'new', name: $t('sort.new') },
    { value: 'old', name: $t('sort.old') },
    { value: 'recently-updated', name: $t('sort.recently-updated') },
    { value: 'voicebank-count', name: $t('sort.voicebank-count') }
  ];

  const [send, receive] = crossfade({
    duration: 300,
    fallback(node) {
      return {
        duration: 200,
        css: (t) => `opacity: ${t}`
      };
    }
  });

  function getAvatarUrl(singer: any) {
    return `https://raw.githubusercontent.com/${singer.creatorLogin}/${singer.repositoryName}/${singer.avatarUrl}`;
  }
</script>

<div class="space-y-8">
  {#if loading}
    <div class="flex flex-col justify-center items-center h-64 space-y-4">
      <Spinner size="12" color="gray" />
      <p class="text-gray-500 animate-pulse">Loading singers data...</p>
    </div>
  {:else}
    <Accordion>
      <AccordionItem>
        <span slot="header" class="flex items-center gap-3 text-lg font-semibold">
          <Filter class="w-5 h-5" />
          {$t('filters.title')}
        </span>
        
        <div class="flex flex-col gap-6 px-6 pb-8 pt-2">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="md:col-span-2">
              <Label for="search" class="mb-2">{$t('filters.name')}</Label>
              <Search bind:value={searchQuery} size="md" class="mb-2 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400"  placeholder={$t('button.search')} clearable />
            </div>
            <div>
              <Label for="sort" class="mb-2">{$t('filters.sort')}</Label>
              <Select id="sort" class="dark:bg-gray-700 dark:text-gray-400 dark:border-gray-600" items={sortOptions} bind:value={selectedSort} />
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div id="ms-languages">
              <Label class="mb-2">{$t('filters.languages')}</Label>
              <MultiSelect items={allLanguages} bind:value={selectedLanguages} placeholder="{$t('filters.languages-placeholder')}" size="sm" class="bg-gray-50 dark:bg-gray-700">
                {#snippet children({ item, clear })}
                  <Badge dismissable on:close={clear} class="!bg-[#ff679d] !text-white mx-0.5
                    [&>button]:!text-white 
                    [&>button:hover]:!text-white 
                    [&>button]:!bg-transparent 
                    [&>button:hover]:!bg-white/15 
                    [&>button]:hover:!bg-white/15">
                      {item.name}
                  </Badge>
                {/snippet}
              </MultiSelect>
            </div>
            <div id="ms-types">
              <Label class="mb-2">{$t('filters.types')}</Label>
              <MultiSelect items={allVbTypes} bind:value={selectedVbTypes} placeholder="{$t('filters.types-placeholder')}" size="sm"  class="bg-gray-50 dark:bg-gray-700">
                {#snippet children({ item, clear })}
                  <Badge dismissable on:close={clear} class="!bg-[#4ea6ea] !text-white mx-0.5
                    [&>button]:!text-white 
                    [&>button:hover]:!text-white 
                    [&>button]:!bg-transparent 
                    [&>button:hover]:!bg-white/15 
                    [&>button]:hover:!bg-white/15">
                      {item.name}
                    </Badge>
                {/snippet}
              </MultiSelect>
            </div>
            <div id="ms-tags">
              <Label class="mb-2">{$t('filters.tags')}</Label>
              <MultiSelect items={allTags} bind:value={selectedTags} placeholder="{$t('filters.tags-placeholder')}" size="sm"  class="bg-gray-50 dark:bg-gray-700">
                {#snippet children({ item, clear })}
                  <Badge dismissable on:close={clear} class="!bg-[#6b7280] !text-white mx-0.5
                    [&>button]:!text-white 
                    [&>button:hover]:!text-white 
                    [&>button]:!bg-transparent 
                    [&>button:hover]:!bg-white/15 
                    [&>button]:hover:!bg-white/15">
                      {item.name}
                    </Badge>
                {/snippet}
              </MultiSelect>  
            </div>
          </div>
          
          <div class="flex items-center gap-3 p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors w-fit">
            <Checkbox id="no-vb" bind:checked={showIfNoVoicebanks} />
            <Label for="no-vb" class="cursor-pointer">{$t('filters.show-if-no-voicebanks')}</Label>
          </div>
        </div>
      </AccordionItem>
    </Accordion>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      {#each sortedSingers as singer (singer.id)}
        <div class="rounded-xl block group h-full relative " {@attach tilt({ reverse: true, glare: true, "max-glare": 0.2 })} animate:flip={{ duration: 500, delay: 50 }} in:receive={{ key: singer.id }} out:send={{ key: singer.id }}>
        <a href="/{singer.creatorLogin}/{singer.repositoryName}" class="z-0" aria-label="View {singer.name}">
          <Card  padding="none" class="rounded-xl flex flex-col h-full overflow-hidden hover:shadow-2xl dark:hover:shadow-black/60 transition-all duration-300 border-none bg-gray-50 dark:bg-gray-800">
            
            

            <div class="relative overflow-hidden aspect-square shrink-0">
               <img src={getAvatarUrl(singer)} alt={singer.name} class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105" loading="lazy" />
            </div>
            
            <div class="p-5 flex flex-col flex-grow ">
              <div class="mb-3">
                <h5 class="text-xl font-bold tracking-tight text-gray-900 dark:text-white truncate group-hover:text-gray-500 transition-colors relative z-10 pointer-events-none">
                  {singer.name}
                </h5>
                <div class="relative z-10 w-fit">
                  <a href="/users/{singer.creatorLogin}" class="text-sm text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 transition-colors block py-1 no-underline">
                    {singer.creatorName}
                  </a>
                </div>
              </div>
              
              <div class="flex flex-wrap gap-1.5 mb-4 relative z-10 pointer-events-none">
                {#each Array.from(new Set(singer.voicebanks?.flatMap(vb => vb.languages || []) || [])) as lang}
                  <span class="px-2.5 py-0.5 text-[10px] lowercase font-bold rounded-full text-white " style="background-color: #ff679d;">{lang}</span>
                {/each}
                {#each Array.from(new Set(singer.voicebanks?.map(vb => vb.type) || [])) as type}
                  <span class="px-2.5 py-0.5 text-[10px] lowercase font-bold rounded-full text-white" style="background-color: #4ea6ea;">{type}</span>
                {/each}
                {#each singer.tags as tag}
                  <span class="px-2.5 py-0.5 text-[10px] lowercase font-bold rounded-full text-white" style="background-color: #6b7280;">{tag.name}</span>
                {/each}
              </div>

              <div class="flex justify-between items-center pt-3 mt-auto border-t border-gray-200 dark:border-gray-700 relative z-10 pointer-events-none">
                <div class="flex items-center gap-1.5">
                  <Star class="w-4 h-4 text-yellow-400 fill-yellow-400" />
                  <span class="font-medium text-gray-700 dark:text-gray-300">{singer.stars}</span>
                </div>
                <div class="text-xs font-medium text-gray-500 bg-gray-200 dark:bg-gray-700 px-2 py-1 rounded-md">
                  {singer.voicebanks?.length || 0} VBs
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