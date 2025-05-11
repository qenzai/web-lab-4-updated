let cart = JSON.parse(localStorage.getItem("cart")) || [];

function updateCartCount() {
    const cartCount = document.getElementById("cart-count");
    if (cartCount) {
        cartCount.textContent = cart.reduce((sum, item) => sum + item.quantity, 0);
    }
}

function saveCart() {
    localStorage.setItem("cart", JSON.stringify(cart));
    updateCartCount();
}

function displayCart() {
    const cartContainer = document.getElementById("cart-items");
    if (!cartContainer) return;

    cartContainer.innerHTML = "";

    if (cart.length === 0) {
        cartContainer.innerHTML = "<p>Ваш кошик порожній</p>";
        return;
    }

    cart.forEach((item, index) => {
        const itemElement = document.createElement("div");
        itemElement.classList.add("cart-item");
        itemElement.innerHTML = `
            <img src="${item.img}" alt="${item.name}">
            <div>
                <h3>${item.name}</h3>
                <p>${item.desc || ""}</p>
                <span>Кількість: ${item.quantity}</span>
                <button onclick="removeFromCart(${index})">Видалити</button>
            </div>
        `;
        cartContainer.appendChild(itemElement);
    });
}

function removeFromCart(index) {
    cart.splice(index, 1);
    saveCart();
    displayCart();
}

function clearCart() {
    cart = [];
    saveCart();
    displayCart();
}

let products = {};

async function fetchProductsFromAPI() {
    const res = await fetch("http://localhost:5174/api/products/grouped");
    const data = await res.json();
    
    console.log("Дані з API:", data); 
    

    products = {};
    data.forEach(group => {
        products[group.category] = group.products.map(p => ({
            name: p.name,
            desc: p.desc,
            img: p.img,
            price: p.price
        }));
    });
}

function setupMainPage() {
    const categoryItems = document.querySelectorAll(".sidebar ul li");
    const productsContainer = document.querySelector(".products");
    

    function updateProducts(category) {
        productsContainer.innerHTML = "";

        if (!products[category]) {
            productsContainer.innerHTML = "<p>Немає товарів у цій категорії.</p>";
            return;
        }

        products[category].forEach(product => {
            const productCard = document.createElement("div");
            productCard.classList.add("product-card");
            productCard.innerHTML = `
                <img src="${product.img}" alt="${product.name}">
                <h3>${product.name}</h3>
                <p>${product.desc}</p>
                <p><strong>${product.price} $</strong></p>
                <button class="add-to-cart" data-name="${product.name}" data-price="${product.price}" data-img="${product.img}" data-desc="${product.desc}">Додати в кошик</button>
            `;

            productsContainer.appendChild(productCard);
        });

        document.querySelectorAll(".add-to-cart").forEach(button => {
            button.addEventListener("click", function () {
                const product = {
                    name: this.dataset.name,
                    price: parseInt(this.dataset.price),
                    img: this.dataset.img,
                    desc: this.dataset.desc
                };

                const existing = cart.find(item => item.name === product.name);
                if (existing) {
                    existing.quantity += 1;
                } else {
                    cart.push({ ...product, quantity: 1 });
                }

                saveCart();
            });
        });
    }

    categoryItems.forEach(item => {
        item.addEventListener("click", function () {
            updateProducts(this.textContent);
        });
    });
}

document.addEventListener("DOMContentLoaded", async function () {
    updateCartCount();

    if (window.location.pathname.includes("index.html") || window.location.pathname.endsWith("/")) {
        await fetchProductsFromAPI();
        setupMainPage();
    }

    if (window.location.pathname.includes("cart.html")) {
        displayCart();
    }

    let chartInstance = null;

    function generateChartData() {
        const productCounts = {};
    
        cart.forEach(item => {
            if (!productCounts[item.name]) {
                productCounts[item.name] = item.quantity;
            } else {
                productCounts[item.name] += item.quantity;
            }
        });
    
        return {
            labels: Object.keys(productCounts),
            datasets: [{
                label: 'Кількість товарів',
                data: Object.values(productCounts),
                backgroundColor: [
                    '#ff6384', '#36a2eb', '#cc65fe', '#ffce56',
                    '#8dd1e1', '#fdb462', '#fb8072', '#80b1d3'
                ],
                borderWidth: 1
            }]
        };
    }
    
    function updateChart(type) {
        const ctx = document.getElementById('productChart').getContext('2d');
    
        if (chartInstance) {
            chartInstance.destroy();
        }
    
        chartInstance = new Chart(ctx, {
            type: type,
            data: generateChartData(),
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: type === 'pie'
                    }
                },
                scales: type === 'pie' ? {} : {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
    
    document.getElementById("chartType").addEventListener("change", function () {
        updateChart(this.value);
    });
    
    function refreshChart() {
        const selectedType = document.getElementById("chartType").value;
        updateChart(selectedType);
    }
    refreshChart();
});
