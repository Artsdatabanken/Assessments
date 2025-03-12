window.addEventListener("click", function (e) {
    // Closes menu dropdown menu when clicking outside it.
    if (document.getElementById("adb-headermenu")) {
        const target = e.target;
        if (!document.getElementById("adb-headermenu").contains(target)) {
            const headerMenu = document.getElementById("dropdown-header-menu");
            if (headerMenu) {
                headerMenu.style.display = "none";
                const dropDownButton = document.getElementById("toggle-dropdown-button");
                dropDownButton.setAttribute("aria-expanded", "false");
            }
        }
    }
    // Closes search field dropdown menu when clicking outside it.
    const searchField = document.getElementById("adb_header_search");
    const searchFieldToggleButton = document.getElementById("adb_header_search_mobile");
    if (searchField) {
        const target = e.target;
        if (!searchField.contains(target) && !searchFieldToggleButton.contains(target)) {
            const closedClassName = "closed";
            searchField.classList.add(closedClassName);
        }
    }
});

const toggleDropdown = (button) => {
    const headerMenu = document.getElementById("dropdown-header-menu");
    const isOpen = headerMenu.style.display === "block";
    if (isOpen) {
        headerMenu.style.display = "none";
        button.setAttribute("aria-expanded", "false");
    }
    else {
        headerMenu.style.display = "block";
        button.setAttribute("aria-expanded", "true");
    }
};

const toggleSearch = () => {
    const closedClassName = "closed";
    const id = "adb_header_search";
    const searchField = document.getElementById(id);
    if (searchField.classList.contains(closedClassName)) {
        searchField.classList.remove(closedClassName);
    }
    else {
        searchField.classList.add(closedClassName);
    }
};