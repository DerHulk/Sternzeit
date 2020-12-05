import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  private storage: any;

    constructor() {
        this.storage = sessionStorage; // localStorage;
    }

    public read<T>(key: string): T {
        const item = this.storage.getItem(key);

        if (item && item !== 'undefined') {
            return <T> JSON.parse(this.storage.getItem(key));
        }

        return;
    }

    public save(key: string, value: any) {
        this.storage.setItem(key, JSON.stringify(value));
    }
}
