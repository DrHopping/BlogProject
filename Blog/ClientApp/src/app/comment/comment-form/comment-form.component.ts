import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CommentService } from 'src/app/_services/comment.service';

@Component({
  selector: 'app-comment-form',
  templateUrl: './comment-form.component.html',
  styleUrls: ['./comment-form.component.css']
})
export class CommentFormComponent implements OnInit {

  @Input() articleId: number;
  @Output() onPost = new EventEmitter<Comment>();
  content: string;

  constructor(private commentService: CommentService) { }

  ngOnInit(): void {
  }

  onSubmit() {
    if (!this.content || this.content.length <= 0) return;
    this.commentService.postComment(this.content, this.articleId)
      .subscribe(
        data => {
          this.onPost.emit(data);
        }
      );
  }

}
