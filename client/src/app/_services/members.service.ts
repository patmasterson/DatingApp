import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    //console.log(this.baseUrl);
    if (this.members.length > 0) return of(this.members);
    
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map( mem => {
        this.members = mem;
        return mem;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName === username);
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + 'users/' + username );
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map( () => {
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index], ...member};
      })
    );
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
