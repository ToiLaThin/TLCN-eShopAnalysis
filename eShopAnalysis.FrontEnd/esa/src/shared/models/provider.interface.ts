export interface IStockItemRequestMeta {
    productId: string;
    productModelId: string;
    businessKey: string;
    unitRequestPrice: number;
}

export interface IProviderRequirement {
    providerRequirementId: string;
    providerName: string;
    minPriceToBeAccepted: number;
    minQuantityToBeAccepted: number;
    providerBusinessKey: string;
    revision: number;
    isUsed: boolean;
    availableStockItemRequestMetas: IStockItemRequestMeta[];
    availableProviderCatalogIds: string[]
}

export interface IProductModelInfoWithStockAggregate {
    productModelId: string;
    productId: string;
    businessKey: string;
    productModelName: string;
    productCoverImage: number;
    price: number;
    unitRequestPrice: number;
    currentQuantity: number;
}