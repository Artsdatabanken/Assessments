/* THEME MODES 
 - Swaps out classes on body adjusting to different css-themes
 - Checks manually set user preference as default (browser,os)
 */

var cookiesave = true;

// Open/Close Menu
function expandThemeButtons() {
    let mainparent = document.getElementById('themeselectordropdown');
    if (mainparent.style && mainparent.style.display == "block") {
        mainparent.style.display = "none";
    } else {
        mainparent.style.display = "block";
    }
}

//// Handle theme changes
//if (window.hasOwnProperty('acceptedcookie') && acceptedcookie == "yes") {
//    cookiesave = true;
//} 

function toggleOnOff(thisbutton, activeornot) {
    if (thisbutton != null) {
        var child = thisbutton.children[1];

        if (activeornot == "active") {
            child.innerHTML = child.innerHTML.replace("off", "on");
        } else {
            child.innerHTML = child.innerHTML.replace("on", "off");
        }
    }
    
}

function highContrastMode(thisbutton,className) {
    var mainparent = document.body;
    if (mainparent.classList && mainparent.classList.contains(className)) {
        mainparent.classList.remove(className);
        thisbutton.classList.remove(className); 
        toggleOnOff(thisbutton, "inactive");
    }else {
        mainparent.classList.add(className); 
        thisbutton.classList.add(className);
        toggleOnOff(thisbutton, "active");
    }
    if (cookiesave) {
        themeCookie();
    }
}

if (cookiesave && getCookie('theme') != null) {  
    let activethemes = getCookie('theme');
    document.body.classList = activethemes;
    if (activethemes.includes("darktheme")) {
        let darkmodebutton = document.getElementById("darkmodebutton");
        toggleOnOff(darkmodebutton, "active");
    }
    if (activethemes.includes("highcontrast")) {
        let highcontrastbutton = document.getElementById("highcontrastbutton");
        toggleOnOff(highcontrastbutton, "active");
    }
} else {
    // Sets initial preferences
    const initialContrast = matchMedia('(forced-colors: active)');
    const initialTheme = matchMedia('(prefers-color-scheme: dark)');

    const themeChecks = () => {
        // Detects user preference for high contrast and dark mode themes
        var mainparent = document.body;
        if (initialTheme && initialTheme.matches) {
            let darkmodebutton = document.getElementById("darkmodebutton");
            toggleOnOff(darkmodebutton, "active");
            mainparent.classList.add("darktheme");          
            
        } else {
            if (mainparent.classList) {
                mainparent.classList.remove("darktheme");
            }
        }
        if (initialContrast.matches) {
            let highcontrastbutton = document.getElementById("highcontrastbutton");
            mainparent.classList.add("highcontrast");
            toggleOnOff(highcontrastbutton, "active");
        } else {
            if (mainparent.classList) {
                mainparent.classList.remove("highcontrast");
            }
        }
    }

    // run the checks immediately
    themeChecks();

    // listen for any changes performed by people tinkering with their settings
    initialContrast.addListener(themeChecks);
    initialTheme.addListener(themeChecks);
}
