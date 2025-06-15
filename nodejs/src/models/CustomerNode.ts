export class CustomerNode {
    public score: number;
    public rankDirty: boolean;
    public lastUpdate: Date;
    public cachedRank: number;
    
    constructor(
        public customerId: number,
        initialScore: number = 0
    ) {
        this.score = initialScore;
        this.rankDirty = true;
        this.lastUpdate = new Date();
        this.cachedRank = 0;
    }
    
    compareTo(other: CustomerNode): number {
        if (this.score !== other.score) {
            return other.score - this.score; // 降序排列
        }
        return this.customerId - other.customerId; // 分数相同时按ID升序
    }
    
    equals(other: CustomerNode): boolean {
        return this.customerId === other.customerId;
    }
    
    updateRank(): void {
        this.rankDirty = false;
        this.lastUpdate = new Date();
    }

    toString(): string {
        return `CustomerNode(id=${this.customerId}, score=${this.score}, rank=${this.cachedRank})`;
    }
} 