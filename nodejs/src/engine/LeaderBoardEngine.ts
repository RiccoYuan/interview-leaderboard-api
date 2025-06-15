import { CustomerNode } from '../models/CustomerNode';
import { SkipList } from '../structures/SkipList';
import { AsyncQueue } from '../structures/AsyncQueue';
import AsyncLock from 'async-lock';
import NodeCache from 'node-cache';

interface RankingUpdateTask {
    customer: CustomerNode;
    timestamp: Date;
}

export class LeaderBoardEngine {
    private customers: Map<number, CustomerNode>;
    private rankedList: SkipList;
    private rankToCustomerId: Map<number, number>;
    private segmentLocks: AsyncLock[];
    private updateQueue: AsyncQueue<RankingUpdateTask>;
    private hotDataCache: NodeCache;
    private batchProcessor: NodeJS.Timer;
    private isProcessing: boolean;
    private readonly segmentCount: number;
    private readonly maxBatchSize: number;
    
    constructor() {
        this.customers = new Map();
        this.rankedList = new SkipList();
        this.rankToCustomerId = new Map();
        this.segmentCount = Math.max(32, Math.ceil(require('os').cpus().length * 4));
        this.segmentLocks = Array.from({ length: this.segmentCount }, () => new AsyncLock());
        this.updateQueue = new AsyncQueue();
        this.hotDataCache = new NodeCache({ 
            stdTTL: 5,  // 增加到5秒
            checkperiod: 1,
            useClones: false  // 减少内存使用
        });
        this.isProcessing = false;
        this.maxBatchSize = 2000;
        
        // 启动批量处理器
        this.batchProcessor = setInterval(() => this.processPendingUpdates(), 10);
    }
    
    private getOrCreateCustomer(customerId: number): CustomerNode {
        let customer = this.customers.get(customerId);
        if (!customer) {
            customer = new CustomerNode(customerId, 0);
            this.customers.set(customerId, customer);
            this.rankedList.insert(customer);
        }
        return customer;
    }
    
    async updateScore(customerId: number, delta: number): Promise<number> {
        const customer = this.getOrCreateCustomer(customerId);
        const newScore = this.updateScoreImmediate(customer, delta);
        
        // 异步更新排名
        this.enqueueRankingUpdate(customer).catch(console.error);
        
        return newScore;
    }
    
    private updateScoreImmediate(customer: CustomerNode, delta: number): number {
        // 内联优化：直接更新分数
        customer.score += delta;
        return customer.score;
    }
    
    private async enqueueRankingUpdate(customer: CustomerNode): Promise<void> {
        await this.updateQueue.enqueue({
            customer,
            timestamp: new Date()
        });
    }
    
    private async processPendingUpdates(): Promise<void> {
        if (this.isProcessing) return;
        
        try {
            this.isProcessing = true;
            const updates: CustomerNode[] = [];
            
            // 批量获取更新任务
            while (updates.length < this.maxBatchSize && this.updateQueue.length > 0) {
                const task = await this.updateQueue.dequeue();
                if (task.customer.rankDirty) {
                    updates.push(task.customer);
                }
            }
            
            if (updates.length > 0) {
                await this.processRankingUpdatesBatch(updates);
            }
        } finally {
            this.isProcessing = false;
        }
    }
    
    private async processRankingUpdatesBatch(updates: CustomerNode[]): Promise<void> {
        // 按customerId分组，减少锁竞争
        const groupedUpdates = new Map<number, CustomerNode[]>();
        for (const customer of updates) {
            const groupId = customer.customerId % this.segmentCount;
            if (!groupedUpdates.has(groupId)) {
                groupedUpdates.set(groupId, []);
            }
            groupedUpdates.get(groupId)!.push(customer);
        }
        
        // 并行处理每个分组
        await Promise.all(
            Array.from(groupedUpdates.entries()).map(async ([groupId, customers]) => {
                const lock = this.segmentLocks[groupId];
                await lock.acquire('update', async () => {
                    for (const customer of customers) {
                        this.rankedList.insert(customer);
                        customer.updateRank();
                    }
                });
            })
        );
    }
    
    async getCustomersByRankRange(start: number, count: number): Promise<CustomerNode[]> {
        const cacheKey = `rankings_${start}_${count}`;
        const cachedData = this.hotDataCache.get<CustomerNode[]>(cacheKey);
        
        if (cachedData) {
            return cachedData;
        }
        
        const customers = this.rankedList.getRange(start - 1, count);
        this.hotDataCache.set(cacheKey, customers);
        
        return customers;
    }
    
    async getCustomerNeighbors(customerId: number, high: number, low: number): Promise<CustomerNode[]> {
        const customer = this.customers.get(customerId);
        if (!customer) {
            return [];
        }
        
        const rank = customer.cachedRank;
        const start = Math.max(1, rank - high);
        const count = high + low + 1;
        
        return this.getCustomersByRankRange(start, count);
    }
    
    async getTotalCustomers(): Promise<number> {
        return this.customers.size;
    }
    
    private stopBatchProcessor(): void {
        if (this.batchProcessor) {
            clearInterval(this.batchProcessor as unknown as number);
            this.batchProcessor = null as unknown as NodeJS.Timeout;
        }
    }
    
    dispose(): void {
        this.stopBatchProcessor();
        this.hotDataCache.flushAll();
    }
} 