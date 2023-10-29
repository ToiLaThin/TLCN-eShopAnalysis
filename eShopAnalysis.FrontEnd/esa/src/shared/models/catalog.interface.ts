
export interface ISubCatalog {
    subCatalogId?: string;
    subCatalogName: string;    
    subCatalogDescription: string;
    subCatalogImage?: string;
}

export interface ICatalog {
    catalogId?: string;
    catalogName: string;
    catalogDescription: string;
    catalogImage?: string;
    subCatalogs?: ISubCatalog[];
}