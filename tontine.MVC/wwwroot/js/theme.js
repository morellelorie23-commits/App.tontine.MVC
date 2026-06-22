// Gestion du thème (Mode clair/sombre)

const THEME_KEY = 'tontine-theme';
const THEME_LIGHT = 'light';
const THEME_DARK = 'dark';

/**
 * Initialise le thème au chargement de la page
 */
function initTheme() {
    const savedTheme = localStorage.getItem(THEME_KEY) || THEME_LIGHT;
    applyTheme(savedTheme);
    updateThemeToggle(savedTheme);
}

/**
 * Applique le thème
 */
function applyTheme(theme) {
    const html = document.documentElement;

    if (theme === THEME_DARK) {
        html.classList.add('dark-mode');
    } else {
        html.classList.remove('dark-mode');
    }

    localStorage.setItem(THEME_KEY, theme);
}

/**
 * Bascule entre les thèmes
 */
function toggleTheme() {
    const currentTheme = localStorage.getItem(THEME_KEY) || THEME_LIGHT;
    const newTheme = currentTheme === THEME_LIGHT ? THEME_DARK : THEME_LIGHT;

    applyTheme(newTheme);
    updateThemeToggle(newTheme);
}

/**
 * Met à jour l'icône du bouton de thème
 */
function updateThemeToggle(theme) {
    const toggle = document.getElementById('theme-toggle');
    if (toggle) {
        toggle.textContent = theme === THEME_DARK ? '☀️' : '🌙';
        toggle.title = theme === THEME_DARK ? 'Mode clair' : 'Mode sombre';
    }
}

/**
 * Initialise au chargement du DOM
 */
document.addEventListener('DOMContentLoaded', initTheme);

// En cas de chargement rapide, initialiser immédiatement
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initTheme);
} else {
    initTheme();
}
