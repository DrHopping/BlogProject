import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-search-widget',
  templateUrl: './search-widget.component.html',
  styleUrls: ['./search-widget.component.css']
})
export class SearchWidgetComponent implements OnInit {

  value: string;

  constructor() { }

  ngOnInit(): void {
  }
}
