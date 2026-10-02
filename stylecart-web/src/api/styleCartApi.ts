import type {
    Cart,
    Category,
    CheckoutRequest,
    LoginResponse,
    Order,
    Product,
    ProductVariant,
} from "../types/models";

const apiUrl = "https://localhost:65125/api";

async function sendRequest<T>(
    endpoint: string,
    options: RequestInit = {},
    token?: string,
): Promise<T> {
    const headers = new Headers(options.headers);

    if (options.body) {
        headers.set("Content-Type", "application/json");
    }

    if (token) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    try {
        const response = await fetch(`${apiUrl}${endpoint}`, {
            ...options,
            headers,
        });

        if (!response.ok) {
            throw new Error(getUserFriendlyError(endpoint, response.status));
        }

        if (response.status === 204) {
            return undefined as T;
        }

        return response.json() as Promise<T>;
    } catch (error) {
        if (error instanceof Error) {
            throw error;
        }

        throw new Error(
            "Could not connect to the server. Please try again.",
        );
    }
}

function getUserFriendlyError(endpoint: string, status: number) {
    if (status === 401) {
        return "Your session has expired. Please log in again.";
    }

    if (status === 403) {
        return "You do not have permission to perform this action.";
    }

    if (status === 404) {
        return "The requested item could not be found.";
    }

    if (status >= 500) {
        return "Something went wrong on the server. Please try again later.";
    }

    if (endpoint === "/Auth/login") {
        return "Invalid email or password.";
    }

    if (endpoint === "/Auth/register") {
        return "Could not create the account. Check your details and try again.";
    }

    if (endpoint.startsWith("/Cart")) {
        return "Could not update your cart. Please try again.";
    }

    if (endpoint === "/Orders/checkout") {
        return "Could not place the order. Check your cart and available stock.";
    }

    if (endpoint.startsWith("/ProductVariants")) {
        return "Could not load the product options. Please try again.";
    }

    if (endpoint.startsWith("/Products")) {
        return "Could not load products. Please refresh the page.";
    }

    return "Something went wrong. Please try again.";
}

export function getProducts(token?: string) {
    return sendRequest<Product[]>("/Products", {}, token);
}

export function getAvailableProducts() {
    return sendRequest<Product[]>("/Products/available");
}

export function getCategories() {
    return sendRequest<Category[]>("/Categories");
}
export function getProductVariants(productId: number, token: string) {
    return sendRequest<ProductVariant[]>(
        `/ProductVariants?productId=${productId}`,
        {},
        token,
    );
}

export function login(email: string, password: string) {
    return sendRequest<LoginResponse>("/Auth/login", {
        method: "POST",
        body: JSON.stringify({
            email,
            password,
        }),
    });
}

export function register(
    firstName: string,
    lastName: string,
    email: string,
    password: string,
) {
    return sendRequest<LoginResponse>("/Auth/register", {
        method: "POST",
        body: JSON.stringify({
            firstName,
            lastName,
            email,
            password,
        }),
    });
}

export function getCart(token: string) {
    return sendRequest<Cart>("/Cart", {}, token);
}

export function addItemToCart(
    productVariantId: number,
    quantity: number,
    token: string,
) {
    return sendRequest(
        "/Cart/items",
        {
            method: "POST",
            body: JSON.stringify({
                productVariantId,
                quantity,
            }),
        },
        token,
    );
}

export function updateCartItem(
    cartItemId: number,
    quantity: number,
    token: string,
) {
    return sendRequest(
        `/Cart/items/${cartItemId}`,
        {
            method: "PUT",
            body: JSON.stringify({
                quantity,
            }),
        },
        token,
    );
}

export function removeCartItem(cartItemId: number, token: string) {
    return sendRequest(
        `/Cart/items/${cartItemId}`,
        {
            method: "DELETE",
        },
        token,
    );
}

export function checkout(data: CheckoutRequest, token: string) {
    return sendRequest<Order>(
        "/Orders/checkout",
        {
            method: "POST",
            body: JSON.stringify(data),
        },
        token,
    );
}

export function getMyOrders(token: string) {
    return sendRequest<Order[]>("/Orders/my-orders", {}, token);
}