 // Elementos generales
const userIcon = document.getElementById('user-icon');
const loginModal = document.getElementById('login-modal');
const registerModal = document.getElementById('register-modal');
const openRegisterLink = document.getElementById('open-register');
const openLoginLink = document.getElementById('open-login');
const closeModalButtons = document.querySelectorAll('.close-modal');
const loginForm = document.querySelector('.login-form');
const registerForm = document.querySelector('.register-form');
const userNameDisplay = document.getElementById('user-name-display');
const logoutBtn = document.getElementById('logout-btn');

// Mostrar modal de login
userIcon.addEventListener('click', () => {
    loginModal.classList.add('active');
});

// Cambiar entre modales
openRegisterLink.addEventListener('click', (e) => {
    e.preventDefault();
    loginModal.classList.remove('active');
    registerModal.classList.add('active');
});

openLoginLink.addEventListener('click', (e) => {
    e.preventDefault();
    registerModal.classList.remove('active');
    loginModal.classList.add('active');
});

// Cerrar modales
closeModalButtons.forEach(button => {
    button.addEventListener('click', () => {
        loginModal.classList.remove('active');
        registerModal.classList.remove('active');
    });
});

window.addEventListener('click', (e) => {
    if (e.target === loginModal) loginModal.classList.remove('active');
    if (e.target === registerModal) registerModal.classList.remove('active');
});

// Registrar usuario
registerForm.addEventListener('submit', (e) => {
    e.preventDefault();
    const name = document.getElementById('register-name').value.trim();
    const email = document.getElementById('register-email').value.trim();
    const password = document.getElementById('register-password').value;

    if (!name || !email || !password) {
        alert('Por favor completa todos los campos');
        return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        alert('Por favor ingresa un email válido');
        return;
    }

    if (password.length < 6) {
        alert('La contraseña debe tener al menos 6 caracteres');
        return;
    }

    const user = { name, email, password };
    localStorage.setItem('user', JSON.stringify(user));
    alert('¡Registro exitoso! Ahora puedes iniciar sesión');

    registerModal.classList.remove('active');
    loginModal.classList.add('active');
});

// Iniciar sesión
loginForm.addEventListener('submit', (e) => {
    e.preventDefault();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;

    if (!email || !password) {
        alert('Por favor completa todos los campos');
        return;
    }

    const savedUser = JSON.parse(localStorage.getItem('user'));

    if (!savedUser) {
        alert('No se encontró ninguna cuenta registrada.');
        return;
    }

    if (savedUser.email === email && savedUser.password === password) {
        alert(`¡Inicio de sesión exitoso, bienvenido ${savedUser.name}!`);
        loginModal.classList.remove('active');
        mostrarUsuarioLogueado(savedUser.name);
    } else {
        alert('Email o contraseña incorrectos.');
    }
});

// Mostrar datos del usuario logueado
function mostrarUsuarioLogueado(nombre) {
    userNameDisplay.textContent = `Hola, ${nombre}`;
    userIcon.style.display = 'none';
    logoutBtn.style.display = 'inline-block';
}

// Cerrar sesión
logoutBtn.addEventListener('click', () => {
    userNameDisplay.textContent = '';
    userIcon.style.display = 'inline-block';
    logoutBtn.style.display = 'none';
    alert('Sesión cerrada');
});

// Mostrar usuario si ya ha iniciado sesión antes
document.addEventListener('DOMContentLoaded', () => {
    const savedUser = JSON.parse(localStorage.getItem('user'));
    if (savedUser && savedUser.name && savedUser.email && savedUser.password) {
        // Opción: auto login si quieres que mantenga sesión iniciada
        // mostrarUsuarioLogueado(savedUser.name);
    }
});

// Modal de carrito (si lo usas)
const cartIcon = document.getElementById('cart-icon');
const cartModal = document.getElementById('cart-modal');

if (cartIcon && cartModal) {
    cartIcon.addEventListener('click', () => {
        cartModal.classList.add('active');
    });

    window.addEventListener('click', (e) => {
        if (e.target === cartModal) {
            cartModal.classList.remove('active');
        }
    });
}
