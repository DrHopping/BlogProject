import { Article } from "./article";

export class Blog {
    id: number;
    name: string;
    ownerId: number;
    ownerUsername: string;
    description: string;
    articles: Article[];
}
