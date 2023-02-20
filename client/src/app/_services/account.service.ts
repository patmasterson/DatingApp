import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { User } from '../_models/users';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  // union type <User | null>  can be either
  private currentUserSource = new BehaviorSubject<User | null>(null);
  // $ signifies an observable
  currentUser$ = this.currentUserSource.asObservable(); 

  constructor(private http: HttpClient) {

   }

   login(model: User) {
      return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
        map((response: User) => {
          const user = response;
          if (user) {
            localStorage.setItem('user', JSON.stringify(user))
            this.currentUserSource.next(user);
          }
        })
      );
   }

   register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        return user;
      })
    )
   }

   setCurrentUser(user: User) {
    this.currentUserSource.next(user);
   }

   logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
   }
}
