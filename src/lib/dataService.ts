import { get, set } from 'idb-keyval';

const DATA_URL = 'https://raw.githubusercontent.com/ViSingers/singers-data/main/data.min.json';
const STORAGE_KEY = 'singers_data';
const LAST_FETCH_KEY = 'last_fetch_time';
const FETCH_INTERVAL = 5 * 1000;

export const dataService = {
  async init() {
    await this.checkUpdate();
    setInterval(() => this.checkUpdate(), FETCH_INTERVAL);
  },

  async checkUpdate() {
    const lastFetch = await get(LAST_FETCH_KEY);
    const now = Date.now();

    if (!lastFetch || now - lastFetch > FETCH_INTERVAL) {
      console.log('Checking for data updates...');
      try {
        const response = await fetch(DATA_URL);

        if (response.ok) {
          const rawData = await response.json();
          const userMap = new Map(rawData.users.map((u: any) => [u.id, u]));
          rawData.singers = rawData.singers.map((s: any) => ({
         ...s,
         creatorLogin: (userMap.get(s.creatorId) as any)?.login || 'unknown',
         creatorName: (userMap.get(s.creatorId) as any)?.name || 'unknown'
       }));    
          await set(STORAGE_KEY, rawData);
          await set(LAST_FETCH_KEY, now);
          console.log('Data updated successfully');
          return rawData;
        }
      } catch (e) {
        console.error('Failed to fetch data updates', e);
      }
    }
    return await this.getData();
  },

  async getData() {
    const data = await get(STORAGE_KEY);
    if (data && data.singers && data.users && !data.singers[0]?.creatorLogin) {
       const userMap = new Map(data.users.map((u: any) => [u.id, u]));
       data.singers = data.singers.map((s: any) => ({
         ...s,
         creatorLogin: (userMap.get(s.creatorId) as any)?.login || 'unknown',
         creatorName: (userMap.get(s.creatorId) as any)?.name || 'unknown'
       }));       
    }

    if (!data || !data.singers || !data.users) {
      await this.checkUpdate();
    }

    return data;
  }
};
