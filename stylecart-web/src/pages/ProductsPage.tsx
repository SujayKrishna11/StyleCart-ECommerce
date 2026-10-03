import { useEffect, useState } from "react";
import {
    getCatalogProducts,
    getCategories,
} from "../api/styleCartApi";
import ProductCard from "../components/ProductCard";

import type {
    CatalogProduct,
    Category,
} from "../types/models";

type ProductsPageProps = {
    onAddToCart: (product: CatalogProduct) => void;
    onViewDetails: (product: CatalogProduct) => void;
};

function ProductsPage({
    onAddToCart,
    onViewDetails,
}: ProductsPageProps) {
    const [products, setProducts] = useState<CatalogProduct[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [selectedCategoryId, setSelectedCategoryId] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        loadPageData();
    }, []);

    async function loadPageData() {
        try {
            setLoading(true);
            setError("");

            const [productData, categoryData] = await Promise.all([
                getCatalogProducts(),
                getCategories(),
            ]);

            setProducts(productData);

            const activeCategories = categoryData
                .filter((category) => category.isActive)
                .sort((firstCategory, secondCategory) =>
                    firstCategory.name.localeCompare(secondCategory.name),
                );

            setCategories(activeCategories);
        } catch (requestError) {
            if (requestError instanceof Error) {
                setError(requestError.message);
            } else {
                setError("Could not load products.");
            }
        } finally {
            setLoading(false);
        }
    }

    const visibleProducts = selectedCategoryId
        ? products.filter(
            (product) =>
                product.categoryId === Number(selectedCategoryId),
        )
        : products;

    return (
        <section>
        <div className= "page-heading" >
        <p className="eyebrow" > Fashion store </p>
            < h1 > Products </h1>
            < p > Browse products, view details, and choose a variant.</p>
                </div>

                < div className = "filter-row" >
                    <label className="filter-label" >
                        Category

                        < select
    value = { selectedCategoryId }
    onChange = {(event) =>
    setSelectedCategoryId(event.target.value)
}
                    >
    <option value="" > All categories </option>

{
    categories.map((category) => (
        <option key= { category.id } value = { category.id } >
        { category.parentCategoryId ? "— " : "" }
                                { category.name }
        </option>
    ))
}
</select>
    </label>

{
    !loading && (
        <p className="product-count" >
        { visibleProducts.length } product
    { visibleProducts.length === 1 ? "" : "s" } found
        </p>
                )
}
</div>

{ loading && <p>Loading products...</p> }

{ error && <p className="error-message" > { error } </p> }

{
    !loading && !error && visibleProducts.length === 0 && (
        <p>No products were found in this category.</p>
            )
}

<div className="product-grid" >
{
    visibleProducts.map((product) => (
        <ProductCard
                        key= { product.id }
                        product = { product }
                        onAddToCart = { onAddToCart }
                        onViewDetails = { onViewDetails }
        />
                ))
}
    </div>
    </section>
    );
}

export default ProductsPage;