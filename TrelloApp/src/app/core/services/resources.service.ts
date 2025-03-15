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
    '#75e0b0',
    '#4bce97',
    '#19b076',
    '#f9e48e',
    '#F5CD47',
    '#f3bd2c',
    '#ffcfa9',
    '#FEA362',
    '#fd803a',
    '#fcaaa5',
    '#F87168',
    '#f04e43',
    '#c3bcf6',
    '#9F8FEF',
    '#856ae8',
    '#90c2ff',
    '#579DFF',
    '#357bfc'
  ];

  avatarBackgrounds: string[] = [
    'var(--color-red-500)',
    'var(--color-blue-500)',
    'var(--color-skyblue-500)',
    'var(--color-orange-500)',
    'var(--color-yellow-500)',
    'var(--color-green-500)'
  ];
}
