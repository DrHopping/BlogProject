import { Blog } from "./blog";
import { Comment } from "./comment";

export class User {
    id: number;
    username: string;
    email: string;
    avatarUrl: string;
    info: string;
    blogs: Blog[];
    role: string;
    comments: Comment[];
}
