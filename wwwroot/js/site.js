// Modo claro/oscuro y flechas del carrusel.
document.addEventListener('click', function (e) {
    var toggle = e.target.closest('[data-theme-toggle]');
    if (toggle) {
        var root = document.documentElement;
        var next = root.dataset.theme === 'dark' ? 'light' : 'dark';
        root.dataset.theme = next;
        try { localStorage.setItem('fl-theme', next); } catch (err) { }
        return;
    }
    var arrow = e.target.closest('[data-scroll]');
    if (arrow) {
        var track = document.getElementById(arrow.dataset.target);
        if (track) track.scrollBy({ left: Number(arrow.dataset.scroll) * track.clientWidth * 0.8, behavior: 'smooth' });
    }
});
