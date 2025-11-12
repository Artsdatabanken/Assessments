document.addEventListener("DOMContentLoaded", () => {
    var data = JSON.parse(document.getElementById('changes-data').innerHTML);

    var width = 950;
    var height = 470 + (data.nodes.length * 15);

    var svg = d3.select("#sankey-target")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .append("g");

    var sankey = d3.sankey()
        .extent([[1, 1], [width - 1, height - 6]])
        .nodeWidth(30);

    var { nodes, links } = sankey({
        nodes: data.nodes,
        links: data.links
    });

    var nodeGroup = svg
        .append("g")
        .attr("stroke", 20)
        .attr("stroke-width", 20)
        .attr("stroke-opacity", 1);

    nodeGroup
        .selectAll("rect")
        .data(nodes)
        .join("rect")
        .attr("class", "node")
        .attr("x", (d) => d.x0)
        .attr("y", (d) => d.y0)
        .attr("height", (d) => d.y1 - d.y0)
        .attr("width", (d) => d.x1 - d.x0)
        .style("fill", (d) => d.color)
        .append("title")
        .text((d) => `${d.name} ${d.category}`);

    nodeGroup
        .selectAll("text")
        .data(nodes)
        .join("text")
        .attr("class", "sankey-node-text")
        .attr("x", d => d.x0 < width / 2 ? d.x1 + 5 : d.x0 - 5)
        .attr("y", d => (d.y1 + d.y0) / 2)
        .attr("dy", "0.35em")
        .attr("text-anchor", d => d.x0 < width / 2 ? "start" : "end")
        .text((d) => d.name + ' ' + d.category);

    var linkGroup = svg
        .append("g")
        .attr("fill", "none")
        .attr("stroke-opacity", 0.6)
        .selectAll("g")
        .data(links)
        .join("g")
        .style("mix-blend-mode", "multiply")
        .attr("class", "sankey-link")
        .append("path")
        .attr("d", d3.sankeyLinkHorizontal())
        .style("fill", "none")
        .style("stroke-width", d => d.width);
});
