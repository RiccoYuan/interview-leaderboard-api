export class AsyncQueue<T> {
    private items: T[];
    private waiting: ((item: T) => void)[];
    private readonly maxSize: number;
    
    constructor(maxSize: number = 10000) {
        this.items = [];
        this.waiting = [];
        this.maxSize = maxSize;
    }
    
    enqueue(item: T): void {
        if (this.waiting.length > 0) {
            const resolve = this.waiting.shift();
            if (resolve) resolve(item);
        } else if (this.items.length < this.maxSize) {
            this.items.push(item);
        } else {
            // 队列已满，等待消费者
            new Promise<void>(resolve => {
                this.waiting.push((item: T) => {
                    resolve();
                    return item;
                });
            }).then(() => this.items.push(item));
        }
    }
    
    async dequeue(): Promise<T> {
        if (this.items.length > 0) {
            return Promise.resolve(this.items.shift() as T);
        }
        return new Promise<T>(resolve => {
            this.waiting.push(resolve);
        });
    }
    
    get length(): number {
        return this.items.length;
    }
    
    clear(): void {
        this.items = [];
        this.waiting = [];
    }
    
    // 批量出队
    async dequeueBatch(count: number): Promise<T[]> {
        const result: T[] = [];
        const actualCount = Math.min(count, this.items.length);
        
        for (let i = 0; i < actualCount; i++) {
            result.push(this.items.shift() as T);
        }
        
        if (result.length > 0) {
            return result;
        }
        
        // 如果没有足够的项目，等待一个项目
        const item = await this.dequeue();
        result.push(item);
        return result;
    }
    
    // 批量入队
    enqueueBatch(items: T[]): void {
        for (const item of items) {
            this.enqueue(item);
        }
    }
} 