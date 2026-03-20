/* ===================================
   Asignación de Ejercicios - JavaScript Mínimo
   Solo: filtrado de categorías, toggle de tarjetas y validación
   =================================== */

(function () {

    // =====================
    // Filtrado de categorías por tipo de terapia
    // =====================
    var filtroTerapia = document.getElementById('idTerapia');
    var filtroCategoria = document.getElementById('idCategoria');

    if (filtroTerapia && filtroCategoria) {
        var todasLasCategorias = [];
        var opciones = filtroCategoria.querySelectorAll('option[data-terapia]');
        opciones.forEach(function (opt) {
            todasLasCategorias.push({
                value: opt.value,
                text: opt.textContent.trim(),
                terapia: opt.getAttribute('data-terapia')
            });
        });

        filtroTerapia.addEventListener('change', function () {
            var idTerapia = this.value;
            var currentValue = filtroCategoria.value;

            filtroCategoria.innerHTML = '<option value="">-- Todas --</option>';

            var categoriasFiltradas = idTerapia
                ? todasLasCategorias.filter(function (c) { return c.terapia == idTerapia; })
                : todasLasCategorias;

            categoriasFiltradas.forEach(function (c) {
                var option = document.createElement('option');
                option.value = c.value;
                option.textContent = c.text;
                if (c.value == currentValue) option.selected = true;
                filtroCategoria.appendChild(option);
            });
        });

        // Filtrar al cargar si hay terapia seleccionada
        if (filtroTerapia.value) {
            filtroTerapia.dispatchEvent(new Event('change'));
        }
    }

    // =====================
    // Click en tarjeta de ejercicio para toggle checkbox
    // =====================
    var cards = document.querySelectorAll('.ejercicio-card');
    cards.forEach(function (card) {
        card.addEventListener('click', function (e) {
            if (e.target.type === 'checkbox' || e.target.tagName === 'A') return;
            var checkbox = this.querySelector('.checkbox-ejercicio');
            if (checkbox) {
                checkbox.checked = !checkbox.checked;
                this.classList.toggle('selected', checkbox.checked);
            }
        });
    });

    // Visual feedback al marcar/desmarcar checkbox
    var checkboxes = document.querySelectorAll('.checkbox-ejercicio');
    checkboxes.forEach(function (cb) {
        cb.addEventListener('change', function () {
            var card = this.closest('.ejercicio-card');
            if (card) {
                card.classList.toggle('selected', this.checked);
            }
        });
    });

})();

// Validación antes de enviar el formulario de asignación
function validarSeleccion(form) {
    var marcados = form.querySelectorAll('input[name="idsEjercicios"]:checked');
    if (marcados.length === 0) {
        alert('Debe seleccionar al menos un ejercicio para asignar.');
        return false;
    }
    return confirm('¿Está seguro de asignar ' + marcados.length + ' ejercicio(s) al paciente?');
}
