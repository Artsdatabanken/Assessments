import { MenuToggle } from './menuToggle.js';
import { initThemeToggle } from './themeToggle.js';
initThemeToggle();
document.addEventListener('DOMContentLoaded', () => {
    new MenuToggle('menu-toggler', 'main-nav-container', {
        ignoreOutsideIds: ['search-form-wrapper'],
        allowFocusOutsideIds: ['edit-search-api-fulltext', 'edit-submit-site-search', 'theme-switcher'],
        trapFocus: true,
        autoFocusMenu: false
    });
});
//# sourceMappingURL=app.js.map