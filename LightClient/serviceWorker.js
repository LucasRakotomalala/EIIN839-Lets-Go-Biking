const cacheName = 'lets-go-biking-pwa';
const filesToCache = [
  '/',
  '/index.html',
  '/css/styles.css',
  '/js/script.js'
];

self.addEventListener('install', (e) => {
  e.waitUntil(
    caches.open(cacheName).then((cache) => {
      return cache.addAll(filesToCache);
    })
  );
});

self.addEventListener('fetch', (e) => {
  e.respondWith(
    caches.match(e.request).then((response) => {
      return response || fetch(e.request);
    })
  );
});