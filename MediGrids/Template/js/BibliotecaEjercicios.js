document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("videoModal");
    const videoFrame = document.getElementById("videoFrame");
    const modalTitle = document.getElementById("videoModalLabel");

    const btnTabla = document.getElementById("btnVistaTabla");
    const btnCards = document.getElementById("btnVistaCards");
    const vistaTabla = document.getElementById("vistaTabla");
    const vistaCards = document.getElementById("vistaCards");

    const selectTerapia = document.getElementById("idTerapia");
    const selectCategoria = document.getElementById("idCategoria");

    if (btnTabla && btnCards && vistaTabla && vistaCards) {
        btnTabla.addEventListener("click", function () {
            vistaTabla.classList.remove("d-none");
            vistaCards.classList.add("d-none");

            btnTabla.classList.remove("btn-outline-success");
            btnTabla.classList.add("btn-success");

            btnCards.classList.remove("btn-success");
            btnCards.classList.add("btn-outline-success");
        });

        btnCards.addEventListener("click", function () {
            vistaCards.classList.remove("d-none");
            vistaTabla.classList.add("d-none");

            btnCards.classList.remove("btn-outline-success");
            btnCards.classList.add("btn-success");

            btnTabla.classList.remove("btn-success");
            btnTabla.classList.add("btn-outline-success");
        });
    }

    if (modal && videoFrame && modalTitle) {
        modal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget;
            if (!button) return;

            let url = button.getAttribute("data-video") || "";
            const nombre = button.getAttribute("data-nombre") || "ejercicio";

            modalTitle.textContent = "Video del ejercicio: " + nombre;

            if (url.includes("youtube.com/watch?v=")) {
                url = url.replace("watch?v=", "embed/");
            } else if (url.includes("youtu.be/")) {
                url = "https://www.youtube.com/embed/" + url.split("youtu.be/")[1].split("?")[0];
            }

            videoFrame.src = url;
        });

        modal.addEventListener("hidden.bs.modal", function () {
            videoFrame.src = "";
        });
    }

    if (selectTerapia && selectCategoria && typeof categoriasCompletas !== "undefined") {
        function cargarCategorias(idTerapiaSeleccionada) {
            selectCategoria.innerHTML = "";

            const opcionDefault = document.createElement("option");
            opcionDefault.value = "";
            opcionDefault.text = idTerapiaSeleccionada
                ? "-- Seleccione una categoría --"
                : "-- Primero seleccione una terapia --";

            selectCategoria.appendChild(opcionDefault);

            if (!idTerapiaSeleccionada) return;

            const filtradas = categoriasCompletas.filter(function (c) {
                return c.id_terapia == idTerapiaSeleccionada;
            });

            filtradas.forEach(function (categoria) {
                const option = document.createElement("option");
                option.value = categoria.id_categoria;
                option.text = categoria.nombre;

                if (typeof categoriaSeleccionada !== "undefined" &&
                    categoriaSeleccionada &&
                    categoria.id_categoria == categoriaSeleccionada) {
                    option.selected = true;
                }

                selectCategoria.appendChild(option);
            });
        }

        selectTerapia.addEventListener("change", function () {
            cargarCategorias(this.value);
            selectCategoria.value = "";
        });

        cargarCategorias(selectTerapia.value);
    }
});