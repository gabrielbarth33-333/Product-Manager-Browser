const API_URL = 'https://product-manager-api-ekbrbagabcfbawg9.westus3-01.azurewebsites.net/api/products';

const elements = {
    loading: document.getElementById('loading'),
    error: document.getElementById('error'),
    success: document.getElementById('success'),
    emptyState: document.getElementById('empty-state'),
    productsBody: document.getElementById('products-body'),
    productsTable: document.getElementById('products-table'),
    btnNewProduct: document.getElementById('btn-new-product'),
    modal: document.getElementById('modal'),
    btnCloseModal: document.getElementById('btn-close-modal'),
    btnCancel: document.getElementById('btn-cancel'),
    productForm: document.getElementById('product-form')
};

document.addEventListener('DOMContentLoaded', () => {
    loadProducts();
    setupEventListeners();
});

function setupEventListeners() {
    elements.btnNewProduct.addEventListener('click', openModal);
    elements.btnCloseModal.addEventListener('click', closeModal);
    elements.btnCancel.addEventListener('click', closeModal);
    elements.modal.addEventListener('click', (e) => {
        if (e.target === elements.modal) closeModal();
    });
    elements.productForm.addEventListener('submit', handleSubmit);
}

async function loadProducts() {
    showLoading(true);
    hideMessages();

    try {
        const response = await fetch(API_URL);

        if (!response.ok) {
            const problem = await response.json().catch(() => ({}));
            throw new Error(problem.detail || `Erro ${response.status}`);
        }

        const result = await response.json();
        renderProducts(result.items || []);
    } catch (error) {
        showError(`Erro ao carregar produtos: ${error.message}`);
    } finally {
        showLoading(false);
    }
}

function renderProducts(products) {
    elements.productsBody.innerHTML = '';

    if (products.length === 0) {
        elements.productsTable.classList.add('hidden');
        elements.emptyState.classList.remove('hidden');
        return;
    }

    elements.productsTable.classList.remove('hidden');
    elements.emptyState.classList.add('hidden');

    products.forEach(product => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${escapeHtml(product.code)}</td>
            <td>${escapeHtml(product.name)}</td>
            <td>${escapeHtml(product.ean || '-')}</td>
            <td>${formatCurrency(product.price)}</td>
            <td>${formatCurrency(product.promotionalPrice)}</td>
            <td>
                <button class="btn btn-danger" onclick="deleteProduct('${product.id}', '${escapeHtml(product.name)}')">
                    Excluir
                </button>
            </td>
        `;
        elements.productsBody.appendChild(row);
    });
}

async function handleSubmit(event) {
    event.preventDefault();
    hideMessages();

    const product = {
        name: document.getElementById('name').value.trim(),
        code: document.getElementById('code').value.trim(),
        ean: document.getElementById('ean').value.trim(),
        price: parseFloat(document.getElementById('price').value),
        promotionalPrice: parseFloat(document.getElementById('promotionalPrice').value)
    };

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(product)
        });

        if (!response.ok) {
            const problem = await response.json().catch(() => ({}));
            throw new Error(problem.detail || `Erro ${response.status}`);
        }

        closeModal();
        elements.productForm.reset();
        showSuccess('Produto criado com sucesso!');
        await loadProducts();
    } catch (error) {
        showError(`Erro ao criar produto: ${error.message}`);
    }
}

async function deleteProduct(id, name) {
    if (!confirm(`Deseja realmente excluir o produto "${name}"?`)) {
        return;
    }

    hideMessages();

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            const problem = await response.json().catch(() => ({}));
            throw new Error(problem.detail || `Erro ${response.status}`);
        }

        showSuccess('Produto excluído com sucesso!');
        await loadProducts();
    } catch (error) {
        showError(`Erro ao excluir produto: ${error.message}`);
    }
}

function openModal() {
    elements.modal.classList.remove('hidden');
    document.getElementById('name').focus();
}

function closeModal() {
    elements.modal.classList.add('hidden');
    elements.productForm.reset();
}

function showLoading(show) {
    elements.loading.classList.toggle('hidden', !show);
}

function showError(message) {
    elements.error.textContent = message;
    elements.error.classList.remove('hidden');
}

function showSuccess(message) {
    elements.success.textContent = message;
    elements.success.classList.remove('hidden');
}

function hideMessages() {
    elements.error.classList.add('hidden');
    elements.success.classList.add('hidden');
}

function formatCurrency(value) {
    if (value === null || value === undefined) return '-';
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
}

function escapeHtml(text) {
    if (text === null || text === undefined) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
