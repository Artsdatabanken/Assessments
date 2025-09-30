export class MenuToggle {
    constructor(togglerId, menuId, options) {
        // focus management
        this.cycleEls = [];
        this.trapFocus = (e) => {
            if (e.key !== 'Tab' || !this.cycleEls.length)
                return;
            const active = document.activeElement;
            const idx = active ? this.cycleEls.indexOf(active) : -1;
            // If focus is not in our cycle (e.g., user clicked into an allowed element that was rebuilt later), allow default.
            if (idx === -1)
                return;
            e.preventDefault();
            const dir = e.shiftKey ? -1 : 1;
            let nextIdx = idx + dir;
            if (nextIdx < 0)
                nextIdx = this.cycleEls.length - 1;
            if (nextIdx >= this.cycleEls.length)
                nextIdx = 0;
            const nextEl = this.cycleEls[nextIdx];
            nextEl === null || nextEl === void 0 ? void 0 : nextEl.focus();
        };
        const togglerEl = document.getElementById(togglerId);
        const menuEl = document.getElementById(menuId);
        if (!(togglerEl instanceof HTMLButtonElement)) {
            throw new Error(`Element #${togglerId} must be a <button>`);
        }
        if (!(menuEl instanceof HTMLElement)) {
            throw new Error(`Element #${menuId} must be a valid container`);
        }
        this.toggler = togglerEl;
        this.menu = menuEl;
        this.options = Object.assign({ closeOnEscape: true, closeOnOutsideClick: true, ignoreOutsideIds: [], trapFocus: true, autoFocusMenu: false, allowFocusOutsideIds: [] }, options);
        this.init();
    }
    init() {
        this.toggler.setAttribute('aria-expanded', 'false');
        this.toggler.setAttribute('aria-controls', this.menu.id);
        this.toggler.setAttribute('aria-haspopup', 'true');
        this.menu.setAttribute('role', 'navigation');
        this.toggler.addEventListener('click', (e) => this.toggle(e));
        this.toggler.addEventListener('keydown', (e) => this.handleKeydown(e));
        if (this.options.closeOnOutsideClick) {
            document.addEventListener('click', (e) => this.handleOutsideClick(e));
        }
        if (this.options.closeOnEscape) {
            document.addEventListener('keydown', (e) => this.handleEscapeKey(e));
        }
    }
    toggle(e) {
        e.preventDefault();
        const isExpanded = this.isExpanded();
        this.toggler.setAttribute('aria-expanded', String(!isExpanded));
        this.menu.classList.toggle('hidden', isExpanded);
        document.body.classList.toggle('menu-open', !isExpanded);
        if (!isExpanded) {
            this.activateFocusTrap();
        }
        else {
            this.deactivateFocusTrap();
        }
    }
    handleKeydown(e) {
        if (e.key === 'Enter' || e.key === ' ' || e.key === 'Spacebar') {
            e.preventDefault();
            this.toggle(e);
        }
    }
    handleOutsideClick(e) {
        const target = e.target;
        // Treat allowed-focus elements as "ignored" for outside-click close as well
        const ignoreIds = new Set([
            ...this.options.ignoreOutsideIds,
            ...this.options.allowFocusOutsideIds,
        ]);
        if (Array.from(ignoreIds).some((id) => {
            const el = document.getElementById(id);
            return el && el.contains(target);
        })) {
            return;
        }
        if (!this.menu.contains(target) && !this.toggler.contains(target) && this.isExpanded()) {
            this.close();
        }
    }
    handleEscapeKey(e) {
        if (e.key === 'Escape' && this.isExpanded()) {
            this.close();
            this.toggler.focus();
        }
    }
    isExpanded() {
        return this.toggler.getAttribute('aria-expanded') === 'true';
    }
    close() {
        this.toggler.setAttribute('aria-expanded', 'false');
        this.menu.classList.add('hidden');
        document.body.classList.remove('menu-open');
        this.deactivateFocusTrap();
        this.toggler.focus();
    }
    // ---------- Focus Trap ----------
    activateFocusTrap() {
        if (!this.options.trapFocus)
            return;
        this.buildFocusCycle();
        // Optional autofocus: first menu item if present (keep toggler focus otherwise)
        if (this.options.autoFocusMenu && this.cycleEls.length > 1) {
            // cycleEls[0] is the toggler; focus the first menu item if it exists
            const firstMenuItem = this.cycleEls[1];
            if (firstMenuItem)
                firstMenuItem.focus();
        }
        document.addEventListener('keydown', this.trapFocus, true);
    }
    deactivateFocusTrap() {
        document.removeEventListener('keydown', this.trapFocus, true);
        this.cycleEls = [];
    }
    buildFocusCycle() {
        const isVisible = (el) => !!(el.offsetWidth || el.offsetHeight || el.getClientRects().length);
        const isFocusable = (el) => {
            if (el.hasAttribute('disabled'))
                return false;
            if (!isVisible(el))
                return false;
            // If it has tabindex -1, skip
            const ti = el.getAttribute('tabindex');
            if (ti !== null && Number(ti) < 0)
                return false;
            return true;
        };
        const focusableSelectors = 'a[href], button, input, select, textarea, [tabindex]:not([tabindex="-1"])';
        const menuEls = Array.from(this.menu.querySelectorAll(focusableSelectors)).filter(isFocusable);
        const allowedOutsideEls = this.options.allowFocusOutsideIds
            .map((id) => document.getElementById(id))
            .filter((el) => !!el && isFocusable(el));
        // Build cycle: [toggler] -> [menu items…] -> [allowed outside…]
        const ordered = [this.toggler, ...menuEls, ...allowedOutsideEls];
        // De-duplicate while preserving order
        const seen = new Set();
        this.cycleEls = ordered.filter((el) => {
            if (seen.has(el))
                return false;
            seen.add(el);
            return true;
        });
    }
}
//# sourceMappingURL=menuToggle.js.map