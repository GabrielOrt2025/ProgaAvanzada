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
let selectedFecha = null;
let selectedHora = null;
let currentDate = new Date();

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
    // Paso 1: Manejo de selección de terapia
    document.getElementById('tipoTerapia').addEventListener('change', onTerapiaChange);
    document.getElementById('categoriaClinica').addEventListener('change', onCategoriaChange);

    // Navegación entre pasos
    document.getElementById('btn-siguiente-1').addEventListener('click', onSiguientePaso1);
    document.getElementById('btn-anterior-2').addEventListener('click', onAnteriorPaso2);

    // Consultar disponibilidad
    document.getElementById('btn-consultar-disponibilidad').addEventListener('click', onConsultarDisponibilidad);

    // Navegación de meses
    document.getElementById('btn-mes-anterior').addEventListener('click', onMesAnterior);
    document.getElementById('btn-mes-siguiente').addEventListener('click', onMesSiguiente);

    // Envío del formulario
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

    // Establecer fechas por defecto
    const today = new Date();
    const nextWeek = new Date(today);
    nextWeek.setDate(nextWeek.getDate() + 7);

    document.getElementById('fechaInicio').value = today.toISOString().split('T')[0];
    document.getElementById('fechaFin').value = nextWeek.toISOString().split('T')[0];

    console.log('Paso 2 activado');
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
 * Consulta la disponibilidad y muestra el calendario
 */
function onConsultarDisponibilidad() {
    const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin').value;

    if (!fechaInicio || !fechaFin) {
        alert('Por favor seleccione el rango de fechas');
        return;
    }

    document.getElementById('calendario-container').style.display = 'block';
    generarCalendario(new Date(fechaInicio));
}

/**
 * Genera el calendario para el mes especificado
 */
function generarCalendario(fecha) {
    const diasContainer = document.getElementById('calendario-dias');
    diasContainer.innerHTML = '';

    const año = fecha.getFullYear();
    const mes = fecha.getMonth();

    const meses = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
    document.getElementById('calendario-mes-anio').textContent = `${meses[mes]} ${año}`;

    const primerDia = new Date(año, mes, 1).getDay();
    const ultimoDia = new Date(año, mes + 1, 0).getDate();

    // Días vacíos al inicio
    for (let i = 0; i < primerDia; i++) {
        const diaVacio = document.createElement('div');
        diaVacio.className = 'calendar-day disabled';
        diasContainer.appendChild(diaVacio);
    }

    // Días del mes
    for (let dia = 1; dia <= ultimoDia; dia++) {
        const diaElement = document.createElement('div');
        diaElement.className = 'calendar-day';
        diaElement.innerHTML = `<strong>${dia}</strong>`;

        const fechaDia = new Date(año, mes, dia);
        const hoy = new Date();
        hoy.setHours(0, 0, 0, 0);

        // Deshabilitar días pasados
        if (fechaDia < hoy) {
            diaElement.classList.add('disabled');
        } else {
            diaElement.addEventListener('click', function () {
                if (!diaElement.classList.contains('disabled')) {
                    document.querySelectorAll('.calendar-day').forEach(d => d.classList.remove('selected'));
                    diaElement.classList.add('selected');
                    selectedFecha = fechaDia.toLocaleDateString('es-ES');
                    document.getElementById('fecha-seleccionada').textContent = selectedFecha;
                    mostrarHorarios(fechaDia);
                }
            });
        }

        diasContainer.appendChild(diaElement);
    }
}

/**
 * Muestra los horarios disponibles para una fecha
 */
function mostrarHorarios(fecha) {
    const horariosContainer = document.getElementById('horarios-disponibles');
    horariosContainer.innerHTML = '';
    document.getElementById('horarios-container').style.display = 'block';

    // Horarios de ejemplo (8:00 AM - 5:00 PM)
    const horarios = [
        { hora: '08:00', disponible: true },
        { hora: '09:00', disponible: true },
        { hora: '10:00', disponible: false },
        { hora: '11:00', disponible: true },
        { hora: '12:00', disponible: false },
        { hora: '13:00', disponible: true },
        { hora: '14:00', disponible: true },
        { hora: '15:00', disponible: false },
        { hora: '16:00', disponible: true },
        { hora: '17:00', disponible: true }
    ];

    horarios.forEach(horario => {
        const slot = document.createElement('div');
        slot.className = `time-slot ${horario.disponible ? 'available' : 'occupied'}`;
        slot.textContent = horario.hora;

        if (horario.disponible) {
            slot.addEventListener('click', function () {
                document.querySelectorAll('.time-slot').forEach(s => s.classList.remove('selected'));
                slot.classList.add('selected');
                selectedHora = horario.hora;
            });
        }

        horariosContainer.appendChild(slot);
    });
}

/**
 * Navega al mes anterior
 */
function onMesAnterior() {
    currentDate.setMonth(currentDate.getMonth() - 1);
    generarCalendario(currentDate);
}

/**
 * Navega al mes siguiente
 */
function onMesSiguiente() {
    currentDate.setMonth(currentDate.getMonth() + 1);
    generarCalendario(currentDate);
}

/**
 * Maneja el envío del formulario de confirmación
 */
function onSubmitFormulario(e) {
    e.preventDefault();

    if (!selectedFecha || !selectedHora) {
        alert('Por favor seleccione una fecha y hora para la cita');
        return;
    }

    alert(`Cita agendada exitosamente!\n\nTerapia: ${selectedTerapia}\nCategoría: ${selectedCategoria}\nTerapeuta: ${selectedTerapeuta}\nFecha: ${selectedFecha}\nHora: ${selectedHora}`);

    // Aquí iría la lógica para enviar los datos al servidor
    // window.location.href = urlRedirect;
}
