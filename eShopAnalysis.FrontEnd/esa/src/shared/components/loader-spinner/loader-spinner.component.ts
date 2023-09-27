import { Component, OnInit } from '@angular/core';
import { ShowLoaderService } from 'src/shared/services/show-loader.service';

@Component({
  selector: 'esa-loader-spinner',
  templateUrl: './loader-spinner.component.html',
  styleUrls: ['./loader-spinner.component.scss']
})
export class LoaderSpinnerComponent implements OnInit {

  constructor(private loaderService: ShowLoaderService) { }

  ngOnInit(): void {}

  isLoadingSpinner(): boolean {
    return this.loaderService.getIsLoadingSpinner;
  }

}
