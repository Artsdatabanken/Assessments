/* Redlist species 2021:
 * Info-tab actions for the assessment-pages. Openin/closing the big lists for 
 * impact factors, criteria and habitats. Re-arranging some items that has a
 * simpler (but more chaotic) display for users without javasctipt. 
 *
 * The rest:
 * If you want to add a section with tabs you need to make a main div with an id. 
 * This div should contain buttons which has their own ids and a "changetab" class, as well as setting one of them with class name "active".
 * Below the buttons you should place a new div with class names "tabbed_element_container assessment_tabs". 
 * tabbed_element_container is a global class, while assessment tabs is added to avoid targetting the spacial cases in redlist species 2021.
 * These classes gets the styling from the stylesheet and make sure the code below targets the correct elements.
 * This div will contain the content divs for the different tabs.
 * Each content div needs to have an id. You can add as many as you like.
 * The buttons, which are the tab selectors should have an onclick function and call the selectTab-function in this file.
 * Then you should have a section with tabs.
 *
 * Example code block:
 * <div id="block_id">
 *    <h2>
 *        Some heading
 *    </h2>
 *    <button id="button_1" class="changetab active" onclick="selectTab('button_1', 'tab_block_1' , 'block_id' )">First button starts as active</button>
 *    <button id="button_2" class="changetab" onclick="selectTab('button_2', 'tab_block_2' , 'block_id' )">Second button starts as inactive</button>
 *    <div class="tabbed_element_container assessment_tabs">
 *        <div id="tab_block_1">
 *            The first tab starts open
 *        </div>
 *        <div id="tab_block_2">
 *            The second tab starts closed
 *        </div>
 *    </div>
 * </div>
*/

/*****
*     Start of code especially for redlist species 2021
*****/

function criterialist(which, button) {
    // See more text by altering css of the clicked element's top parent node
    document.getElementById('summary_criteria').classList.remove("active");
    document.getElementById('full_list_criteria').classList.remove("active");

    if (which == 'full_list') {
        document.getElementById('criteria').classList.add("full_list");
        document.getElementById('criteria').classList.remove("summary");
    }
    if (which == 'summary') {
        document.getElementById('criteria').classList.add("summary");
        document.getElementById('criteria').classList.remove("full_list");
    }
    button.classList.add("active");
}

function impactlist(which, button) {
    // See more text by altering css of the clicked element's top parent node
    document.getElementById('summary').classList.remove("active");
    document.getElementById('full_list').classList.remove("active");

    if (which == 'full_list') {
        document.getElementById('impactfactors').classList.add("full_list");
        document.getElementById('impactfactors').classList.remove("summary");
    }
    if (which == 'summary') {
        document.getElementById('impactfactors').classList.add("summary");
        document.getElementById('impactfactors').classList.remove("full_list");
    }
    button.classList.add("active");
}

function expandCriteria(element, className) {
    expand(element, className, 'criteria');
}

function expandImpact(element, className) {
    expand(element, className, 'impactfactors');
}

function expand(element, className, id) {
    element = element.closest("li");
    var mainparent = document.getElementById(id).classList;
    // Never expand or collapse summary items
    if (mainparent.contains("full_list")) {
        if (element.classList.contains(className)) {
            element.classList.remove(className);
        } else {
            element.classList.add(className);
        }
    }
}

// Adds the class "opened_element" to all active tabs for use in the dropdown effects.
function first_close() {
    var elements = document.querySelectorAll(".tabbed_element_container li.active");

    Array.prototype.forEach.call(elements, el => {
        el.classList.add("opened_element");
    });
}

/*****
*     End of code especially for redlist species 2021
*****/

function showTabButtons() {
    // Users with javascript are shown the buttons to toggle tabs
    var elements = document.getElementsByClassName("changetab");
    for (let i in elements) {
        if (elements[i] && elements[i].style) {
            elements[i].style.visibility = "visible";
        }
    }
}

const setInactiveTabs = () => {
    const section = document.querySelectorAll(".tabbed_element_container.assessment_tabs");
    Array.prototype.forEach.call(section, el => {
        Array.prototype.forEach.call(el?.children, (node, index) => {
            if (index != 0) node?.classList.add('inactive');
        })
    });
}

const selectTab = (buttonId, tabId, mainSectionId) => {
    const active = 'active';
    const inactive = 'inactive';
    const elementButtons = document.querySelectorAll(`#${mainSectionId} button.changetab`);
    const elementSections = document.querySelectorAll(`#${mainSectionId} .tabbed_element_container.assessment_tabs`)[0]?.children;

    Array.prototype.forEach.call(elementButtons, button => {
        if (button?.id === buttonId) {
            button.classList.remove(inactive);
            button.classList.add(active)
        }
        else {
            button.classList.remove(active);
            button.classList.add(inactive);
        }
    });

    Array.prototype.forEach.call(elementSections, element => {
        if (element?.id === tabId) element.classList.remove(inactive);
        else element.classList.add(inactive);
    });
}

showTabButtons();
first_close();
setInactiveTabs();
if (document.getElementById('criteria') && document.getElementById('criteria').classList) {
    document.getElementById('criteria').classList.add("summary");
}
if (document.getElementById('impactfactors') && document.getElementById('impactfactors').classList) {
    document.getElementById('impactfactors').classList.add("summary");
}
