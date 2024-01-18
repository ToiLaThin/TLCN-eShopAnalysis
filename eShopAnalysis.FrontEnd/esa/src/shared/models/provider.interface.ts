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

export interface IStockItemRequest {
    productId: string;
    productModelId: string;
    businessKey: string;
    unitRequestPrice: number;
    itemQuantity: number;
    totalItemRequestPrice: number
}

export interface IProductModelInfoMergeStockItemRequest {
    productModelId: string;
    productId: string;
    businessKey: string;
    productModelName: string;
    productCoverImage: number;
    price: number;
    unitRequestPrice: number;
    currentQuantity: number;
    itemQuantity: number;
    totalItemRequestPrice: number;
    afterRequestQuantity: number;
}

export interface IStockRequestTransaction {
    //all ? prop will be calc or gen in server-side
    stockRequestTransactionId?: string;
    providerRequirementId: string;
    providerBusinessKey: string;
    totalTransactionPrice?: number;
    totalQuantity?: number;
    dateCreated?: Date;
    stockItemRequests: IStockItemRequest[];
}