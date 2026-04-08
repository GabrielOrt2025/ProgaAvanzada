/* ===================================
   Agendar Citas - JavaScript
   =================================== */

// Variables globales (se inicializan desde la vista)
let categoriasPorTerapia = [];
let terapeutasData = [];
let categoriasPorTerapiaAgrupadas = {};
let selectedTerapia = null;
let selectedCategoria = null;
let selectedTerapeuta = null;
let selectedTerapeutaId = null;
let selectedFecha = null;
let selectedHora = null;

/**
 * Inicializa la aplicación con los datos del servidor
 */
function initAgendarCitas(categorias, terapeutas) {
    categoriasPorTerapia = categorias || [];
    terapeutasData = terapeutas || [];

    console.log('=== DATOS RECIBIDOS DEL SERVIDOR ===');
    console.log('Total Categorias:', categoriasPorTerapia.length);
    console.log('Categorias:', categoriasPorTerapia);
    console.log('Total Terapeutas:', terapeutasData.length);
    console.log('Terapeutas:', terapeutasData);

    // Agrupar categorías por terapia en el cliente
    categoriasPorTerapiaAgrupadas = {};
    if (categoriasPorTerapia && Array.isArray(categoriasPorTerapia)) {
        categoriasPorTerapia.forEach(cat => {
            // CORRECCIÓN: Usar PascalCase del ViewModel
            if (!categoriasPorTerapiaAgrupadas[cat.IdTerapia]) {
                categoriasPorTerapiaAgrupadas[cat.IdTerapia] = [];
            }
            categoriasPorTerapiaAgrupadas[cat.IdTerapia].push(cat);
        });
    }

    console.log('Categorías agrupadas por terapia:', categoriasPorTerapiaAgrupadas);
    console.log('Script cargado correctamente');
    registrarEventListeners();
}

/**
 * Registra todos los event listeners
 */
function registrarEventListeners() {
    // Paso 1: Manejo de seleccion de terapia
    document.getElementById('tipoTerapia').addEventListener('change', onTerapiaChange);
    document.getElementById('categoriaClinica').addEventListener('change', onCategoriaChange);

    // Navegacion entre pasos
    document.getElementById('btn-siguiente-1').addEventListener('click', onSiguientePaso1);
    document.getElementById('btn-anterior-2').addEventListener('click', onAnteriorPaso2);

    // Envio del formulario
    document.getElementById('form-paso-2').addEventListener('submit', onSubmitFormulario);

    console.log('Todos los event listeners registrados correctamente');
}

/**
 * Maneja el cambio de selección de tipo de terapia
 */
function onTerapiaChange() {
    const terapiaId = parseInt(this.value);
    selectedTerapia = this.options[this.selectedIndex].text;

    const categoriaSelect = document.getElementById('categoriaClinica');

    // Actualizar el mensaje inicial dinámicamente basado en la terapia seleccionada
    const mensajeInicial = selectedTerapia 
        ? `-- Seleccione una categoría de ${selectedTerapia} --`
        : '-- Primero seleccione un tipo de terapia --';

    categoriaSelect.innerHTML = `<option value="">${mensajeInicial}</option>`;

    // Limpiar la selección de terapeutas
    const container = document.getElementById('terapeutas-container');
    container.innerHTML = '<p class="text-muted">Primero seleccione una categoría clínica para ver los profesionales disponibles</p>';

    if (terapiaId && categoriasPorTerapiaAgrupadas[terapiaId]) {
        categoriaSelect.disabled = false;
        categoriasPorTerapiaAgrupadas[terapiaId].forEach(cat => {
            const option = document.createElement('option');
            // CORRECCIÓN: Usar PascalCase del ViewModel
            option.value = cat.IdCategoria;
            option.textContent = cat.Nombre;
            categoriaSelect.appendChild(option);
        });
    } else {
        categoriaSelect.disabled = true;
        categoriaSelect.innerHTML = '<option value="">-- Primero seleccione un tipo de terapia --</option>';
    }

    // NO cargar terapeutas aquí - solo cuando se seleccione la categoría
}

/**
 * Maneja el cambio de selección de categoría clínica
 */
function onCategoriaChange() {
    selectedCategoria = this.options[this.selectedIndex].text;
    const categoriaId = parseInt(this.value);

    console.log('=== CATEGORÍA SELECCIONADA ===');
    console.log('Categoría ID:', categoriaId);
    console.log('Categoría Nombre:', selectedCategoria);

    // Cargar terapeutas SOLO basados en la categoría clínica
    if (categoriaId) {
        cargarTerapeutasPorCategoria(categoriaId);
    } else {
        // Si se deselecciona la categoría, limpiar terapeutas
        const container = document.getElementById('terapeutas-container');
        container.innerHTML = '<p class="text-muted">Seleccione una categoría clínica para ver los profesionales disponibles</p>';
    }
}

/**
 * Carga y muestra los terapeutas disponibles filtrados por categoría clínica
 */
function cargarTerapeutasPorCategoria(categoriaId) {
    const container = document.getElementById('terapeutas-container');
    container.innerHTML = '';

    console.log('=== FILTRANDO TERAPEUTAS POR CATEGORÍA ===');
    console.log('Categoría ID:', categoriaId);
    console.log('Total terapeutas disponibles:', terapeutasData.length);

    if (!categoriaId) {
        container.innerHTML = '<p class="text-muted">Seleccione una categoría clínica para ver los profesionales disponibles</p>';
        return;
    }

    // Filtrar terapeutas SOLO por categoría clínica
    const terapeutasFiltrados = terapeutasData.filter(terapeuta => {
        return terapeuta.IdCategoria === categoriaId;
    });

    console.log('Terapeutas filtrados por categoría:', terapeutasFiltrados.length);
    mostrarTerapeutas(terapeutasFiltrados);
}

/**
 * Muestra la lista de terapeutas en el contenedor
 */
function mostrarTerapeutas(terapeutasFiltrados) {
    const container = document.getElementById('terapeutas-container');

    if (terapeutasFiltrados.length === 0) {
        container.innerHTML = '<p class="text-muted">No hay profesionales disponibles para esta categoría</p>';
        return;
    }

    terapeutasFiltrados.forEach(terapeuta => {
        const card = document.createElement('div');
        card.className = 'terapeuta-card';

        // Mostrar las especialidades del terapeuta
        const infoEspecialidades = [];
        if (terapeuta.Especialidades && terapeuta.Especialidades.length > 0) {
            infoEspecialidades.push(`${terapeuta.Especialidades.length} tipo(s) de terapia`);
        }
        if (terapeuta.Categorias && terapeuta.Categorias.length > 0) {
            infoEspecialidades.push(`${terapeuta.Categorias.length} categoría(s)`);
        }

        const especialidadesText = infoEspecialidades.length > 0
            ? `<small class="text-muted">Experiencia: ${infoEspecialidades.join(', ')}</small>` 
            : '';

        card.innerHTML = `
            <input type="radio" name="terapeuta" value="${terapeuta.IdTerapeuta}" id="terapeuta-${terapeuta.IdTerapeuta}">
            <label for="terapeuta-${terapeuta.IdTerapeuta}" style="cursor: pointer; width: 100%;">
                <h5>${terapeuta.Nombre} ${terapeuta.Apellidos}</h5>
                <p class="mb-0"><i class="lni lni-phone"></i> ${terapeuta.Telefono || 'No especificado'}</p>
                <p class="mb-0"><i class="lni lni-envelope"></i> ${terapeuta.Email || 'No especificado'}</p>
                ${especialidadesText}
            </label>
        `;

        card.addEventListener('click', function () {
            document.querySelectorAll('.terapeuta-card').forEach(c => c.classList.remove('selected'));
            card.classList.add('selected');
            card.querySelector('input[type="radio"]').checked = true;
            selectedTerapeuta = `${terapeuta.Nombre} ${terapeuta.Apellidos}`;
            selectedTerapeutaId = terapeuta.IdTerapeuta;
        });

        container.appendChild(card);
    });
}

/**
 * Avanza al paso 2 después de validar el paso 1
 */
function onSiguientePaso1() {
    console.log('Botón Siguiente clickeado');

    const terapia = document.getElementById('tipoTerapia').value;
    const categoria = document.getElementById('categoriaClinica').value;
    const terapeuta = document.querySelector('input[name="terapeuta"]:checked');

    console.log('Validación - Terapia:', terapia);
    console.log('Validación - Categoría:', categoria);
    console.log('Validación - Terapeuta:', terapeuta);

    if (!terapia || !categoria || !terapeuta) {
        alert('Por favor complete todos los campos antes de continuar');
        return;
    }

    // Actualizar resumen
    document.getElementById('resumen-terapia').textContent = selectedTerapia || 'No seleccionado';
    document.getElementById('resumen-categoria').textContent = selectedCategoria || 'No seleccionado';
    document.getElementById('resumen-terapeuta').textContent = selectedTerapeuta || 'No seleccionado';

    console.log('Cambiando al paso 2');

    // Cambiar de paso
    document.getElementById('step-1').classList.remove('active');
    document.getElementById('step-2').classList.add('active');
    document.getElementById('step-indicator-1').classList.remove('active');
    document.getElementById('step-indicator-1').classList.add('completed');
    document.getElementById('step-indicator-2').classList.add('active');

    // Establecer fecha minima (hoy) y registrar evento
    var today = new Date();
    var yyyy = today.getFullYear();
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var dd = String(today.getDate()).padStart(2, '0');
    var todayStr = yyyy + '-' + mm + '-' + dd;

    var fechaCitaInput = document.getElementById('fechaCita');
    fechaCitaInput.min = todayStr;
    fechaCitaInput.value = '';
    selectedFecha = null;
    selectedHora = null;
    document.getElementById('horarios-container').style.display = 'none';

    // Registrar evento change en el input de fecha
    fechaCitaInput.removeEventListener('change', onFechaCitaChange);
    fechaCitaInput.addEventListener('change', onFechaCitaChange);

    console.log('Paso 2 activado - fecha min:', todayStr);
}

/**
 * Regresa al paso 1
 */
function onAnteriorPaso2() {
    document.getElementById('step-2').classList.remove('active');
    document.getElementById('step-1').classList.add('active');
    document.getElementById('step-indicator-2').classList.remove('active');
    document.getElementById('step-indicator-1').classList.remove('completed');
    document.getElementById('step-indicator-1').classList.add('active');
}

/**
 * Maneja el cambio de fecha de la cita
 * Hace una consulta AJAX al servidor para obtener los horarios del terapeuta
 */
function onFechaCitaChange() {
    var fechaValue = this.value;
    if (!fechaValue) {
        document.getElementById('horarios-container').style.display = 'none';
        selectedFecha = null;
        selectedHora = null;
        return;
    }

    var partes = fechaValue.split('-');
    var fechaSeleccionada = new Date(parseInt(partes[0]), parseInt(partes[1]) - 1, parseInt(partes[2]));
    selectedFecha = fechaSeleccionada.toLocaleDateString('es-ES');
    selectedHora = null;

    document.getElementById('fecha-seleccionada').textContent = selectedFecha;

    // Consultar horarios del terapeuta al servidor
    consultarHorariosServidor(selectedTerapeutaId, fechaValue);
}

/**
 * Consulta los horarios disponibles desde el servidor via AJAX
 */
function consultarHorariosServidor(idTerapeuta, fecha) {
    var horariosContainer = document.getElementById('horarios-disponibles');
    horariosContainer.innerHTML = '<p class="text-muted">Consultando horarios disponibles...</p>';
    document.getElementById('horarios-container').style.display = 'block';

    var url = '/Paciente/ObtenerHorariosDisponibles?idTerapeuta=' + idTerapeuta + '&fecha=' + fecha;

    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var respuesta = JSON.parse(xhr.responseText);
                if (respuesta.success) {
                    mostrarHorarios(respuesta.horarios, respuesta.mensaje);
                } else {
                    horariosContainer.innerHTML = '<p class="text-muted">' + (respuesta.mensaje || 'Error al consultar horarios') + '</p>';
                }
            } else {
                horariosContainer.innerHTML = '<p class="text-muted">Error de conexion al consultar horarios</p>';
            }
        }
    };
    xhr.send();
}

/**
 * Muestra los horarios disponibles recibidos del servidor
 */
function mostrarHorarios(horarios, mensaje) {
    var horariosContainer = document.getElementById('horarios-disponibles');
    horariosContainer.innerHTML = '';
    document.getElementById('horarios-container').style.display = 'block';

    if (!horarios || horarios.length === 0) {
        horariosContainer.innerHTML = '<p class="text-muted">' + (mensaje || 'No hay horarios disponibles para este dia') + '</p>';
        return;
    }

    horarios.forEach(function(horario) {
        var slot = document.createElement('div');
        slot.className = 'time-slot ' + (horario.disponible ? 'available' : 'occupied');
        slot.textContent = horario.hora;

        if (horario.disponible) {
            slot.addEventListener('click', function () {
                document.querySelectorAll('.time-slot').forEach(function(s) { s.classList.remove('selected'); });
                slot.classList.add('selected');
                selectedHora = horario.hora;
            });
        }

        horariosContainer.appendChild(slot);
    });
}

/**
 * Maneja el envio del formulario de confirmacion
 */
function onSubmitFormulario(e) {
    e.preventDefault();

    if (!selectedFecha || !selectedHora) {
        alert('Por favor seleccione una fecha y hora para la cita');
        return;
    }

    if (!selectedTerapeutaId) {
        alert('No se ha seleccionado un terapeuta');
        return;
    }

    var fechaValue = document.getElementById('fechaCita').value;

    // Deshabilitar boton para evitar doble click
    var btnConfirmar = document.querySelector('#form-paso-2 button[type="submit"]');
    btnConfirmar.disabled = true;
    btnConfirmar.innerHTML = '<i class="lni lni-spinner-arrow"></i> Confirmando...';

    var datos = 'idTerapeuta=' + selectedTerapeutaId + '&fecha=' + fechaValue + '&horaInicio=' + selectedHora;

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Paciente/ConfirmarCita', true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            btnConfirmar.disabled = false;
            btnConfirmar.innerHTML = '<i class="lni lni-checkmark-circle"></i> Confirmar Cita';

            if (xhr.status === 200) {
                var respuesta = JSON.parse(xhr.responseText);
                if (respuesta.success) {
                    alert('Cita agendada exitosamente!\n\n' +
                        'Terapia: ' + selectedTerapia + '\n' +
                        'Categoria: ' + selectedCategoria + '\n' +
                        'Terapeuta: ' + selectedTerapeuta + '\n' +
                        'Fecha: ' + selectedFecha + '\n' +
                        'Hora: ' + selectedHora);

                    // Refrescar horarios para mostrar el slot como ocupado
                    consultarHorariosServidor(selectedTerapeutaId, fechaValue);
                    selectedHora = null;
                } else {
                    alert(respuesta.mensaje || 'Error al confirmar la cita');
                }
            } else {
                alert('Error de conexion al confirmar la cita');
            }
        }
    };
    xhr.send(datos);
}
