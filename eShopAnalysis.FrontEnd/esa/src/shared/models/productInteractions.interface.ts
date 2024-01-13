export enum LikeStatus {
    Liked = 0,
    Neutral = 1,
    Disliked = 2
}

export interface ILikeProduct {
    likeId: string;
    userId: string;
    productBusinessKey: string;
    // likeStatus: LikeStatus;
    status: LikeStatus;
}

export interface IRateProduct {
    rateId: string;
    userId: string;
    productBusinessKey: string;
    rating: number;
}

export interface IBookmarkProduct {
    bookmarkId: string;
    userId: string;
    productBusinessKey: string;
}