import { Component, OnInit, Input, OnDestroy, Output, EventEmitter, AfterViewInit, AfterViewChecked } from '@angular/core';
import { AuthUser } from 'src/app/_models/authUser';
import { Comment } from 'src/app/_models/comment';
import { Subscription } from 'rxjs';
import { CurrentUserService } from 'src/app/_services/current-user.service';
import { CommentService } from 'src/app/_services/comment.service';

@Component({
  selector: 'app-comment-list',
  templateUrl: './comment-list.component.html',
  styleUrls: ['./comment-list.component.css']
})
export class CommentListComponent implements OnInit, OnDestroy {

  private sub: Subscription = new Subscription();


  @Input() comments: Comment[];
  @Output() onDelete = new EventEmitter<number>();

  user: AuthUser;

  constructor(private currentUserService: CurrentUserService, private commentService: CommentService) { }

  ngOnInit(): void {
    this.sub = this.currentUserService.currentUser$.subscribe(u => this.user = u);
  }

  onCommentDelete(id: number) {
    this.commentService.deleteComment(id)
      .subscribe(
        data => {
          this.onDelete.emit(id);
        }
      );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
