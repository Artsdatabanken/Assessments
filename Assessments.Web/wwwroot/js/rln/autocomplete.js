function autocomplete(input, data) {
    var currentFocus;
    input.addEventListener("input", function() {
        var b, i;
        const val = this.value;
        closeAllLists();
        if (!val) {
            return false;
        }
        currentFocus = -1;
        const a = document.createElement("div");
        a.setAttribute("id", this.id + "autocomplete-list");
        a.setAttribute("class", "autocomplete-items");
        this.parentNode.appendChild(a);
        for (i = 0; i < data.length; i++) {
            if (data[i].toUpperCase().includes(val.toUpperCase())) {
                b = document.createElement("div");
                b.innerHTML = makeQueryStrong(data[i], val);
                b.innerHTML += `<input type="hidden"" value="${data[i]}"">`;
                b.addEventListener("click", function() {
                    input.value = this.getElementsByTagName("input")[0].value;
                    closeAllLists();
                    input.closest("form").submit();
                });
                a.appendChild(b);
            }
        }
        return false;
    });

    input.addEventListener("keydown", function(e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x) x = x.getElementsByTagName("div");
        if (e.keyCode === 40) {
            currentFocus++;
            addActive(x);
        } else if (e.keyCode === 38) {
            currentFocus--;
            addActive(x);
        } else if (e.keyCode === 13) {
            if (currentFocus > -1) {
                if (x) {
                    x[currentFocus].click();
                    this.submit();
                }
            }
        } else if (e.keyCode === 27) {
            closeAllLists();
        }
    });

    function makeQueryStrong(str, query) {
        const n = str.toUpperCase();
        const q = query.toUpperCase();
        const x = n.indexOf(q);
        if (!q || x === -1) {
            return str;
        }
        const l = q.length;
        return str.substr(0, x) + "<strong>" + str.substr(x, l) + "</strong>" + str.substr(x + l);
    }

    function addActive(x) {
        if (!x)
            return false;
        removeActive(x);
        if (currentFocus >= x.length)
            currentFocus = 0;
        if (currentFocus < 0)
            currentFocus = (x.length - 1);
        x[currentFocus].classList.add("autocomplete-active");
        return false;
    }

    function removeActive(x) {
        for (let i = 0; i < x.length; i++) {
            x[i].classList.remove("autocomplete-active");
        }
    }

    function closeAllLists(element) {
        const x = document.getElementsByClassName("autocomplete-items");
        for (let i = 0; i < x.length; i++) {
            if (element !== x[i] && element !== input) {
                x[i].parentNode.removeChild(x[i]);
            }
        }
    }

    document.addEventListener("click", function(e) {
        closeAllLists(e.target);
    });
}

document.addEventListener("DOMContentLoaded", () => {
    fetch("/naturtyper/2025/suggestions").then(response => {
        return response.json();
    }).then(data => {
        autocomplete(document.getElementById("Name"), data);
    }).catch(err => {
        console.warn(err);
    });
});