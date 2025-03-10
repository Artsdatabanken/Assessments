/* Script for swapping between 
 * checkboxes which must be submitted for smaller screens and 
 * toggles with immediate update-effect for bigger screens. 
 * Checkboxes are default and workd for non-js users. */

function addSliders() {
    Array.prototype.forEach.call(submitCheckInputs, el => {
        const span = document.createElement("span");
        const div = document.createElement("div");
        span.setAttribute("class", "slider");
        div.setAttribute("class", "slider_container toggle_switch");
        parent = el.parentNode;
        parent.classList.add("switch_list_element");
        div.appendChild(span);
        parent.insertBefore(div, el.nextSibling);
    });
}

function addTogglesCheck() {
    if (!isSmallReader()) {
        addSliders();
    }
}

addTogglesCheck();
// TODO: WHEN resizing from mobile with unapplied changes to bigscreen, reset unapplied changes.


window.addEventListener('resize', function (event) {

    // ON RESIZE - choose between toggles and checkboxes. Complex, but necessary.
    if (isSmallReader()) {
        document.querySelectorAll('.switch_list_element').forEach(function (el) {
            el.classList.remove('switch_list_element')
        });
        document.querySelectorAll('.slider_container').forEach(function (el) {
            el.style.display = "none";
        });
    } else {
        if (document.querySelectorAll('.slider_container').length == 0) {
            addSliders();
        } 
        Array.prototype.forEach.call(submitCheckInputs, el => {
            parent = el.parentNode;
            parent.classList.add("switch_list_element");
        });
        document.querySelectorAll('.slider_container').forEach(function (el) {
            el.style.display = "inline-block";
        });
    }
}, true);