import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductEditComponent } from './product-edit/product-edit.component';
import { ProductAddComponent } from './product-add/product-add.component';
import { ProductListComponent } from './product-list/product-list.component';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from 'src/shared/shared.module';
import { ProductCatalogComponent } from './product-catalog.component';
import { ProductSidenavLinksComponent } from './product-sidenav-links/product-sidenav-links.component';
import { TestComponent } from './product-add/test/test.component';
import { CatalogAddComponent } from './catalog-add/catalog-add.component';
import { SubcatalogAddComponent } from './subcatalog-add/subcatalog-add.component';
import { CatalogListComponent } from './catalog-list/catalog-list.component';
import { SubcatalogListComponent } from './subcatalog-list/subcatalog-list.component';
import { RoleGuard } from 'src/shared/guards/role.guard';


const productCatalogRoutes: Routes = [
  {
    canActivate: [RoleGuard],
    path: '',
    component: ProductCatalogComponent,
    pathMatch: 'prefix',
    // why pathMatch: 'prefix' is good but 'full' is not runable?
    children: [
      {
        path: 'product-list',
        component: ProductListComponent,
        outlet: 'primary',
        // will be infer to be router-outlet without name inside ProductCatalogComponent, can be omitted
        pathMatch: 'full',
      },
      {
        path: 'product-add',
        component: ProductAddComponent,
        outlet: 'primary',
        pathMatch: 'prefix',
        //this prefix work but full not work
        children: [
          {
            path: 'product-test',
            component: TestComponent,
            outlet: 'primary',
            pathMatch: 'full',
          },
        ]
      },
      {
        path: 'product-edit',
        component: ProductEditComponent,
        outlet: 'primary',
        pathMatch: 'full',
      },
      {
        path: 'catalog-list',
        component: CatalogListComponent,
        outlet: 'primary',
        pathMatch: 'full',
      },
      {
        path: 'catalog-add',
        component: CatalogAddComponent,
        outlet: 'primary',
        pathMatch: 'full',
      },
      {
        path: 'subcatalog-add',
        component: SubcatalogAddComponent,
        outlet: 'primary',
        pathMatch: 'full',
      },
      {
        path: 'subcatalog-list/:catalogId',
        component: SubcatalogListComponent,
        outlet: 'primary',
        pathMatch: 'full',
      },
      {
        path: 'subcatalog-add',
        component: SubcatalogAddComponent,
        outlet: 'primary',
        pathMatch: 'full',
      }
    ]
  },
  
]

@NgModule({
  declarations: [
    ProductEditComponent,
    ProductAddComponent,
    ProductListComponent,
    ProductCatalogComponent,
    ProductSidenavLinksComponent,
    TestComponent,
    CatalogAddComponent,
    SubcatalogAddComponent,
    CatalogListComponent,
    SubcatalogListComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(productCatalogRoutes),
    SharedModule,
  ]
})
export class ProductCatalogModule { }
