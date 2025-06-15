# LeaderBoard.API (Node.js 版本)

## 📋 项目概述

一个基于 Node.js 的高性能实时排行榜 API 服务，专为处理大规模并发请求和百万级用户排名而设计。使用 TypeScript 开发，确保代码的可维护性和类型安全。

## 🎯 性能目标

| 指标 | 目标值 |
|------|--------|
| 分数更新响应时间 | < 1ms |
| 排名查询响应时间 | < 5ms |
| 并发处理能力 | > 10,000 QPS |
| 支持用户规模 | 1,000,000+ |
| 排名一致性 | 99.9% 实时 |

## 🏗️ 核心架构设计

### 架构图
```
┌─────────────────────────────────────────────┐
│              Express API Layer              │
├─────────────────────────────────────────────┤
│           Service Layer (Async)             │
├─────────────────────────────────────────────┤
│  Memory Engine │  Ranking Engine │ Cache   │
├─────────────────────────────────────────────┤
│        High-Performance Data Structures     │
└─────────────────────────────────────────────┘
```

### 三层处理模型

1. **接入层**：使用 Express.js 快速接收请求，立即响应
2. **计算层**：使用 Node.js 异步处理复杂排名计算
3. **存储层**：优化的内存数据结构

## 🔧 核心数据结构

### 主要组件

```typescript
class LeaderBoardEngine {
    // 客户数据快速查找
    private customers: Map<number, CustomerNode>;
    
    // 跳表维护有序排名
    private rankedList: SkipList<CustomerNode>;
    
    // 排名到客户映射（预计算）
    private rankToCustomerId: Map<number, number>;
    
    // 分段锁减少竞争
    private segmentLocks: AsyncLock[];
    
    // 异步更新队列
    private updateQueue: AsyncQueue<RankingUpdateTask>;
    
    // 热点数据缓存
    private hotDataCache: NodeCache;
}
```

### 客户节点结构

```typescript
class CustomerNode {
    constructor(
        public customerId: number,
        public score: number,
        public cachedRank: number = 0,
        public rankDirty: boolean = true,
        public lastUpdate: Date = new Date()
    ) {}

    compareTo(other: CustomerNode): number {
        // 先按分数降序，再按CustomerID升序
        const scoreComparison = other.score - this.score;
        return scoreComparison !== 0 ? scoreComparison : this.customerId - other.customerId;
    }
}
```

## ⚡ 性能优化策略

### 1. 异步分离架构

```typescript
async updateScore(customerId: number, delta: number): Promise<number> {
    // 立即更新分数
    const customer = this.getOrCreateCustomer(customerId);
    const newScore = await this.updateScoreImmediate(customer, delta);
    
    // 异步更新排名
    this.enqueueRankingUpdate(customer).catch(console.error);
    
    return newScore;
}

private async updateScoreImmediate(customer: CustomerNode, delta: number): Promise<number> {
    const segmentIndex = this.getSegmentIndex(customer.customerId);
    await this.segmentLocks[segmentIndex].acquire();
    
    try {
        customer.score += delta;
        customer.rankDirty = true;
        customer.lastUpdate = new Date();
        return customer.score;
    } finally {
        this.segmentLocks[segmentIndex].release();
    }
}
```

### 2. 批量排名更新

```typescript
class BatchProcessor {
    private timer: NodeJS.Timer;
    private pendingUpdates: AsyncQueue<CustomerNode>;
    
    constructor() {
        this.pendingUpdates = new AsyncQueue();
        this.timer = setInterval(() => this.processPendingUpdates(), 10);
    }
    
    private async processPendingUpdates(): Promise<void> {
        const updates: CustomerNode[] = [];
        const maxBatchSize = this.getOptimalBatchSize();
        
        // 收集待处理更新
        while (updates.length < maxBatchSize) {
            const customer = await this.pendingUpdates.dequeue();
            if (!customer) break;
            
            if (customer.rankDirty) {
                updates.push(customer);
            }
        }
        
        if (updates.length > 0) {
            await this.processRankingUpdatesBatch(updates);
        }
    }
}
```

### 3. 智能缓存策略

```typescript
class HotDataCacheManager {
    private cache: NodeCache;
    private cacheStats: Map<string, CacheStatistics>;
    
    // 缓存前N名排行榜
    private static readonly TOP_RANKINGS_KEY = 'top_rankings';
    private static readonly TOP_RANKINGS_SIZE = 100;
    
    // 缓存热点客户
    private hotCustomers: Map<number, HotCustomerData>;
    
    async getTopRankings(count: number): Promise<CustomerRankInfo[]> {
        const cacheKey = `${HotDataCacheManager.TOP_RANKINGS_KEY}_${count}`;
        
        const cachedData = this.cache.get<CustomerRankInfo[]>(cacheKey);
        if (cachedData) {
            this.updateCacheHitStats(cacheKey);
            return cachedData;
        }
        
        const freshData = await this.computeTopRankings(count);
        this.cache.set(cacheKey, freshData, 1000); // 1秒过期
        
        return freshData;
    }
}
```

## 🚀 API 实现

### 1. 分数更新接口

```typescript
router.post('/customer/:customerId/score/:score', async (req: Request, res: Response) => {
    try {
        const customerId = parseInt(req.params.customerId);
        const score = parseFloat(req.params.score);
        
        // 参数验证
        if (score < -1000 || score > 1000) {
            return res.status(400).json({ error: 'Score must be between -1000 and 1000' });
        }
        
        // 高性能更新
        const newScore = await leaderBoardEngine.updateScore(customerId, score);
        
        res.json({
            customerId,
            currentScore: newScore,
            updateTime: new Date()
        });
    } catch (error) {
        console.error('Error updating score:', error);
        res.status(500).json({ error: 'Internal server error' });
    }
});
```

### 2. 排名查询接口

```typescript
router.get('/leaderboard', async (req: Request, res: Response) => {
    try {
        const start = parseInt(req.query.start as string) || 1;
        const end = parseInt(req.query.end as string) || 10;
        
        // 参数验证和边界检查
        if (start < 1 || end < start || end - start > 1000) {
            return res.status(400).json({ error: 'Invalid range parameters' });
        }
        
        // 高性能范围查询
        const customers = await leaderBoardEngine.getCustomersByRankRange(start, end);
        
        res.json({
            customers: customers.map(this.mapToResponseModel),
            startRank: start,
            endRank: end,
            totalCount: await leaderBoardEngine.getTotalCustomers()
        });
    } catch (error) {
        console.error('Error getting leaderboard:', error);
        res.status(500).json({ error: 'Internal server error' });
    }
});
```

## 🧪 性能测试

### 基准测试

```typescript
import { performance } from 'perf_hooks';
import { LeaderBoardEngine } from '../src/engine';

describe('LeaderBoard Performance Tests', () => {
    let engine: LeaderBoardEngine;
    
    beforeAll(async () => {
        engine = new LeaderBoardEngine();
        
        // 预热：添加100万用户
        for (let i = 1; i <= 1_000_000; i++) {
            await engine.updateScore(i, Math.floor(Math.random() * 1000));
        }
    });
    
    test('Update Score Performance', async () => {
        const iterations = 1000000;
        const start = performance.now();
        
        for (let i = 0; i < iterations; i++) {
            const customerId = Math.floor(Math.random() * 1_000_000) + 1;
            await engine.updateScore(customerId, Math.floor(Math.random() * 200) - 100);
        }
        
        const end = performance.now();
        const duration = end - start;
        const qps = iterations / (duration / 1000);
        
        console.log(`Update Score QPS: ${qps.toFixed(2)}`);
        expect(qps).toBeGreaterThan(10000);
    });
    
    test('Get Leaderboard Performance', async () => {
        const iterations = 100000;
        const start = performance.now();
        
        for (let i = 0; i < iterations; i++) {
            await engine.getCustomersByRankRange(1, 100);
        }
        
        const end = performance.now();
        const duration = end - start;
        const qps = iterations / (duration / 1000);
        
        console.log(`Get Leaderboard QPS: ${qps.toFixed(2)}`);
        expect(qps).toBeGreaterThan(5000);
    });
});
```

### 压力测试场景

1. **高频更新测试**：使用 Artillery 模拟每秒10万次分数更新
2. **混合负载测试**：70%查询 + 30%更新
3. **大规模用户测试**：1000万用户同时在线
4. **峰值流量测试**：突发10倍流量冲击

## 📈 预期性能表现

| 操作类型 | QPS | 平均延迟 | P99延迟 | 内存使用 |
|----------|-----|----------|---------|----------|
| 分数更新 | 100,000+ | 0.5ms | 2ms | 线性增长 |
| 排名查询 | 50,000+ | 2ms | 10ms | 常量级 |
| 邻居查询 | 30,000+ | 3ms | 15ms | 常量级 |

## 🔧 项目设置

### 依赖项

```json
{
  "dependencies": {
    "express": "^4.18.2",
    "node-cache": "^5.1.2",
    "async-lock": "^1.4.0",
    "typescript": "^5.0.0"
  },
  "devDependencies": {
    "@types/express": "^4.17.17",
    "@types/node": "^18.15.0",
    "jest": "^29.5.0",
    "ts-jest": "^29.1.0",
    "artillery": "^2.0.0"
  }
}
```

### 运行测试

```bash
# 安装依赖
npm install

# 运行单元测试
npm test

# 运行性能测试
npm run test:perf

# 运行压力测试
npm run test:stress
```

这个 Node.js 实现通过利用 Node.js 的异步特性和事件驱动模型，结合优化的数据结构和缓存策略，实现了与 .NET 版本相当的性能表现。主要优化点包括：

1. 使用 TypeScript 确保代码质量和类型安全
2. 利用 Node.js 的异步 I/O 模型处理并发
3. 实现高效的内存数据结构和缓存策略
4. 使用分段锁减少竞争
5. 批量处理排名更新
6. 智能缓存热点数据 