import createCache, { type EmotionCache } from '@emotion/cache';
import { prefixer } from 'stylis';
import rtlPlugin from 'stylis-plugin-rtl';
import type { AppDirection } from './theme';

const ltrCache = createCache({ key: 'mui', prepend: true });
const rtlCache = createCache({ key: 'mui-rtl', stylisPlugins: [prefixer, rtlPlugin], prepend: true });

export function getEmotionCache(direction: AppDirection): EmotionCache {
  return direction === 'rtl' ? rtlCache : ltrCache;
}
