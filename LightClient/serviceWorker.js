const cacheName = "lets-go-biking-pwa";
const filesToCache = [
  "/",
  "/index.html",
  "/css/styles.css",
  "/js/script.js"
];

self.addEventListener("install", (event) => {
  event.waitUntil(
    caches.open(cacheName).then((cache) => {
      return cache.addAll(filesToCache);
    })
  );
});

self.addEventListener("fetch", (event) => {
  event.respondWith(
    caches.match(event.request).then((response) => {
      return response || fetch(event.request);
    })
  );
});