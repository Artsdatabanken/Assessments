/* - JS relevant for the entire page */

// DOM elements
const submitCheckInputs = document.getElementsByClassName("submitOnclick");
const headerMenuRL = document.getElementById("headermenu");
const themeSelectorDropDownRL = document.getElementById("themeselectordropdown");

// Screen Size
const smallScreenSize = 750; // In case we want to tinker later.
const padScreenSize = 940; // In case we want to tinker later.

function isSmallReader(){
    return Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0) <= smallScreenSize;
}

function isPadSize() {
    return Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0) <= padScreenSize;
}

function closeHeadermenu() {
    if (headerMenuRL && headerMenuRL.classList &&
        headerMenuRL.classList.contains("show")) {
        headerMenuRL.classList.remove("show")
        headerMenuRL.classList.add("hide")
    }
}

function closeThemeselectordropdown() {
    if (themeSelectorDropDownRL &&
        themeSelectorDropDownRL.style &&
        themeSelectorDropDownRL.style.display == "block") {        
        themeSelectorDropDownRL.style.display = "none";
    }
}

function closeSidebarmenuitemclick(target) {
    const sidebarMenuItemClickRL = document.getElementsByClassName('sidebarmenu');
    Array.prototype.forEach.call(sidebarMenuItemClickRL, function (item) {
        if (isPadSize && item?.classList?.contains("expand") && !item.contains(target)) {
            item.classList.remove("expand")
        }
    });
}

document.addEventListener('click', e => {    
    // Click on entrie page
    if (!e.target.matches('#headermenu *')) {
        closeHeadermenu();
    }
    if (!e.target.matches('#theme_elements *')) { // Surrounding parent 
        closeThemeselectordropdown();
    }
    if (!e.target.matches('#sidebarmenu_container *')) { // Surrounding parent 
        closeSidebarmenuitemclick(e.target);
    }
    if (!e.target.matches('#tableOfContentsOuter *')) {
        if (document.getElementById('showTableOfContentList')) {
            document.getElementById('showTableOfContentList').checked = false;
        }
    }
});

document.addEventListener('keydown', (e) => {
    // Keypress on entire page

    if (e.code == "Escape") {
        // Only listen to escape clicks
        if (document.getElementById("Omoss")) {
            document.getElementById("Omoss").style.display = "none";
        }

        if (document.getElementById("Meny")) {
            document.getElementById("Meny").style.display = "none";
        }
        
        if (themeSelectorDropDownRL){
            themeSelectorDropDownRL.style.display = "none";
        }

        if (document.getElementById('showTableOfContentList')) {
            document.getElementById('showTableOfContentList').checked = false;
        }


        closeSidebarmenuitemclick(e.target);
        if (isSmallReader()) {
            // For elements with different ux for mobile
            if (document.getElementById("filters") &&
                !document.getElementById("filters").classList.contains("hide_on_smallscreen")) {
                // If filter box is open, close it.
                closeFilters();
            }
            closeHeadermenu();
            closeThemeselectordropdown();
        }
    }
});
