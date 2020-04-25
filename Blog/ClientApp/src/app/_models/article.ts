import { Comment } from '@angular/compiler';
import { Tag } from './tag';

export class Article {
    id: number;
    title: string;
    content: string;
    blogId: number;
    authorId: number;
    authorUsername: number;
    comments: Comment[];
    tags: Tag[]
    lastUpdated: Date
}
