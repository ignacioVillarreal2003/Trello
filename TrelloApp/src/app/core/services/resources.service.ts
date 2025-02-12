import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ResourcesService {

  backgroundPath: string = './backgrounds/';

  backgrounds: string[] = [
    'background-0.svg',
    'background-1.svg',
    'background-2.svg',
    'background-3.svg',
    'background-4.svg',
    'background-5.svg',
    'background-6.svg',
    'background-7.svg',
    'background-8.svg',
    'background-9.svg'
  ];

  colors: string[] = [
    '#4BCE97',
    '#F5CD47',
    '#FEA362',
    '#F87168',
    '#9F8FEF',
    '#579DFF',
  ];

  iconPath: string = './icons/'

  icons: string[] = [
    'icon-1.png',
    'icon-2.png',
    'icon-3.png',
    'icon-4.png',
    'icon-5.png',
    'icon-6.png',
    'icon-7.png',
    'icon-8.png',
    'icon-9.png',
    'icon-10.png',
    'icon-11.png',
    'icon-12.png',
  ];
}
