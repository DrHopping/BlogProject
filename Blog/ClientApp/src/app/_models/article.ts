import { Tag } from './tag';
import { Comment } from './comment';


export class Article {
    id: number;
    title: string;
    content: string;
    blogId: number;
    blogName: string;
    authorId: number;
    authorUsername: string;
    imageUrl: string;
    comments: Comment[];
    tags: Tag[];
    lastUpdated: Date;
}
