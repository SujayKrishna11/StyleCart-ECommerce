import { useEffect, useState } from "react";
import { getProductVariants } from "../api/styleCartApi";
import type { CatalogProduct, ProductVariant } from "../types/models";

type ProductDetailsPageProps = {
    product: CatalogProduct;
    token: string;
    onBack: () => void;
    onAddToCart: (variantId: number, quantity: number) => Promise<void>;
};

function ProductDetailsPage({
    product,
    token,
    onBack,
    onAddToCart,
}: ProductDetailsPageProps) {
    const [variants, setVariants] = useState<ProductVariant[]>([]);
    const [selectedVariantId, setSelectedVariantId] =
        useState<number | null>(null);
    const [quantity, setQuantity] = useState(1);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [adding, setAdding] = useState(false);

    useEffect(() => {
        loadVariants();
    }, [product.id]);

    async function loadVariants() {
        try {
            setLoading(true);
            setError("");

            const variantData = await getProductVariants(product.id, token);

            const purchasableVariants = variantData.filter(
                (variant) => variant.isActive && variant.isInStock,
            );

            setVariants(purchasableVariants);
            setSelectedVariantId(purchasableVariants[0]?.id ?? null);
        } catch (requestError) {
            if (requestError instanceof Error) {
                setError(requestError.message);
            } else {
                setError("Could not load product options.");
            }
        } finally {
            setLoading(false);
        }
    }

    const selectedVariant = variants.find(
        (variant) => variant.id === selectedVariantId,
    );

    async function handleAddToCart() {
        if (!selectedVariantId) {
            return;
        }

        try {
            setAdding(true);
            await onAddToCart(selectedVariantId, quantity);
        } finally {
            setAdding(false);
        }
    }

    return (
        <section className= "product-details-page" >
        <button
                className="back-button"
    type = "button"
    onClick = { onBack }
        >
                ← Back to products
        </button>

        < div className = "product-details-layout" >
            <div className="detail-image-panel" >
                {
                    product.imageUrl ? (
                        <img
                            className= "detail-product-image"
                            src={ product.imageUrl }
                            alt={ product.name }
                    />
                    ) : (
                        <div className="detail-image-placeholder" >
                        { product.brand || "StyleCart" }
                    </div>
                    )
}
</div>

    < div className = "detail-content" >
        <p className="eyebrow" >
        { product.brand || "StyleCart" }
            </p>

            < h1 > { product.name } </h1>

            < p className = "detail-description" >
            {
                product.description ||
                    "No description available."
            }
                </p>

                < p className = "detail-price" >
                        ₹{ selectedVariant?.price ?? product.basePrice }
</p>

{
    product.isInStock ? (
        <p className= "in-stock-message" > In stock </p>
                    ) : (
        <p className= "error-message" > Out of stock </p>
                    )
}

{ loading && <p>Loading available variants...</p> }

{
    error && (
        <p className="error-message" > { error } </p>
                    )
}

{
    !loading && !error && product.isInStock && (
        <>
        <label className="detail-label" >
            Variant

            < select
    value = { selectedVariantId ?? ""
}
onChange = {(event) =>
setSelectedVariantId(
    Number(event.target.value),
)
                                    }
                                >
{
    variants.map((variant) => (
        <option
                                            key= { variant.id }
                                            value = { variant.id }
        >
        { variant.color } · { variant.size }
                                            { " — "}₹{ variant.price }
    </option>
    ))
}
    </select>
    </label>

    < label className = "detail-label" >
        Quantity

        < select
value = { quantity }
onChange = {(event) =>
setQuantity(
    Number(event.target.value),
)
                                    }
                                >
{
    Array.from(
        { length: 10 },
        (_, index) => index + 1,
    ).map((amount) => (
        <option key= { amount } value = { amount } >
        { amount }
        </option>
    ))
}
    </select>
    </label>

    < button
className = "detail-add-button"
type = "button"
disabled = {!selectedVariantId || adding}
onClick = { handleAddToCart }
    >
{ adding? "Adding...": "Add to cart" }
    </button>
    </>
                    )}
</div>
    </div>
    </section>
    );
}

export default ProductDetailsPage;
