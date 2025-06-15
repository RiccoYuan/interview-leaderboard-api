import { performance } from 'perf_hooks';
import { LeaderBoardEngine } from '../engine/LeaderBoardEngine';

describe('LeaderBoard Performance Tests', () => {
    let engine: LeaderBoardEngine;
    
    beforeAll(async () => {
        engine = new LeaderBoardEngine();
        
        // 预热：添加100万用户
        console.log('Warming up with 1,000,000 users...');
        const start = performance.now();
        
        for (let i = 1; i <= 1_000_000; i++) {
            await engine.updateScore(i, Math.floor(Math.random() * 1000));
        }
        
        const end = performance.now();
        console.log(`Warmup completed in ${((end - start) / 1000).toFixed(2)}s`);
    });
    
    afterAll(() => {
        engine.dispose();
    });
    
    test('Update Score Performance', async () => {
        const iterations = 1000000;
        console.log(`Testing update score performance with ${iterations} iterations...`);
        
        const start = performance.now();
        
        for (let i = 0; i < iterations; i++) {
            const customerId = Math.floor(Math.random() * 1_000_000) + 1;
            await engine.updateScore(customerId, Math.floor(Math.random() * 200) - 100);
        }
        
        const end = performance.now();
        const duration = end - start;
        const qps = iterations / (duration / 1000);
        
        console.log(`Update Score Performance:`);
        console.log(`- Total time: ${(duration / 1000).toFixed(2)}s`);
        console.log(`- QPS: ${qps.toFixed(2)}`);
        console.log(`- Average latency: ${(duration / iterations).toFixed(2)}ms`);
        
        expect(qps).toBeGreaterThan(10000);
    });
    
    test('Get Leaderboard Performance', async () => {
        const iterations = 100000;
        console.log(`Testing get leaderboard performance with ${iterations} iterations...`);
        
        const start = performance.now();
        
        for (let i = 0; i < iterations; i++) {
            await engine.getCustomersByRankRange(1, 100);
        }
        
        const end = performance.now();
        const duration = end - start;
        const qps = iterations / (duration / 1000);
        
        console.log(`Get Leaderboard Performance:`);
        console.log(`- Total time: ${(duration / 1000).toFixed(2)}s`);
        console.log(`- QPS: ${qps.toFixed(2)}`);
        console.log(`- Average latency: ${(duration / iterations).toFixed(2)}ms`);
        
        expect(qps).toBeGreaterThan(5000);
    });
    
    test('Get Neighbors Performance', async () => {
        const iterations = 100000;
        console.log(`Testing get neighbors performance with ${iterations} iterations...`);
        
        const start = performance.now();
        
        for (let i = 0; i < iterations; i++) {
            const customerId = Math.floor(Math.random() * 1_000_000) + 1;
            await engine.getCustomerNeighbors(customerId, 5, 5);
        }
        
        const end = performance.now();
        const duration = end - start;
        const qps = iterations / (duration / 1000);
        
        console.log(`Get Neighbors Performance:`);
        console.log(`- Total time: ${(duration / 1000).toFixed(2)}s`);
        console.log(`- QPS: ${qps.toFixed(2)}`);
        console.log(`- Average latency: ${(duration / iterations).toFixed(2)}ms`);
        
        expect(qps).toBeGreaterThan(3000);
    });
}); 