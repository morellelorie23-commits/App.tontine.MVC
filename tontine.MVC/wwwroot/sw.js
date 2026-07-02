const CACHE_NAME = 'tontineapp-v1';
const STATIC_ASSETS = [
    '/',
    '/css/theme.css',
    '/js/tableutils.js',
    '/js/i18n.js',
    '/js/lucide.min.js',
    '/lib/bootstrap/dist/css/bootstrap.min.css',
    '/lib/bootstrap/dist/js/bootstrap.bundle.min.js',
    '/manifest.json'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => cache.addAll(STATIC_ASSETS))
    );
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k)))
        )
    );
    self.clients.claim();
});

self.addEventListener('fetch', event => {
    // Navigation requests — network first, fallback to cache
    if (event.request.mode === 'navigate') {
        event.respondWith(
            fetch(event.request).catch(() =>
                caches.match('/').then(r => r || new Response('Hors-ligne', { status: 503 }))
            )
        );
        return;
    }

    // Static assets — cache first
    if (['css', 'js', 'woff2', 'woff', 'ttf'].some(ext => event.request.url.endsWith('.' + ext))) {
        event.respondWith(
            caches.match(event.request).then(r => r || fetch(event.request).then(resp => {
                const clone = resp.clone();
                caches.open(CACHE_NAME).then(c => c.put(event.request, clone));
                return resp;
            }))
        );
        return;
    }

    // Everything else — network only
    event.respondWith(fetch(event.request));
});
