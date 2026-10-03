type Page =
    | "products"
    | "details"
    | "login"
    | "register"
    | "cart"
    | "checkout"
    | "orders";

type HeaderProps = {
    currentPage: Page;
    isLoggedIn: boolean;
    cartItemCount: number;
    onNavigate: (page: Page) => void;
    onLogout: () => void;
};

function Header({
    currentPage,
    isLoggedIn,
    cartItemCount,
    onNavigate,
    onLogout,
}: HeaderProps) {
    return (
        <header className= "header" >
        <button
                className="brand-button"
    type = "button"
    onClick = {() => onNavigate("products")
}
            >
    StyleCart
    </button>

    < nav className = "navigation" >
        <button
                    className={
    currentPage === "products" ? "active-link" : ""
}
type = "button"
onClick = {() => onNavigate("products")}
                >
    Products
    </button>

    < button
className = {
    currentPage === "cart" ? "active-link" : ""
                    }
type = "button"
onClick = {() => onNavigate("cart")}
                >
    Cart({ cartItemCount })
    </button>

{
    isLoggedIn ? (
        <>
        <button
                            className= {
            currentPage === "orders"
            ? "active-link"
            : ""
                            }
type = "button"
onClick = {() => onNavigate("orders")}
                        >
    My Orders
        </button>

        < button type = "button" onClick = { onLogout } >
            Logout
            </button>
            </>
                ) : (
    <>
    <button
                            className= {
        currentPage === "login"
        ? "active-link"
        : ""
                            }
type = "button"
onClick = {() => onNavigate("login")}
                        >
    Login
    </button>

    < button
className = {
    currentPage === "register"
    ? "active-link"
    : ""
                            }
type = "button"
onClick = {() => onNavigate("register")}
                        >
    Register
    </button>
    </>
                )}
</nav>
    </header>
    );
}

export default Header;