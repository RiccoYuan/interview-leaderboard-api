# 实时排行榜系统

## 项目说明

本项目是作为面试题目"实时排行榜系统"的解决方案。面试题目详细要求请参考 `docs/interview Codes for donnet-updated.pdf`。

### 面试题目要求概述

1. 实现一个实时排行榜系统，支持以下功能：
   - 更新客户分数
   - 查询指定排名范围的客户
   - 查询指定客户ID的排名及其相邻排名

2. 技术栈要求：
   - 使用.NET技术栈实现
   - 提供RESTful API接口
   - 确保高并发性能和实时性

3. 性能要求：
   - 支持高并发请求
   - 保证数据一致性
   - 优化查询性能

## 需求分析

### 核心业务需求

1. 客户管理
   - 每个客户有唯一的CustomerID (int64)
   - 每个客户有对应的分数(decimal)
   - 支持客户信息的增删改查

2. 排行榜机制
   - 仅分数>0的客户参与排名
   - 按分数降序排列
   - 相同分数时按CustomerID升序排列
   - 支持实时更新排名

3. 查询功能
   - 支持按排名范围查询
   - 支持按客户ID查询其排名及相邻排名
   - 支持实时获取最新排名

### 非功能性需求

1. 性能要求
   - 高并发处理能力
   - 低延迟响应
   - 实时排名更新

2. 可靠性要求
   - 数据一致性
   - 线程安全
   - 异常处理

## 项目实现

本项目提供了两种技术栈的实现方案：

### .NET实现 (.NET 9)

位于 `dotnet` 目录下，采用分层架构设计：

- `LeaderBoard.API`: Web API层，处理HTTP请求
- `LeaderBoard.Core`: 核心业务逻辑层
- `LeaderBoard.Models`: 数据模型层
- `LeaderBoard.API.Tests`: 单元测试项目

核心实现方案：
1. 数据结构
   - 使用ConcurrentDictionary存储客户信息
   - 使用ConcurrentSkipList实现排行榜
   - 使用Channel实现异步消息队列

2. 并发控制
   - 使用ReaderWriterLockSlim实现细粒度锁
   - 基于CPU核心数动态调整分段锁数量
   - 实现读写锁分离

3. 性能优化
   - 使用向量化计算（AVX2/SSE2）
   - 实现批量处理机制
   - 使用内存缓存优化查询

### Node.js实现

位于 `nodejs` 目录下，采用TypeScript开发：

- `src`: 源代码目录
- `tests`: 测试代码目录
- 使用TypeScript确保类型安全
- 包含完整的测试用例

核心实现方案：
1. 数据结构
   - 使用Map存储客户信息
   - 使用SkipList实现排行榜
   - 使用AsyncQueue实现异步消息队列

2. 并发控制
   - 使用AsyncLock实现分段锁
   - 基于CPU核心数动态调整锁数量
   - 实现批量处理机制

3. 性能优化
   - 优化内存分配
   - 实现智能缓存策略
   - 使用批量操作减少开销

## 实现方案对比

### 相同点
1. 核心算法
   - 都使用SkipList作为主要数据结构
   - 都实现了分段锁机制
   - 都采用异步处理模式

2. 架构设计
   - 都采用分层架构
   - 都实现了完整的RESTful API
   - 都包含完整的测试用例

3. 性能优化
   - 都实现了批量处理
   - 都使用了缓存机制
   - 都优化了内存使用

### 不同点
1. 并发控制
   - .NET使用ReaderWriterLockSlim，更接近底层
   - Node.js使用AsyncLock，更符合异步模型

2. 数据结构
   - .NET使用线程安全的ConcurrentDictionary
   - Node.js使用普通Map配合锁机制

3. 性能优化
   - .NET使用向量化计算
   - Node.js使用智能缓存策略

## 性能测试分析

### 测试方案
为了确保测试结果的可比性，使用统一的测试工具和测试环境：
- 测试工具：NBomber 性能测试工具
- 测试环境：本机
- 测试项目：/dotnet/LeaderBoard.API.Tests （nodejs也是用的这个项目做的测试）

### 测试场景
1. 更新分数场景 (100 RPS, 30秒)
2. 获取排行榜场景 (200 RPS, 30秒)
3. 获取用户排名场景 (100 RPS, 30秒)

### 测试结果分析

1. 更新分数场景
   - .NET版本平均延迟降低66%
   - 但最大延迟略高
   - 两个版本都保持100%成功率

2. 获取排行榜场景
   - .NET版本平均延迟降低61%
   - 数据传输量减少48%
   - 两个版本都保持100%成功率

3. 获取用户排名场景
   - .NET版本出现大量失败
   - Node.js版本保持100%成功率
   - 成功请求的平均延迟降低32%

### 发现与建议

1. 性能特点
   - .NET版本在正常负载下性能更好
   - Node.js版本在极端情况下更稳定
   - 两个版本都有优化空间

2. 稳定性特点
   - Node.js版本在所有场景都保持稳定
   - .NET版本在某些场景需要改进
   - 两个版本都需要更好的错误处理

3. 优化方向
   - 需要更详细的性能监控
   - 实现自适应限流机制
   - 优化错误处理策略

## 快速开始

### .NET版本

```bash
cd dotnet
dotnet restore
dotnet build
dotnet run --project LeaderBoard.API
dotnet run --project LeaderBoard.API.Tests
```

### Node.js版本

```bash
cd nodejs
yarn install
yarn build
yarn start
```

## API文档

详细的API文档请参考 `docs` 目录下的相关文档。
