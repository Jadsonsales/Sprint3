const API_URL = "/api";

document.addEventListener("DOMContentLoaded", () => {
    const token = localStorage.getItem("jwt_token");
    if (token) {
        mostrarPainel();
    }
});

function alternarAba(aba) {
    document.getElementById("auth-msg").textContent = "";
    if (aba === "login") {
        document.getElementById("form-login").classList.remove("hidden");
        document.getElementById("form-registro").classList.add("hidden");
        document.getElementById("tab-login").classList.add("active");
        document.getElementById("tab-registro").classList.remove("active");
    } else {
        document.getElementById("form-login").classList.add("hidden");
        document.getElementById("form-registro").classList.remove("hidden");
        document.getElementById("tab-login").classList.remove("active");
        document.getElementById("tab-registro").classList.add("active");
    }
}

function obterUsuarioDoToken(token) {
    try {
        const payloadBase64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
        const payloadJson = JSON.parse(atob(payloadBase64));
        return (
            payloadJson["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
            payloadJson["unique_name"] ||
            payloadJson["name"] ||
            payloadJson["email"] ||
            payloadJson["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ||
            localStorage.getItem("user_nome") ||
            localStorage.getItem("user_email") ||
            "Usuário"
        );
    } catch {
        return localStorage.getItem("user_nome") || localStorage.getItem("user_email") || "Usuário";
    }
}

async function fazerRegistro(event) {
    event.preventDefault();
    const msg = document.getElementById("auth-msg");
    const nomeDigitado = document.getElementById("reg-nome").value.trim();
    const emailDigitado = document.getElementById("reg-email").value.trim();

    const payload = {
        nome: nomeDigitado,
        email: emailDigitado,
        senha: document.getElementById("reg-senha").value
    };

    try {
        const res = await fetch(`${API_URL}/Auth/registrar`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const texto = await res.text();
        const data = texto ? JSON.parse(texto) : {};
        if (!res.ok) throw new Error(data.mensagem || data.message || "Erro ao registrar.");

        localStorage.setItem("user_nome", nomeDigitado);
        msg.style.color = "green";
        msg.textContent = "Conta criada com sucesso! Faça login agora.";
        document.getElementById("form-registro").reset();

        setTimeout(() => {
            alternarAba("login");
            document.getElementById("login-email").value = emailDigitado;
        }, 1200);
    } catch (err) {
        msg.style.color = "#dc2626";
        msg.textContent = err.message;
    }
}

async function fazerLogin(event) {
    event.preventDefault();
    const msg = document.getElementById("auth-msg");
    const emailInput = document.getElementById("login-email");
    const emailDigitado = emailInput.value.trim();
    const senhaDigitada = document.getElementById("login-senha").value;

    msg.textContent = "";

    const emailValido = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailDigitado);
    if (!emailValido) {
        msg.style.color = "#dc2626";
        msg.textContent = "Digite um endereço de e-mail válido.";
        emailInput.focus();
        return;
    }

    try {
        const res = await fetch(`${API_URL}/Auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                email: emailDigitado,
                senha: senhaDigitada
            })
        });

        const texto = await res.text();
        let data = {};
        try {
            data = texto ? JSON.parse(texto) : {};
        } catch {
            data = { mensagem: texto };
        }

        if (!res.ok) {
            const erroApi = data.mensagem || data.message || data.title;
            throw new Error(erroApi || "E-mail não cadastrado ou senha incorreta.");
        }

        const tokenRecebido = data.token || data.Token || data.accessToken;
        if (!tokenRecebido) {
            throw new Error("A API não retornou o token de acesso.");
        }

        localStorage.setItem("jwt_token", tokenRecebido);
        localStorage.setItem("user_email", emailDigitado);
        document.getElementById("form-login").reset();
        mostrarPainel();
    } catch (err) {
        msg.style.color = "#dc2626";
        msg.textContent = err.message === "Failed to fetch"
            ? "Não foi possível conectar ao servidor."
            : err.message;
    }
}

function logout() {
    localStorage.removeItem("jwt_token");
    localStorage.removeItem("user_email");
    localStorage.removeItem("user_nome");

    const userInfoEl = document.getElementById("user-info");
    const btnLogoutEl = document.getElementById("btn-logout");

    if (userInfoEl) userInfoEl.classList.add("hidden");
    if (btnLogoutEl) btnLogoutEl.classList.add("hidden");

    document.getElementById("auth-section").classList.remove("hidden");
    document.getElementById("app-section").classList.add("hidden");
}

function mostrarPainel() {
    const token = localStorage.getItem("jwt_token");
    const usuarioLogado = obterUsuarioDoToken(token);

    const userNomeEl = document.getElementById("user-nome");
    const userInfoEl = document.getElementById("user-info");
    const btnLogoutEl = document.getElementById("btn-logout");

    if (userNomeEl) userNomeEl.textContent = usuarioLogado;
    if (userInfoEl) userInfoEl.classList.remove("hidden");
    if (btnLogoutEl) btnLogoutEl.classList.remove("hidden");

    document.getElementById("auth-section").classList.add("hidden");
    document.getElementById("app-section").classList.remove("hidden");
    carregarProdutos();
}

function getHeadersAutenticados() {
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${localStorage.getItem("jwt_token")}`
    };
}

async function carregarProdutos() {
    const tbody = document.getElementById("tabela-produtos");
    tbody.innerHTML = "<tr><td colspan='6'>Carregando...</td></tr>";

    try {
        const res = await fetch(`${API_URL}/Produtos`, {
            headers: getHeadersAutenticados()
        });

        if (res.status === 401) {
            logout();
            return;
        }

        const produtos = await res.json();
        tbody.innerHTML = "";

        if (produtos.length === 0) {
            tbody.innerHTML = "<tr><td colspan='6'>Nenhum produto cadastrado.</td></tr>";
            return;
        }

        produtos.forEach(p => {
            const precoFormatado = Number(p.preco).toLocaleString("pt-BR", {
                style: "currency",
                currency: "BRL"
            });

            tbody.innerHTML += `
                <tr>
                    <td>${p.id}</td>
                    <td>${p.nome}</td>
                    <td>${p.descricao}</td>
                    <td>${precoFormatado}</td>
                    <td>${p.estoque}</td>
                    <td>
                        <button class="btn-edit" onclick='prepararEdicao(${JSON.stringify(p)})'>Editar</button>
                        <button class="btn-del" onclick="deletarProduto(${p.id})">Excluir</button>
                    </td>
                </tr>
            `;
        });
    } catch (err) {
        tbody.innerHTML = "<tr><td colspan='6'>Erro ao conectar com a API.</td></tr>";
    }
}

async function salvarProduto(event) {
    event.preventDefault();
    const msg = document.getElementById("prod-msg");
    const id = document.getElementById("produto-id").value;

    const payload = {
        nome: document.getElementById("prod-nome").value,
        descricao: document.getElementById("prod-descricao").value,
        preco: parseFloat(document.getElementById("prod-preco").value),
        estoque: parseInt(document.getElementById("prod-estoque").value)
    };

    const url = id ? `${API_URL}/Produtos/${id}` : `${API_URL}/Produtos`;
    const method = id ? "PUT" : "POST";

    try {
        const res = await fetch(url, {
            method: method,
            headers: getHeadersAutenticados(),
            body: JSON.stringify(payload)
        });

        if (!res.ok) {
            const errData = await res.json().catch(() => ({}));
            throw new Error(errData.title || errData.mensagem || "Erro ao salvar produto.");
        }

        msg.style.color = "green";
        msg.textContent = id ? "Produto atualizado com sucesso!" : "Produto cadastrado com sucesso!";
        cancelarEdicao();
        carregarProdutos();
    } catch (err) {
        msg.style.color = "red";
        msg.textContent = err.message;
    }
}

function prepararEdicao(produto) {
    document.getElementById("form-produto-titulo").textContent = `Editando Produto #${produto.id}`;
    document.getElementById("produto-id").value = produto.id;
    document.getElementById("prod-nome").value = produto.nome;
    document.getElementById("prod-descricao").value = produto.descricao;
    document.getElementById("prod-preco").value = produto.preco;
    document.getElementById("prod-estoque").value = produto.estoque;
    document.getElementById("btn-salvar-produto").textContent = "Atualizar Produto";
    document.getElementById("btn-cancelar-edicao").classList.remove("hidden");
}

function cancelarEdicao() {
    document.getElementById("form-produto-titulo").textContent = "Cadastrar Novo Produto";
    document.getElementById("form-produto").reset();
    document.getElementById("produto-id").value = "";
    document.getElementById("btn-salvar-produto").textContent = "Salvar Produto";
    document.getElementById("btn-cancelar-edicao").classList.add("hidden");
}

async function deletarProduto(id) {
    if (!confirm(`Deseja realmente excluir o produto #${id}?`)) return;

    try {
        const res = await fetch(`${API_URL}/Produtos/${id}`, {
            method: "DELETE",
            headers: getHeadersAutenticados()
        });

        if (!res.ok) throw new Error("Erro ao excluir produto.");
        carregarProdutos();
    } catch (err) {
        alert(err.message);
    }
}

async function baixarPdfEstoque() {
    try {
        const res = await fetch(`${API_URL}/Produtos/relatorio-pdf`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${localStorage.getItem("jwt_token")}`
            }
        });

        if (res.status === 401) {
            logout();
            return;
        }

        if (!res.ok) throw new Error("Erro ao gerar o PDF.");

        const blob = await res.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = "Relatorio_Estoque.pdf";
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
    } catch (err) {
        alert(err.message);
    }
}