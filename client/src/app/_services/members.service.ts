import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMembers() {
    console.log(this.baseUrl);
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username );
  }

  // getHttpOptions() {
  //   const userString = localStorage.getItem('user');
  //   if (!userString) return;

  //   const user = JSON.parse(userString);
  //   console.log('token: ' + user.token);

  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: 'Bearer ' + user.token
  //     })
  //   }
  // }
}
