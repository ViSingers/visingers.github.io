<script lang="ts">
  import { page } from "$app/stores";
  import { onMount } from "svelte";
  import { t, locale } from "svelte-i18n";
  import SingersBrowser from "$lib/components/SingersBrowser.svelte";
  import { dataService } from "$lib/dataService";
  
  import { Users, Github, Star, Calendar, Mic2 } from "lucide-svelte";
  import { Button, Spinner } from "flowbite-svelte";

  let currentGroup: any = null;
  let loading = true;

  onMount(async () => {
    const data = await dataService.getData();
    currentGroup = data.groups.find((g: any) => g.repositoryName === $page.params.group);
    loading = false;
  });

  $: description = currentGroup?.description?.[$locale?.split('-')[0]] 
                || currentGroup?.description?.['en'] 
                || "";

  $: avatarUrl = currentGroup 
    ? `https://raw.githubusercontent.com/${currentGroup.repositoryPath}/${currentGroup.avatarUrl || 'main/image.png'}` 
    : '';

  function formatDate(dateString: string) {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString($locale || 'en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }
</script>

{#if loading}
  <div class="flex flex-col justify-center items-center h-64 space-y-4">
    <Spinner size="12" color="gray" />
    <p class="text-gray-500">Loading group details...</p>
  </div>
{:else if !currentGroup}
  <div class="text-center py-20 space-y-6">
    <div class="text-6xl">😕</div>
    <h2 class="text-3xl font-bold text-gray-900 dark:text-white">Group not found</h2>
    <p class="text-gray-500">The group you are looking for doesn't exist in our database.</p>
    <Button href="/" pill color="dark">Back to Home</Button>
  </div>
{:else}
  <div class="max-w-5xl space-y-16 pb-20 pt-10 px-4 xl:px-0">
    
    <section class="flex flex-col md:flex-row gap-12 items-center md:items-start text-center md:text-left">
      <div class="relative group shrink-0">
        <img 
          src={avatarUrl} 
          alt={currentGroup.name} 
          class="w-56 h-56 md:w-64 md:h-64 rounded-[2.5rem] object-cover shadow-2xl transition-transform duration-500 group-hover:scale-105 ring-8 ring-gray-50 dark:ring-gray-800"
          onerror={(e) => { e.currentTarget.src = 'https://via.placeholder.com/256?text=Group'; }}
        />
        
        <div class="absolute -bottom-4 -right-4 bg-white dark:bg-gray-800 p-3 rounded-2xl shadow-xl flex items-center gap-2">
           <Star class="w-5 h-5 text-yellow-400 fill-yellow-400" />
           <span class="font-bold text-lg">{currentGroup.stars || 0}</span>
        </div>
      </div>
      
      <div class="flex-1 space-y-6 flex flex-col items-center md:items-start">
        <div class="space-y-3">
          <h1 class="text-5xl md:text-6xl font-black tracking-tighter text-gray-900 dark:text-white">
            {currentGroup.name}
          </h1>
        </div>

        {#if description}
          <p class="text-gray-600 dark:text-gray-400 text-lg leading-relaxed max-w-2xl">
            {description}
          </p>
        {/if}

        <div class="flex flex-wrap justify-center md:justify-start gap-3">
          {#if currentGroup.participants?.length}
            <div class="flex items-center gap-2 px-4 py-2 bg-gray-100 dark:bg-gray-800 rounded-full text-sm font-bold text-gray-600 dark:text-gray-300">
              <Users class="w-4 h-4" />
              <span>{currentGroup.participants.length} {$t('group.members') || 'Members'}</span>
            </div>
          {/if}
          
          {#if currentGroup.createdAt}
            <div class="flex items-center gap-2 px-4 py-2 bg-gray-100 dark:bg-gray-800 rounded-full text-sm font-bold text-gray-600 dark:text-gray-300">
              <Calendar class="w-4 h-4" />
              <span>{formatDate(currentGroup.createdAt)}</span>
            </div>
          {/if}
        </div>

        <div class="pt-2">
          <Button 
            href={`https://github.com/${currentGroup.repositoryPath}`} 
            target="_blank" 
            color="dark" 
            pill 
            class="gap-2 px-8 py-3 text-base shadow-sm"
          >
            <Github class="w-5 h-5" /> {$t('singer.repository') || 'Repository'}
          </Button>
        </div>
      </div>
    </section>
  </div>
  <SingersBrowser targetGroup={$page.params.group} />
{/if}