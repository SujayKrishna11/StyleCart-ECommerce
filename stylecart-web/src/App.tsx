import { useState } from "react";
import "./App.css";

import {
    addItemToCart,
    getCart,
    getProductVariants,
    removeCartItem,
    updateCartItem,
} from "./api/styleCartApi";

import Header from "./components/Header";
import VariantPicker from "./components/VariantPicker";

import CartPage from "./pages/CartPage";
import CheckoutPage from "./pages/CheckoutPage";
import LoginPage from "./pages/LoginPage";
import OrdersPage from "./pages/OrdersPage";
import ProductDetailsPage from "./pages/ProductDetailsPage";
import ProductsPage from "./pages/ProductsPage";
import RegisterPage from "./pages/RegisterPage";

import type {
    Cart,
    CatalogProduct,
    LoginResponse,
    Order,
    ProductVariant,
} from "./types/models";

type Page =
    | "products"
    | "details"
    | "login"
    | "register"
    | "cart"
    | "checkout"
    | "orders";

function App() {
    const [currentPage, setCurrentPage] = useState<Page>("products");

    const [token, setToken] = useState(
        localStorage.getItem("styleCartToken") ?? "",
    );

    const [cart, setCart] = useState<Cart | null>(null);

    const [selectedProduct, setSelectedProduct] =
        useState<CatalogProduct | null>(null);

    const [detailsProduct, setDetailsProduct] =
        useState<CatalogProduct | null>(null);

    const [availableVariants, setAvailableVariants] = useState<
        ProductVariant[]
    >([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const cartItemCount =
        cart?.items.reduce((total, item) => total + item.quantity, 0) ?? 0;

    async function loadCart(activeToken = token) {
        if (!activeToken) {
            return;
        }

        try {
            const cartData = await getCart(activeToken);
            setCart(cartData);
        } catch (requestError) {
            showError(requestError, "Could not load the cart.");
        }
    }

    async function navigateTo(page: Page) {
        setMessage("");
        setError("");

        if (page === "cart" || page === "orders") {
            if (!token) {
                setMessage("Please log in to access this page.");
                setCurrentPage("login");
                return;
            }
        }

        if (page === "cart") {
            await loadCart();
        }

        setCurrentPage(page);
    }

    function handleLogin(response: LoginResponse) {
        localStorage.setItem("styleCartToken", response.token);

        setToken(response.token);
        setMessage(`Welcome, ${response.email}.`);
        setError("");
        setCurrentPage("products");

        loadCart(response.token);
    }

    function handleLogout() {
        localStorage.removeItem("styleCartToken");

        setToken("");
        setCart(null);
        setSelectedProduct(null);
        setDetailsProduct(null);
        setAvailableVariants([]);
        setMessage("You have logged out.");
        setError("");
        setCurrentPage("products");
    }

    function handleViewDetails(product: CatalogProduct) {
        setMessage("");
        setError("");
        setDetailsProduct(product);
        setCurrentPage("details");
    }

    async function handleAddToCart(product: CatalogProduct) {
        if (!product.isInStock) {
            setError(`${product.name} is currently out of stock.`);
            return;
        }

        if (!token) {
            setMessage("Please log in before adding a product to the cart.");
            setCurrentPage("login");
            return;
        }

        try {
            setMessage("");
            setError("");

            const variants = await getProductVariants(product.id, token);

            const purchasableVariants = variants.filter(
                (variant) => variant.isActive && variant.isInStock,
            );

            if (purchasableVariants.length === 0) {
                setError(`${product.name} is currently out of stock.`);
                return;
            }

            setSelectedProduct(product);
            setAvailableVariants(purchasableVariants);
        } catch (requestError) {
            showError(requestError, "Could not load product variants.");
        }
    }

    async function addVariantToCart(
        variantId: number,
        quantity: number,
    ): Promise<boolean> {
        if (!token) {
            setMessage("Please log in before adding a product to the cart.");
            setCurrentPage("login");
            return false;
        }

        try {
            await addItemToCart(variantId, quantity, token);
            await loadCart();

            setMessage(
                quantity === 1
                    ? "Product added to your cart."
                    : `${quantity} items added to your cart.`,
            );

            setError("");
            return true;
        } catch (requestError) {
            showError(requestError, "Could not add this product to the cart.");
            return false;
        }
    }

    async function addSelectedVariantToCart(variantId: number) {
        const added = await addVariantToCart(variantId, 1);

        if (added) {
            setSelectedProduct(null);
            setAvailableVariants([]);
        }
    }

    async function addDetailVariantToCart(
        variantId: number,
        quantity: number,
    ) {
        await addVariantToCart(variantId, quantity);
    }

    async function increaseQuantity(cartItemId: number, quantity: number) {
        try {
            await updateCartItem(cartItemId, quantity + 1, token);
            await loadCart();
        } catch (requestError) {
            showError(requestError, "Could not update the quantity.");
        }
    }

    async function decreaseQuantity(cartItemId: number, quantity: number) {
        if (quantity <= 1) {
            return;
        }

        try {
            await updateCartItem(cartItemId, quantity - 1, token);
            await loadCart();
        } catch (requestError) {
            showError(requestError, "Could not update the quantity.");
        }
    }

    async function deleteCartItem(cartItemId: number) {
        try {
            await removeCartItem(cartItemId, token);
            await loadCart();
            setMessage("Item removed from your cart.");
        } catch (requestError) {
            showError(requestError, "Could not remove the item.");
        }
    }

    function goToCheckout() {
        if (!cart || cart.items.length === 0) {
            setError("Your cart is empty.");
            return;
        }

        setMessage("");
        setError("");
        setCurrentPage("checkout");
    }

    function handleOrderPlaced(order: Order) {
        setCart(null);
        setMessage(`Order #${order.id} was placed successfully.`);
        setError("");
        setCurrentPage("orders");
    }

    function closeVariantPicker() {
        setSelectedProduct(null);
        setAvailableVariants([]);
    }

    function showError(requestError: unknown, fallbackMessage: string) {
        if (requestError instanceof Error) {
            setError(requestError.message);
        } else {
            setError(fallbackMessage);
        }
    }

    return (
        <div className= "app" >
        <Header
                currentPage={ currentPage }
    isLoggedIn = { Boolean(token) }
    cartItemCount = { cartItemCount }
    onNavigate = { navigateTo }
    onLogout = { handleLogout }
        />

        <main className="main-content" >
        { message && (
                <p className="success-message" > { message } </p>
                )
}

{
    error && (
        <p className="error-message" > { error } </p>
                )
}

{
    currentPage === "products" && (
        <ProductsPage
                        onAddToCart={ handleAddToCart }
    onViewDetails = { handleViewDetails }
        />
                )
}

{
    currentPage === "details" && detailsProduct && (
        <ProductDetailsPage
                        product={ detailsProduct }
    token = { token }
    onBack = {() => navigateTo("products")
}
onAddToCart = { addDetailVariantToCart }
    />
                )}

{
    currentPage === "login" && (
        <LoginPage onLogin={ handleLogin } />
                )
}

{
    currentPage === "register" && (
        <RegisterPage onRegister={ handleLogin } />
                )
}

{
    currentPage === "cart" && (
        <CartPage
                        cart={ cart }
    onIncreaseQuantity = { increaseQuantity }
    onDecreaseQuantity = { decreaseQuantity }
    onRemoveItem = { deleteCartItem }
    onCheckout = { goToCheckout }
        />
                )
}

{
    currentPage === "checkout" && (
        <CheckoutPage
                        token={ token }
    onOrderPlaced = { handleOrderPlaced }
        />
                )
}

{
    currentPage === "orders" && (
        <OrdersPage token={ token } />
                )
}
</main>

{
    selectedProduct && (
        <VariantPicker
                    product={ selectedProduct }
    variants = { availableVariants }
    onAddToCart = { addSelectedVariantToCart }
    onClose = { closeVariantPicker }
        />
            )
}
</div>
    );
}

export default App;