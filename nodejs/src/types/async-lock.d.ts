declare module 'async-lock' {
    export default class AsyncLock {
        acquire(key: string | string[], fn: () => Promise<any>): Promise<any>;
        acquire(key: string | string[], fn: () => any): Promise<any>;
        acquire(key: string | string[], options: { timeout?: number }, fn: () => Promise<any>): Promise<any>;
        acquire(key: string | string[], options: { timeout?: number }, fn: () => any): Promise<any>;
        release(key: string | string[]): void;
    }
} 