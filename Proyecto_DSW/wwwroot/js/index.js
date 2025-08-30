// Cart functionality
const cart = [];
const cartItemsContainer = document.querySelector('.cart-items');
const cartTotalPrice = document.getElementById('cart-total-price');
const cartCount = document.querySelector('.cart-count');
const continueShoppingBtn = document.getElementById('continue-shopping');
const checkoutBtn = document.getElementById('checkout-btn');
const paymentModal = document.getElementById('payment-modal');
const paymentTotal = document.getElementById('payment-total');
const paymentForm = document.getElementById('payment-form');
const subscriptionModal = document.getElementById('subscription-modal');
const subscriptionButtons = document.querySelectorAll('[data-plan]');
const subscribeBtn = document.getElementById('subscribe-btn');
const menuToggle = document.getElementById('menu-toggle');
const navContainer = document.getElementById('nav-container');
const toastContainer = document.getElementById('toast-container');
const cartModal = document.getElementById('cart-modal');
const closeModalButtons = document.querySelectorAll('.close-button');
const cartIcon = document.getElementById('cart-icon');

// Menu toggle for mobile
menuToggle.addEventListener('click', () => {
    navContainer.classList.toggle('active');
});

// Show toast notification
function showToast(message, isError = false) {
    const toast = document.createElement('div');
    toast.className = `toast ${isError ? 'error' : ''}`;
    toast.textContent = message;
    toastContainer.appendChild(toast);

    setTimeout(() => {
        toast.classList.add('show');
    }, 10);

    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            toast.remove();
        }, 300);
    }, 3000);
}

// Add to cart functionality
document.querySelectorAll('.btn-cart').forEach(button => {
    button.addEventListener('click', (e) => {
        const id = e.target.closest('.btn-cart').dataset.id;
        const title = e.target.closest('.btn-cart').dataset.title;
        const price = parseFloat(e.target.closest('.btn-cart').dataset.price);
        const img = e.target.closest('.btn-cart').dataset.img;

        // Check if item already exists in cart
        const existingItem = cart.find(item => item.id === id);

        if (existingItem) {
            existingItem.quantity += 1;
            showToast(`Se añadió otra copia de "${title}" al carrito`);
        } else {
            cart.push({
                id,
                title,
                price,
                img,
                quantity: 1
            });
            showToast(`"${title}" se añadió al carrito`);
        }

        updateCart();
        cartModal.classList.add('active');
    });
});

// PDF download buttons
document.querySelectorAll('.btn-download').forEach(button => {
    button.addEventListener('click', (e) => {
        const title = e.target.closest('.btn-download').dataset.title;
        subscriptionModal.classList.add('active');
        showToast(`Para descargar "${title}", por favor suscríbete a nuestro plan premium`);
    });
});

// Update cart display
function updateCart() {
    cartItemsContainer.innerHTML = '';

    let total = 0;

    if (cart.length === 0) {
        cartItemsContainer.innerHTML = '<p class="empty-cart">Tu carrito está vacío</p>';
        checkoutBtn.disabled = true;
    } else {
        checkoutBtn.disabled = false;

        cart.forEach(item => {
            const itemTotal = item.price * item.quantity;
            total += itemTotal;

            const cartItemElement = document.createElement('div');
            cartItemElement.className = 'cart-item';
            cartItemElement.innerHTML = `
                <div class="cart-item-img">
                    <img src="${item.img}" alt="${item.title}">
                </div>
                <div class="cart-item-details">
                    <h3 class="cart-item-title">${item.title}</h3>
                    <div class="cart-item-price">$${item.price.toFixed(2)}</div>
                    <div class="cart-item-quantity">
                        <button class="quantity-btn minus" data-id="${item.id}">-</button>
                        <span>${item.quantity}</span>
                        <button class="quantity-btn plus" data-id="${item.id}">+</button>
                    </div>
                </div>
                <button class="remove-item" data-id="${item.id}">
                    <i class="fas fa-trash"></i>
                </button>
            `;

            cartItemsContainer.appendChild(cartItemElement);
        });
    }

    cartTotalPrice.textContent = `$${total.toFixed(2)}`;
    paymentTotal.textContent = `$${total.toFixed(2)}`;

    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    cartCount.textContent = totalItems;

    // Delegated event handling for quantity buttons and remove buttons
    cartItemsContainer.addEventListener('click', (e) => {
        if (e.target.classList.contains('remove-item')) {
            const id = e.target.closest('.remove-item').dataset.id;
            const itemIndex = cart.findIndex(item => item.id === id);
            if (itemIndex !== -1) {
                const removedItem = cart.splice(itemIndex, 1)[0];
                showToast(`"${removedItem.title}" se eliminó del carrito`);
                updateCart();
            }
        } else if (e.target.classList.contains('quantity-btn')) {
            const id = e.target.dataset.id;
            const item = cart.find(item => item.id === id);
            if (e.target.classList.contains('minus') && item.quantity > 1) {
                item.quantity -= 1;
                updateCart();
            } else if (e.target.classList.contains('plus')) {
                item.quantity += 1;
                updateCart();
            }
        }
    });
}

// Open cart modal
cartIcon.addEventListener('click', () => {
    cartModal.classList.add('active');
});

// Open subscription modal
subscribeBtn.addEventListener('click', (e) => {
    e.preventDefault();
    subscriptionModal.classList.add('active');
});

// Close modal functionality
closeModalButtons.forEach(button => {
    button.addEventListener('click', () => {
        cartModal.classList.remove('active');
        paymentModal.classList.remove('active');
        subscriptionModal.classList.remove('active');
    });
});

// Continue shopping
continueShoppingBtn.addEventListener('click', () => {
    cartModal.classList.remove('active');
});

// Proceed to checkout
checkoutBtn.addEventListener('click', () => {
    if (cart.length > 0) {
        cartModal.classList.remove('active');
        paymentModal.classList.add('active');
    } else {
        showToast('Tu carrito está vacío. Agrega algunos libros primero.', true);
    }
});

// Subscription buttons
document.querySelectorAll('[data-plan]').forEach(button => {
    button.addEventListener('click', (e) => {
        const plan = e.target.dataset.plan;
        subscriptionModal.classList.remove('active');
        showToast(`¡Gracias por seleccionar el plan ${plan.toUpperCase()}! Serás redirigido a la página de pago.`);
    });
});

// Payment form submission
paymentForm.addEventListener('submit', (e) => {
    e.preventDefault();

    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const card = document.getElementById('card-number').value;

    if (!name || !email || !card) {
        showToast('Por favor completa todos los campos', true);
        return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        showToast('Por favor ingresa un email válido', true);
        return;
    }

    if (card.replace(/\s/g, '').length !== 16) {
        showToast('El número de tarjeta debe tener 16 dígitos', true);
        return;
    }

    // Simulate payment processing
    showToast('Procesando pago...');

    setTimeout(() => {
        generateReceipt();

        showToast('¡Pago realizado con éxito! Se ha enviado la boleta a tu correo electrónico.');

        cart.length = 0;
        updateCart();
        paymentModal.classList.remove('active');
    }, 2000);
});

// Generate PDF receipt
function generateReceipt() {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();

    doc.setFontSize(22);
    doc.setTextColor(44, 62, 80);
    doc.text('Biblioteca Premium', 105, 20, null, null, 'center');

    doc.setFontSize(16);
    doc.setTextColor(0, 0, 0);
    doc.text('Recibo de Compra', 105, 35, null, null, 'center');

    const now = new Date();
    const dateStr = now.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' });
    const timeStr = now.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' });
    doc.setFontSize(12);
    doc.text(`Fecha: ${dateStr} ${timeStr}`, 105, 45, null, null, 'center');

    doc.setFontSize(12);
    doc.text(`Cliente: ${document.getElementById('name').value}`, 20, 60);
    doc.text(`Email: ${document.getElementById('email').value}`, 20, 70);

    let y = 110;
    doc.setFontSize(12);
    doc.setTextColor(0, 0, 0);

    doc.setFillColor(44, 62, 80);
    doc.setTextColor(255, 255, 255);
    doc.rect(20, y, 170, 10, 'F');
    doc.text('Producto', 25, y + 7);
    doc.text('Precio', 120, y + 7);
    doc.text('Cantidad', 150, y + 7);
    doc.text('Total', 180, y + 7);

    doc.setTextColor(0, 0, 0);
    y += 15;

    cart.forEach(item => {
        const itemTotal = item.price * item.quantity;
        doc.text(item.title, 25, y);
        doc.text(`$${item.price.toFixed(2)}`, 120, y);
        doc.text(item.quantity.toString(), 150, y);
        doc.text(`$${itemTotal.toFixed(2)}`, 180, y);
        y += 10;
    });

    const total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    y += 10;
    doc.setFontSize(14);
    doc.setTextColor(231, 76, 60);
    doc.text(`Total: $${total.toFixed(2)}`, 180, y, null, null, 'right');

    doc.setFontSize(10);
    doc.setTextColor(100, 100, 100);
    doc.text('Gracias por su compra - Biblioteca Premium', 105, 280, null, null, 'center');

    doc.save(`recibo-biblioteca-${Date.now()}.pdf`);
}

// Close modals when clicking outside
window.addEventListener('click', (e) => {
    if (e.target === cartModal) cartModal.classList.remove('active');
    if (e.target === paymentModal) paymentModal.classList.remove('active');
    if (e.target === subscriptionModal) subscriptionModal.classList.remove('active');
});

// Initialize cart
updateCart();
