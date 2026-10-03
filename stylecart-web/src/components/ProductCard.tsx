import type { CatalogProduct } from "../types/models";

type ProductCardProps = {
    product: CatalogProduct;
    onAddToCart: (product: CatalogProduct) => void;
    onViewDetails: (product: CatalogProduct) => void;
};

function ProductCard({
    product,
    onAddToCart,
    onViewDetails,
}: ProductCardProps) {
    return (
        <article className= "product-card" >
        {
            product.imageUrl ? (
                <img
                    className= "product-image"
                    src={ product.imageUrl }
                    alt={ product.name }
            />
            ) : (
                <div className="product-placeholder" >
                { product.brand || "StyleCart" }
            </div>
            )
}

<div className="product-details" >
    <div className="product-card-top" >
        <p className="product-brand" >
        { product.brand || "StyleCart" }
            </p>

{
    !product.isInStock && (
        <span className="stock-badge" > Out of stock </span>
                    )
}
</div>

    < h2 > { product.name } </h2>

    < p className = "product-description" >
    { product.description || "No description available." }
        </p>

        < div className = "product-footer" >
            <strong>₹{ product.basePrice } </strong>

                < div className = "product-card-actions" >
                    <button
                            className="view-details-button"
type = "button"
onClick = {() => onViewDetails(product)}
                        >
    View details
        </button>

        < button
type = "button"
disabled = {!product.isInStock}
onClick = {() => onAddToCart(product)}
                        >
{
    product.isInStock
        ? "Add to cart"
        : "Out of stock"
}
    </button>
    </div>
    </div>
    </div>
    </article>
    );
}

export default ProductCard;