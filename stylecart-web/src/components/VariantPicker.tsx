import type { Product, ProductVariant } from "../types/models";

type VariantPickerProps = {
    product: Product;
    variants: ProductVariant[];
    onAddToCart: (variantId: number) => void;
    onClose: () => void;
};

function VariantPicker({
    product,
    variants,
    onAddToCart,
    onClose,
}: VariantPickerProps) {
    const purchasableVariants = variants.filter(
        (variant) => variant.isActive && variant.isInStock,
    );

    return (
        <div className="modal-background">
            <section
                className="variant-picker"
                aria-label={`Choose a variant for ${product.name}`}
            >
                <button
                    className="close-button"
                    type="button"
                    onClick={onClose}
                    aria-label="Close variant picker"
                >
                    ×
                </button>

                <p className="eyebrow">Choose a variant</p>
                <h2>{product.name}</h2>

                {purchasableVariants.length === 0 ? (
                    <p className="error-message">
                        This product is currently out of stock.
                    </p>
                ) : (
                    <div className="variant-list">
                        {purchasableVariants.map((variant) => (
                            <button
                                className="variant-button"
                                key={variant.id}
                                type="button"
                                onClick={() => onAddToCart(variant.id)}
                            >
                                <span>
                                    {variant.color} · {variant.size}
                                </span>

                                <strong>₹{variant.price}</strong>
                            </button>
                        ))}
                    </div>
                )}
            </section>
        </div>
    );
}

export default VariantPicker;
