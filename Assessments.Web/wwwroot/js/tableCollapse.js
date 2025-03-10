const toggleTableRows = (classNames) => {
    const tableRows = document.getElementsByClassName(classNames);
    const buttonIcons = document.getElementsByClassName(`${classNames.split(' ')[0]} material-icons`);

    Array.prototype.forEach.call(buttonIcons, el => {
        el.classList.toggle('water-area-hide');
    });

    Array.prototype.forEach.call(tableRows, el => {
        el.classList.toggle('water-area-hide');
    });
}

const hideWaterAreas = () => {
    const tableRows = document.getElementsByClassName('water-area');

    Array.prototype.forEach.call(tableRows, el => {
        el.classList.add('water-area-hide');
    });
}

const showExpandWaterAreasButton = () => {
    const tableRows = document.getElementsByClassName('collapse_water_area');

    Array.prototype.forEach.call(tableRows, el => {
        el.classList.remove('water-area-hide');
    });
}

const hidePlaceholderTextNoJs = () => {
    const tableRows = document.getElementsByClassName('water-area-no-js');

    Array.prototype.forEach.call(tableRows, el => {
        el.classList.add('water-area-hide');
    });
}

const initializeWaterAreas = () => {
    hideWaterAreas();
    showExpandWaterAreasButton();
    hidePlaceholderTextNoJs();
}

initializeWaterAreas();