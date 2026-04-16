<script lang="ts">
  import { page } from "$app/stores";
  import SingersBrowser from "$lib/components/SingersBrowser.svelte";
  import { dataService } from "$lib/dataService";
  import { User } from "lucide-svelte";
  import { onMount } from "svelte";
  import { t } from "svelte-i18n";

  let currentUser: any = null;

  onMount(async () => {
    let data = await dataService.getData();
    if (!data) data = await dataService.checkUpdate();
    currentUser = data.users.find((u: any) => u.login === $page.params.user);
  });
</script>

<div class="container mx-auto py-8">
  <div
    class="mb-8 flex items-center gap-4 bg-gray-50 dark:bg-gray-800 p-6 rounded-2xl shadow-sm"
  >
    <div
      class="p-4 bg-primary-100 dark:bg-primary-900 rounded-full text-primary-600 dark:text-primary-400"
    >
      <User class="w-8 h-8" />
    </div>
    <div>
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
        {$t("user")}
        <span class="text-primary-600 dark:text-primary-400"
          >{currentUser?.name || $page.params.user}</span
        >
      </h1>
      <p class="text-gray-500 mt-1">{$t("user-singers")}</p>
    </div>
  </div>

  <SingersBrowser targetUser={$page.params.user} />
</div>
