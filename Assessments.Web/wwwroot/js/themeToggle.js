/**
 * Initialize theme based on user preference or system settings.
 */
export function initThemeToggle() {
    const root = document.documentElement;
    const themeSwitcher = document.getElementById('theme-switcher');
    if (!themeSwitcher)
        return;
    const label = themeSwitcher.querySelector('.theme-switcher__label');
    if (!label)
        return;
    const activeText = themeSwitcher.getAttribute('data-active-text') || 'Light mode';
    const inactiveText = themeSwitcher.getAttribute('data-inactive-text') || 'Dark mode';
    const THEMES = {
        DARK: 'dark-theme',
        LIGHT: 'light-theme',
    };
    const isValidTheme = (theme) => theme === THEMES.DARK || theme === THEMES.LIGHT;
    const applyTheme = (theme) => {
        root.classList.remove(THEMES.DARK, THEMES.LIGHT);
        root.classList.add(theme);
        localStorage.setItem('theme', theme);
        themeSwitcher.setAttribute('aria-pressed', theme === THEMES.DARK ? 'true' : 'false');
        label.textContent = theme === THEMES.DARK ? activeText : inactiveText;
    };
    const getInitialTheme = () => {
        const saved = localStorage.getItem('theme');
        if (isValidTheme(saved))
            return saved;
        return window.matchMedia('(prefers-color-scheme: dark)').matches
            ? THEMES.DARK
            : THEMES.LIGHT;
    };
    let currentTheme = getInitialTheme();
    applyTheme(currentTheme);
    themeSwitcher.addEventListener('click', (e) => {
        e.preventDefault();
        currentTheme = currentTheme === THEMES.DARK ? THEMES.LIGHT : THEMES.DARK;
        applyTheme(currentTheme);
    });
}
//# sourceMappingURL=themeToggle.js.map