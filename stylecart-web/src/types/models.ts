export type Product = {
    id: number;
    categoryId: number;
    name: string;
    description?: string;
    brand?: string;
    basePrice: number;
    isActive: boolean;
    createdAt: string;
};

export type CatalogProduct = Product & {
    isInStock: boolean;
    imageUrl?: string;
};

export type Category = {
    id: number;
    name: string;
    description?: string;
    parentCategoryId: number | null;
    isActive: boolean;
    createdAt: string;
};

export type ProductVariant = {
    id: number;
    productId: number;
    size: string;
    color: string;
    sku: string;
    price: number;
    isActive: boolean;
    isInStock: boolean;
};

export type LoginResponse = {
    token: string;
    expiresAt: string;
    userId: string;
    email: string;
    role: string;
};

export type CartItem = {
    id: number;
    productVariantId: number;
    productName: string;
    size: string;
    color: string;
    unitPrice: number;
    quantity: number;
    lineTotal: number;
};

export type Cart = {
    id: number;
    userId: string;
    updatedAt: string;
    items: CartItem[];
};

export type CheckoutRequest = {
    recipientName: string;
    shippingAddressLine1: string;
    city: string;
    state: string;
    postalCode: string;
    country: string;
    phoneNumber: string;
};

export type OrderItem = {
    id: number;
    productVariantId: number;
    productName: string;
    variantDescription: string;
    unitPrice: number;
    quantity: number;
    lineTotal: number;
};

export type Order = {
    id: number;
    orderDate: string;
    totalAmount: number;
    orderStatus: string;
    paymentStatus: string;
    recipientName: string;
    shippingAddressLine1: string;
    city: string;
    state: string;
    postalCode: string;
    country: string;
    phoneNumber: string;
    items: OrderItem[];
};