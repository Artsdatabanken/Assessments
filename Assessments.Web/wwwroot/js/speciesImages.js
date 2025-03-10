// Get species images from taxon pages

const adbLink = 'https://artsdatabanken.no';

const getImageList = (doc) => {
    const imageClassName = 'js-lazy-taxonimage'
    return Array.prototype.map.call(doc.getElementsByClassName(imageClassName), el => el.parentElement.parentElement);
}

const updateMetaData = (element) => {
    element.href = `https://artsdatabanken.no/Pages${element.href.split('Media')[1]}`

    Array.prototype.map.call(element.children[0].children, child => {
        if (child.nodeName == 'IMG') {
            const location = child.src.split('/Content')[1];
            child.src = `https://artsdatabanken.no/Content${location}`;
            child.style.width = '26px';
            child.style['padding-bottom'] = '0';
        }
        return child;
    });
    return element;
}

const renderSpeciesImage = (targetElement, element) => {
    const imagewrapper = document.createElement('div');
    const img = document.createElement('img');

    const datasetSrc = element.children[0].children[0].children[0].attributes.srcset.value.split('?')[0];
    img.alt = "Bilde av arten";

    img.src = `https://artsdatabanken.no${datasetSrc}?mode=320x320`;
    img.dataset.src = '';
    img.style.height = 'auto';
    img.style.width = '200px';
    img.style.padding = '0';
    imagewrapper.style['margin-bottom'] = '20px';

    const metaData = updateMetaData(element.children[1].children[0])
    
    imagewrapper.appendChild(img);
    imagewrapper.appendChild(element.children[1].children[0]);
    console.log('\nmeta', imagewrapper)
    targetElement.appendChild(imagewrapper);
}

const renderHeader = (element) => {
    const separatorLine = document.createElement('hr');
    const header = document.createElement('h3');
    header.innerText = 'Bilder av arten';
    element.appendChild(separatorLine);
    element.appendChild(header);
}

const addLinkToTaxonPage = (element, url) => {
    const link = document.createElement('a');
    link.href = url;
    link.innerText = 'Se flere bilder av arten her';
    element.appendChild(link);
}

const getAssessmentImages = () => {
    const taxonPagesElements = document.getElementsByClassName('taxon-page-link');
    const taxonPagesUrls = Array.prototype.map.call(taxonPagesElements, (el) => {
        return el.href;
    });

    Array.prototype.forEach.call(taxonPagesUrls, (url) => {
        fetch(url)
            .then((res) => {
                return res.text();
            })
            .then((text) => {
                const parser = new DOMParser();
                const doc = parser.parseFromString(text, 'text/html');
                const images = getImageList(doc);
                const targetClassName = 'species-images-' + url.split('/').reverse()[0];
                const targetElement = document.getElementsByClassName(targetClassName)[0];

                images.length && renderHeader(targetElement);


                for (let i = 0; i < images.length; i++) {
                    if (i == 4) {
                        addLinkToTaxonPage(targetElement, url);
                        return;
                    }
                    const imageText = images[i].children;
                    renderSpeciesImage(targetElement, images[i]);
                };
            })
            .catch(() => {
                return;
            })
    });
}

getAssessmentImages();
