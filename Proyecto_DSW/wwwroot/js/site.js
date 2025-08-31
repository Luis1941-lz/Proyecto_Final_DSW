// ~/wwwroot/js/sidebar.js - VERSIÓN SOLO PC

document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const openBtn = document.getElementById("open-sidebar");
    const closeBtn = document.getElementById("close-sidebar");
    const cartCount = document.querySelector(".cart-count");

    // Estado inicial: sidebar visible
    let isSidebarOpen = true;

    console.log("Elementos cargados:", { sidebar, openBtn, closeBtn, cartCount });

    // Función para abrir sidebar
    function openSidebar() {
        sidebar.classList.remove("collapsed");
        if (openBtn) openBtn.classList.add("hidden");
        isSidebarOpen = true;
    }

    // Función para cerrar sidebar
    function closeSidebar() {
        sidebar.classList.add("collapsed");
        if (openBtn) openBtn.classList.remove("hidden");
        isSidebarOpen = false;
    }

    // Event listeners
    if (openBtn) {
        openBtn.addEventListener("click", openSidebar);
    }

    if (closeBtn) {
        closeBtn.addEventListener("click", closeSidebar);
    }

    // Actualizar carrito
    function actualizarCarrito() {
        fetch("/Carrito/ObtenerCantidad")
            .then(res => {
                if (!res.ok) throw new Error('Error en la respuesta');
                return res.json();
            })
            .then(data => {
                if (cartCount) {
                    cartCount.textContent = data;
                    // Animación cuando cambia el contador
                    if (data > 0) {
                        cartCount.style.transform = 'scale(1.2)';
                        setTimeout(() => {
                            cartCount.style.transform = 'scale(1)';
                        }, 300);
                    }
                }
            })
            .catch(err => console.error("Error al cargar carrito:", err));
    }

    // Inicializar
    actualizarCarrito();

    // Opcional: Actualizar carrito cada 30 segundos
    setInterval(actualizarCarrito, 30000);

    // Exportar funciones para uso global
    window.sidebarManager = {
        open: openSidebar,
        close: closeSidebar,
        isOpen: () => isSidebarOpen,
        updateCart: actualizarCarrito
    };
});