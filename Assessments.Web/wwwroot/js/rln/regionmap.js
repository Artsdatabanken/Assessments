const regionsData = document.getElementById("regionsMap").getAttribute("data-regions");
const shortNames = JSON.parse(regionsData);

const dimensions = {
    width: 500,
    height: 600
}

const selectedColor = "#00ff0066"; 

fetch("/json/naturetyperegions.json").then(response => {
    return response.json();
}).then(data => {
    const projection = d3.geoIdentity()
        .reflectY(true)
        .fitSize([dimensions.width, dimensions.height], data);

    const path = d3.geoPath(projection);

    d3.select("#regionsMap")
        .append("svg")
        .attr("preserveAspectRatio", "xMinYMin meet")
        .attr("viewBox", "0 0 500 600")
        .selectAll()
        .data(data.features)
        .join("path")
        .attr("d", path)
        .style("stroke-width", ".3")
        .style("stroke", "black")
        .attr("fill",
            function (d) {
                const regionShort = d.properties["Region_kort"];
                if (regionShort) {
                    if (shortNames.includes(regionShort.toLowerCase())) {
                        return selectedColor;
                    }
                }
                return d.properties["rgb"];
            }
        )
        .append("title").text((d) => d.properties["Region"]);
}).catch(err => {
    console.warn(err);
});
