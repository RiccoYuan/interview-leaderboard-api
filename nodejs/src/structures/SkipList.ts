import { CustomerNode } from '../models/CustomerNode';

class SkipListNode {
    public forward: (SkipListNode | null)[];
    
    constructor(
        public customer: CustomerNode | null,
        public level: number
    ) {
        // 预分配数组，避免动态扩容
        this.forward = new Array(level + 1).fill(null);
    }
}

export class SkipList {
    // 根据预期数据量动态调整最大层数
    static MAX_LEVEL = Math.ceil(Math.log2(1000000));
    private head: SkipListNode;
    private level: number;
    private size: number;
    private nodeMap: Map<CustomerNode, SkipListNode>;
    
    constructor() {
        this.head = new SkipListNode(null, SkipList.MAX_LEVEL);
        this.level = 0;
        this.size = 0;
        this.nodeMap = new Map();
        
        // 预初始化所有层的指针
        for (let i = 0; i < SkipList.MAX_LEVEL; i++) {
            this.head.forward[i] = null;
        }
    }
    
    private randomLevel(): number {
        // 优化随机层数生成算法
        let lvl = 1;
        const maxLevel = Math.min(SkipList.MAX_LEVEL, this.level + 1);
        
        while (Math.random() < 0.5 && lvl < maxLevel) {
            lvl++;
        }
        return lvl;
    }
    
    insert(customer: CustomerNode): void {
        // 如果节点已存在，先删除
        const existingNode = this.nodeMap.get(customer);
        if (existingNode) {
            this.delete(customer);
        }
        
        const update: (SkipListNode | null)[] = new Array(SkipList.MAX_LEVEL).fill(null);
        let current: SkipListNode | null = this.head;
        
        // 从最高层开始查找插入位置
        for (let i = this.level; i >= 0; i--) {
            while (
                current && current.forward[i] &&
                current.forward[i]!.customer &&
                current.forward[i]!.customer!.compareTo(customer) < 0
            ) {
                current = current.forward[i];
            }
            update[i] = current;
        }
        
        // 生成新节点的层数
        const lvl = this.randomLevel();
        if (lvl > this.level) {
            for (let i = this.level + 1; i <= lvl; i++) {
                update[i] = this.head;
            }
            this.level = lvl;
        }
        
        // 创建新节点
        const newNode = new SkipListNode(customer, lvl);
        this.nodeMap.set(customer, newNode);
        
        // 更新各层的指针
        for (let i = 0; i <= lvl; i++) {
            newNode.forward[i] = update[i]!.forward[i];
            update[i]!.forward[i] = newNode;
        }
        
        this.size++;
    }
    
    delete(customer: CustomerNode): boolean {
        const update: (SkipListNode | null)[] = new Array(SkipList.MAX_LEVEL).fill(null);
        let current: SkipListNode | null = this.head;
        
        // 从最高层开始查找要删除的节点
        for (let i = this.level; i >= 0; i--) {
            while (
                current && current.forward[i] &&
                current.forward[i]!.customer &&
                current.forward[i]!.customer!.compareTo(customer) < 0
            ) {
                current = current.forward[i];
            }
            update[i] = current;
        }
        
        current = current && current.forward[0] ? current.forward[0] : null;
        
        // 如果找到要删除的节点
        if (current && current.customer && current.customer.equals(customer)) {
            // 更新各层的指针
            for (let i = 0; i <= this.level; i++) {
                if (update[i] && update[i]!.forward[i] !== current) {
                    break;
                }
                if (update[i]) {
                    update[i]!.forward[i] = current.forward[i];
                }
            }
            
            // 更新跳表的层数
            while (this.level > 0 && !this.head.forward[this.level]) {
                this.level--;
            }
            
            this.nodeMap.delete(customer);
            this.size--;
            return true;
        }
        
        return false;
    }
    
    find(customer: CustomerNode): CustomerNode | null {
        // 使用nodeMap快速查找
        const node = this.nodeMap.get(customer);
        return node ? node.customer : null;
    }
    
    getRange(start: number, count: number): CustomerNode[] {
        // 预分配结果数组，避免动态扩容
        const result: CustomerNode[] = new Array(count);
        let current: SkipListNode | null = this.head.forward[0];
        let idx = 1;
        let resultIdx = 0;
        
        // 跳过前start个元素
        while (current && idx < start) {
            current = current.forward[0];
            idx++;
        }
        
        // 返回count个元素
        while (current && resultIdx < count) {
            if (current.customer) {
                result[resultIdx++] = current.customer;
            }
            current = current.forward[0];
        }
        
        // 如果结果数组未填满，截断数组
        return resultIdx === count ? result : result.slice(0, resultIdx);
    }
    
    getSize(): number {
        return this.size;
    }
    
    clear(): void {
        this.head = new SkipListNode(null, SkipList.MAX_LEVEL);
        this.level = 0;
        this.size = 0;
        this.nodeMap.clear();
        
        // 重新初始化所有层的指针
        for (let i = 0; i < SkipList.MAX_LEVEL; i++) {
            this.head.forward[i] = null;
        }
    }
} 