const tableOfContentButton = document.getElementById('tableOfContentsLabel');
const tableOfContentInput = document.getElementById('showTableOfContentList');
const tableOfContentsSmallScreen = document.getElementById('tableOfContentsSmallScreen');
const tableOfContentsOuter = document.getElementById('tableOfContentsOuter');

const hideTableOfContentsSmallScreen = () => {
    tableOfContentsSmallScreen?.classList.add('table-of-contents-small-screen-show');
    tableOfContentsOuter?.classList.add('table-of-contents-sticky');
}

const showTableOfContentButton = () => {
    tableOfContentButton?.classList.remove('table-of-contents-small-screen-button-hide');
}

const initializeTableOfContents = () => {
    hideTableOfContentsSmallScreen();
    showTableOfContentButton();
}

const closeTableOfContents = (event) => {
    if (event.target.nodeName == 'A')
        tableOfContentInput.checked = false;
}

initializeTableOfContents();