document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const openBtn = document.getElementById("open-sidebar");
    const closeBtn = document.getElementById("close-sidebar");
    const cartCount = document.querySelector(".cart-count");

    console.log("sidebar:", sidebar);
    console.log("openBtn:", openBtn);
    console.log("closeBtn:", closeBtn);

    if (openBtn) {
        openBtn.addEventListener("click", function () {
            sidebar.classList.add("active");
            openBtn.classList.add("hidden");
        });
    }

    if (closeBtn) {
        closeBtn.addEventListener("click", function () {
            sidebar.classList.remove("active");
            openBtn.classList.remove("hidden");
        });
    }

    const sidebarLinks = document.querySelectorAll(".sidebar-links a");
    sidebarLinks.forEach(link => {
        link.addEventListener("click", function (e) {
            sidebarLinks.forEach(l => l.classList.remove("active"));
            this.classList.add("active");
        });
    });

    function actualizarCarrito() {
        fetch("/Carrito/ObtenerCantidad")
            .then(res => res.json())
            .then(data => {
                cartCount.textContent = data;
            })
            .catch(err => console.error("Error al cargar carrito:", err));
    }

    actualizarCarrito();
});
