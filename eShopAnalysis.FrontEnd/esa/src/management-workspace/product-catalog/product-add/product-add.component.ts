import { Component, ElementRef, OnDestroy, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { map, Observable, Subscription } from 'rxjs';
import { CublicType, IProduct } from 'src/shared/models/product.interface';
import { CatalogHttpService } from 'src/shared/services/http/catalog-http.service';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';
import { ISubCatalog } from 'src/shared/models/catalog.interface';

@Component({
  selector: 'esa-product-add',
  templateUrl: './product-add.component.html',
  styleUrls: ['./product-add.component.scss']
})
export class ProductAddComponent implements OnInit, OnDestroy {
  
  constructor(private productService: ProductHttpService,
              private catalogService: CatalogHttpService,
              private router: Router,
              private fb: FormBuilder) { }
    cublicTypeSubcription!: Subscription;      
    ngOnDestroy(): void {
      this.cublicTypeSubcription.unsubscribe();
    }
    //we can remove form group inside a form group, and remove control inside a form group
    
    
  @ViewChildren('checkboxHavePricePerCublic', { read: ElementRef}) checkboxHavePricePerCublics!: QueryList<ElementRef>;
  newProduct!: IProduct;
  allCatalogs$ = this.catalogService.allCatalog$;
  subCatalogVisible$!: Observable<ISubCatalog[] | undefined>;
  havePricePerCublic: boolean = false;
  haveVariants: boolean = false;
  get CublicType() {
    return CublicType;
  }
  cublicKeyArr = Object.keys(CublicType).map(x => parseInt(x)).filter(x => !isNaN(x));
  cublicKeyValueArr = this.cublicKeyArr.map((key) => {
    //if we have variants, we do not want to show N, check in toggleHaveVariants()
    if (this.haveVariants !== true) { 
      return { key, value: CublicType[key] }
    }
    if (key === CublicType.N) { 
      return undefined; 
    } 
    return { key, value: CublicType[key] }
  });

  //template for a product model form should be add in the form array
  productModelFrmGrp(): FormGroup { 
    return this.fb.group({ //phải có return, nếu không sẽ ref tới cùng 1 form group
      productCublicType: [CublicType.M], //this is important, if we do not set default value to be enum, but give it string , it cannot compare
      productCublicValue: [0],
      productPricePerCublicValue: [0],
      productPriceNonCublic: [0],
    });  
  }
  godProductFrmGrp: FormGroup = this.fb.group({
    catalogProductFrmGrp: this.fb.group({
      catalogId : [''],
      subCatalogId: ['']
    }),
    productEssentialFrmGrp: this.fb.group({
      productName: [''],
      productCoverImage: ['']
    }),
    productInfoFrmGrp: this.fb.group({
      productDescription: [''],
      productBrand: ['']
    }),

    productModelsFrmArr: this.fb.array([])
      
  });
  //like shortcut to get form group
  get catalogProductFrmGrp() { return this.godProductFrmGrp.get('catalogProductFrmGrp') as FormGroup; }
  get productEssentialFrmGrp() { return this.godProductFrmGrp.get('productEssentialFrmGrp') as FormGroup; }
  get productInfoFrmGrp() { return this.godProductFrmGrp.get('productInfoFrmGrp') as FormGroup; }
  get productModelsFrmArr() { return this.godProductFrmGrp.get('productModelsFrmArr') as FormArray; }

  addProductModel() {
    this.productModelsFrmArr.push(this.productModelFrmGrp())
  }

  removeProductModel(index: number) {
    this.productModelsFrmArr.removeAt(index);    
  }

  reloadSubCatalogs() {
    let catalogId = this.catalogProductFrmGrp.get('catalogId')?.value;
    this.subCatalogVisible$ = this.catalogService.allCatalog$.pipe(
      map((cat) => {
        const subCatalogs = cat.find((c) => c.catalogId === this.catalogProductFrmGrp.get('catalogId')?.value)?.subCatalogs;
        return subCatalogs;
      }),    
    )
  }  
  
  toggleHavePricePerCublic() { this.havePricePerCublic = !this.havePricePerCublic; }
  toggleHaveVariants() { 
    this.haveVariants = !this.haveVariants; 
    this.cublicKeyValueArr = this.cublicKeyArr.map((key) => {
      //if we have variants, we do not want to show N
      if (this.haveVariants === true) {
        if (key !== CublicType.N) {
          return {
            key,
            value: CublicType[key]
          }
        } else {
          return undefined;
        }
      } else {
        return {
          key,
          value: CublicType[key]
        }
      };
    });
    //reset all controls in form array, if we toggle have variants,form array only have one form group as init state
    let productModelFrmGrpTemp = this.productModelsFrmArr.controls[0] as FormGroup;
    console.log("Before reset",productModelFrmGrpTemp.controls);
    this.productModelsFrmArr.clear();    
    this.productModelsFrmArr.push(this.productModelFrmGrp());
    console.log("After reset",productModelFrmGrpTemp.controls);
    
  }
  
 
  ngOnInit(): void {
    this.newProduct = {
      productModels: []
    };
    this.productModelsFrmArr.push(this.productModelFrmGrp());
    //subscribe to value change observable of a form control, using selectionChange do not work
    //since this is reactive form, we need to subscribe to value change observable

    //only for if not having variant, having variant will not have cublic type == N
    let productModelFrmGrpTemp = this.productModelsFrmArr.controls[0] as FormGroup;
    this.cublicTypeSubcription = productModelFrmGrpTemp.controls["productCublicType"].valueChanges.subscribe((value) => {
      if (value === CublicType.N) {
        productModelFrmGrpTemp.controls["productCublicValue"].reset();
        productModelFrmGrpTemp.controls["productCublicValue"].disable();
        this.checkboxHavePricePerCublics.forEach((checkbox) => {
          (checkbox.nativeElement as HTMLElement).style.display = "none";
        });
      } else {
        productModelFrmGrpTemp.controls["productCublicValue"].enable();
        this.checkboxHavePricePerCublics.forEach((checkbox) => {
          (checkbox.nativeElement as HTMLElement).style.display = "block";
        });        
      }
    });

    //remember, have price per cublic is of the product, 
    //so if one model have price per cublic, all model have price per cublic
  }


  addNewProduct() {
    this.newProduct.productName = this.productEssentialFrmGrp.get('productName')?.value;
    this.newProduct.subCatalogId = this.catalogProductFrmGrp.get('subCatalogId')?.value;
    this.newProduct.subCatalogName = document.getElementById('subCatalogId')?.textContent as string;
    console.log(this.newProduct.subCatalogName);
    
    this.newProduct.productCoverImage = this.productEssentialFrmGrp.get('productCoverImage')?.value;
    this.newProduct.isOnSale = false;
    this.newProduct.productDisplaySaleValue = undefined;
    this.newProduct.productDisplayPriceOnSale = undefined;
    this.newProduct.haveVariants = this.haveVariants;
    this.newProduct.havePricePerCublic = this.havePricePerCublic;
    this.newProduct.revision = 0;
    this.newProduct.productInfo = {
      productDescription: this.productInfoFrmGrp.get('productDescription')?.value,
      productBrand: this.productInfoFrmGrp.get('productBrand')?.value,
    }

    //from here
    if (this.haveVariants === false) {      
      //but since this is haveVariants === false, we only have one form group in the form array
      let productModelFrmGrpTemp = this.productModelsFrmArr.controls[0] as FormGroup;
      const pCublicType = productModelFrmGrpTemp.get('productCublicType')?.value;
      let pCublicValue = undefined;
      let pPricePerCublicValue = undefined;
      if (pCublicType !== CublicType.N) {
        pCublicValue = productModelFrmGrpTemp.get('productCublicValue')?.value;
        if (this.havePricePerCublic === true) {
          pPricePerCublicValue = productModelFrmGrpTemp.get('productPricePerCublicValue')?.value;
        }
      }
      let pCublicPrice = (pCublicValue !== undefined &&  pPricePerCublicValue !== undefined) ? pCublicValue * pPricePerCublicValue : undefined;

      this.newProduct.productModels = [ 
        {
          productModelId: undefined,
          productModelThumbnails: [],
          cublicType: productModelFrmGrpTemp.get('productCublicType')?.value,
          cublicValue: pCublicValue,
          pricePerCublicValue: pPricePerCublicValue,
          cublicPrice: pCublicPrice,
          price: pCublicPrice !== undefined ? pCublicPrice : productModelFrmGrpTemp.get('productPriceNonCublic')?.value,
          isOnSaleModel: false,
          saleValueModel: undefined,
          saleType: undefined,
          priceOnSaleModel: undefined,
        }
      ];
      this.productService.AddProduct(this.newProduct);
      this.router.navigate(['/management/product-catalog']);
    } else { //add all product models in the form array to the product, then call the product service
      this.productModelsFrmArr.controls.forEach(modelFrmGrp => {
        console.log("Here are each model group",modelFrmGrp);
        
        let pmCublicValue = undefined;
        let pmPricePerCublicValue = undefined;
        pmCublicValue = modelFrmGrp.get('productCublicValue')?.value;
        if (this.havePricePerCublic === true) {
          pmPricePerCublicValue = modelFrmGrp.get('productPricePerCublicValue')?.value;
        }
        let pmCublicPrice = (pmCublicValue !== undefined &&  pmPricePerCublicValue !== undefined) ? pmCublicValue * pmPricePerCublicValue : undefined;

        let thisProductModel = {
          productModelId: undefined,
          productModelThumbnails: [],
          cublicType: modelFrmGrp.get('productCublicType')?.value,
          cublicValue: pmCublicValue,
          pricePerCublicValue: pmPricePerCublicValue,
          cublicPrice: pmCublicPrice,
          price: pmCublicPrice !== undefined ? pmCublicPrice : modelFrmGrp.get('productPriceNonCublic')?.value,
          isOnSaleModel: false,
          productDisplaySaleValue: undefined,
          productDisplaySaleType: undefined,
          saleType: undefined,
          productDisplayPriceOnSale: undefined,
        };
        this.newProduct.productModels.push(thisProductModel);
      });
      this.productService.AddProduct(this.newProduct);
    }
  }
}

