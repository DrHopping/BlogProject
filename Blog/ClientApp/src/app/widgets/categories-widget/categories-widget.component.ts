import { Component, OnInit, OnDestroy } from '@angular/core';
import { TagService } from 'src/app/_services/tag.service';
import { Tag } from 'src/app/_models/tag';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-categories-widget',
  templateUrl: './categories-widget.component.html',
  styleUrls: ['./categories-widget.component.css']
})
export class CategoriesWidgetComponent implements OnInit, OnDestroy {

  private sub: Subscription = new Subscription();

  tags: Tag[]

  constructor(private tagService: TagService) { }

  ngOnInit(): void {
    this.sub = this.tagService.getTopTags().subscribe(t => this.tags = t.slice(0, 9));
  }


  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
