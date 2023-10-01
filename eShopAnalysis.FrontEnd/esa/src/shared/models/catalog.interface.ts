import { ISubCatalog } from "./subcatalog.interface";

export interface ICatalog {
    catalogId?: string;
    catalogName: string;
    catalogDescription: string;
    catalogImage?: string;
    subCatalogs?: ISubCatalog[];
}