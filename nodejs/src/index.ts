import express from 'express';
import leaderboardRouter from './routes/leaderboard';

const app = express();
// 从命令行参数获取端口，如果没有则使用默认值 3000
const port = process.argv.find(arg => arg.startsWith('--port='))?.split('=')[1] || process.env.PORT || 3000;

// 中间件
app.use(express.json());

// 路由
app.use('/api', leaderboardRouter);

// 错误处理
app.use((err: Error, req: express.Request, res: express.Response, next: express.NextFunction) => {
    console.error('未处理的错误:', err);
    res.status(500).json({ error: '服务器内部错误' });
});

// 启动服务器
app.listen(port, () => {
    console.log(`服务器运行在端口 ${port}`);
}); 