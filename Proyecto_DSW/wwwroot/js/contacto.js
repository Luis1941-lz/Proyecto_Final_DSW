// ~/wwwroot/js/contacto.js

document.addEventListener("DOMContentLoaded", function () {
    const contactoForm = document.getElementById('contactoForm');
    const btnEnviar = document.getElementById('btnEnviar');
    const alertSuccess = document.getElementById('alertSuccess');
    const alertError = document.getElementById('alertError');

    // Inicializar tooltips si es necesario
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Validación del formulario
    if (contactoForm) {
        contactoForm.addEventListener('submit', function (e) {
            e.preventDefault();

            if (!contactoForm.checkValidity()) {
                e.stopPropagation();
                contactoForm.classList.add('was-validated');
                return;
            }

            // Mostrar estado de carga
            btnEnviar.classList.add('loading');

            // Simular envío (reemplazar con tu lógica real)
            setTimeout(() => {
                enviarFormulario();
            }, 2000);
        });
    }

    function enviarFormulario() {
        const formData = new FormData(contactoForm);

        // Aquí iría tu lógica de envío real (fetch, AJAX, etc.)
        fetch('/Contacto/Enviar', {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => {
                if (!response.ok) throw new Error('Error en el servidor');
                return response.json();
            })
            .then(data => {
                mostrarMensajeExito();
                contactoForm.reset();
                contactoForm.classList.remove('was-validated');
            })
            .catch(error => {
                mostrarMensajeError();
                console.error('Error:', error);
            })
            .finally(() => {
                btnEnviar.classList.remove('loading');
            });
    }

    function mostrarMensajeExito() {
        // Crear o mostrar alerta de éxito
        if (!alertSuccess) {
            const alert = document.createElement('div');
            alert.className = 'alert alert-success alert-dismissible fade show';
            alert.innerHTML = `
                <strong>¡Éxito!</strong> Tu mensaje ha sido enviado correctamente.
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;
            contactoForm.parentNode.insertBefore(alert, contactoForm);
        } else {
            alertSuccess.classList.remove('d-none');
        }
    }

    function mostrarMensajeError() {
        // Crear o mostrar alerta de error
        if (!alertError) {
            const alert = document.createElement('div');
            alert.className = 'alert alert-danger alert-dismissible fade show';
            alert.innerHTML = `
                <strong>Error:</strong> No se pudo enviar el mensaje. Intenta nuevamente.
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;
            contactoForm.parentNode.insertBefore(alert, contactoForm);
        } else {
            alertError.classList.remove('d-none');
        }
    }

    // Animación para los elementos de información
    const infoItems = document.querySelectorAll('.info-item');
    infoItems.forEach((item, index) => {
        item.style.animationDelay = `${index * 0.1}s`;
    });

    // Efecto de parallax para el fondo
    window.addEventListener('scroll', function () {
        const scrolled = window.pageYOffset;
        const parallax = document.querySelector('.contacto-container::before');
        if (parallax) {
            parallax.style.transform = `rotate(${scrolled * 0.1}deg)`;
        }
    });

    // Validación en tiempo real
    const inputs = contactoForm.querySelectorAll('input, textarea, select');
    inputs.forEach(input => {
        input.addEventListener('input', function () {
            if (this.checkValidity()) {
                this.classList.remove('is-invalid');
                this.classList.add('is-valid');
            } else {
                this.classList.remove('is-valid');
                this.classList.add('is-invalid');
            }
        });
    });
});

// Función para copiar información de contacto
function copiarAlPortapapeles(texto, elemento) {
    navigator.clipboard.writeText(texto).then(() => {
        // Mostrar tooltip de confirmación
        const tooltip = new bootstrap.Tooltip(elemento, {
            title: '¡Copiado!',
            trigger: 'manual'
        });
        tooltip.show();

        setTimeout(() => {
            tooltip.hide();
        }, 2000);
    }).catch(err => {
        console.error('Error al copiar: ', err);
    });
}