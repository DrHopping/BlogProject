import { Blog } from "./blog";
import { Comment } from "./comment";

export class User {
    Id: string;
    username: string;
    email: string;
    avatarUrl: string;
    info: string;
    blogs: Blog[]
    Comments: Comment[]
}
