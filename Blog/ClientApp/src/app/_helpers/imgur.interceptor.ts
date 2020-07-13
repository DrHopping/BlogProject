import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';


@Injectable()
export class ImgurInterceptor implements HttpInterceptor {
    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add auth header with jwt if user is logged in and request is to the api url
        const isImgurUrl = request.url.startsWith('https://api.imgur.com');
        if (isImgurUrl) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Client-ID ${environment.imgurClientId}`
                }
            });
        }
        return next.handle(request);
    }
}