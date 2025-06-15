import { Router, Request, Response, RequestHandler } from 'express';
import { LeaderBoardEngine } from '../engine/LeaderBoardEngine';

const router = Router();
const leaderBoardEngine = new LeaderBoardEngine();

// 更新分数
router.post('/v1/customer/:customerId/score/:score', (async (req: Request, res: Response) => {
    try {
        const customerId = parseInt(req.params.customerId);
        const score = parseFloat(req.params.score);
        
        // 参数验证
        if (score < -1000 || score > 1000) {
            return res.status(400).json({ error: '分数必须在 -1000 到 1000 之间' });
        }
        
        // 高性能更新
        const newScore = await leaderBoardEngine.updateScore(customerId, score);
        
        res.json(newScore);
    } catch (error) {
        console.error('Error updating score:', error);
        res.status(500).json({ error: '服务器内部错误' });
    }
}) as RequestHandler);

// 获取排行榜
router.get('/v1/leaderboard', (async (req: Request, res: Response) => {
    try {
        const start = parseInt(req.query.start as string) || 1;
        const end = parseInt(req.query.end as string) || 10;
        
        // 参数验证和边界检查
        if (start < 1 || end < start || end - start > 1000) {
            return res.status(400).json({ error: '无效的范围参数' });
        }
        
        // 高性能范围查询
        const customers = await leaderBoardEngine.getCustomersByRankRange(start, end);
        
        res.json(customers.map(customer => ({
            customerId: customer.customerId,
            score: customer.score,
            rank: customer.cachedRank
        })));
    } catch (error) {
        console.error('Error getting leaderboard:', error);
        res.status(500).json({ error: '服务器内部错误' });
    }
}) as RequestHandler);

// 获取邻居排名
router.get('/v1/leaderboard/:customerId', (async (req: Request, res: Response) => {
    try {
        const customerId = parseInt(req.params.customerId);
        const high = parseInt(req.query.high as string) || 0;
        const low = parseInt(req.query.low as string) || 0;
        
        // 参数验证
        if (high < 0 || low < 0 || high + low > 100) {
            return res.status(400).json({ error: '无效的邻居参数' });
        }
        
        // 高性能邻居查询
        const neighbors = await leaderBoardEngine.getCustomerNeighbors(customerId, high, low);
        
        if (!neighbors) {
            return res.status(404).json({ error: `未找到用户 ${customerId} 的排行榜信息` });
        }
        
        res.json(neighbors.map(customer => ({
            customerId: customer.customerId,
            score: customer.score,
            rank: customer.cachedRank
        })));
    } catch (error) {
        console.error('Error getting neighbors:', error);
        res.status(500).json({ error: '服务器内部错误' });
    }
}) as RequestHandler);

export default router; 