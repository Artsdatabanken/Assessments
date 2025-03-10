/*
This is taken from Natur i Norge 3 and modified slightly to fit for this project.
To see more use cases and code examples, visit Natur i Norge 3. 

<div class="adb-accordion accordion-primary open">
    <button class="adb-accordion-button adb-accordion-button-variable-special Label_18" onclick="toggleAccordion(this)">
        <div class="accordion-left">
            <div class="accordion-title-value Heading_20_bold">@step.Verdi</div>
            <div class="accordion-title">
                <span class="accordion-title-code">@step.Kode</span>
                <b class="Heading_20_bold accordion-title-heading">@step.Beskrivelse</b>
            </div>
        </div>
        <span class="material-icons-outlined add">expand_more</span>
        <span class="material-icons-outlined remove">expand_less</span>
    </button>
    <div class="adb-accordion-content Body_M">
        add content here
    </div>
</div>
*/

const toggleAccordion = (element) => {
    const openClassName = 'open';
    const parent = element.parentElement;
    const isOpen = parent.classList.contains(openClassName);
    if (isOpen) {
        parent.classList.remove(openClassName);
    }
    else {
        parent.classList.add(openClassName);
    }
};
const triggerAccordion = () => {
    const accordionClassName = 'adb-accordion';
    const smallAccordionClassName = 'adb-accordion-small';
    const openClassName = 'open';
    const accordions = document.getElementsByClassName(accordionClassName);
    const smallAccordions = document.getElementsByClassName(smallAccordionClassName);
    if (accordions.length != 0) {
        Array.prototype.forEach.call(accordions, (element) => {
            if (element === null || element === void 0 ? void 0 : element.classList.contains(openClassName)) {
                element.classList.remove(openClassName);
            }
        });
    }
    if (smallAccordions.length != 0) {
        Array.prototype.forEach.call(smallAccordions, (element) => {
            if (element === null || element === void 0 ? void 0 : element.classList.contains(openClassName)) {
                element.classList.remove(openClassName);
            }
        });
    }
};
