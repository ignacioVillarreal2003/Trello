import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ResourcesService {

  boardBackgroundPath: string = './board-backgrounds/';

  boardBackgrounds: string[] = [
    'background-1.svg',
    'background-2.svg',
    'background-3.svg',
    'background-4.svg',
    'background-5.svg',
    'background-6.svg',
    'background-7.svg',
    'background-8.svg',
    'background-9.svg',
    'background-10.svg',
    'background-11.svg',
    'background-12.svg',
    'background-13.svg',
    'background-14.svg',
    'background-15.svg'
  ];

  labelColors: string[] = [
    '#4BCE97',
    '#F5CD47',
    '#FEA362',
    '#F87168',
    '#9F8FEF',
    '#579DFF',
  ];
}
