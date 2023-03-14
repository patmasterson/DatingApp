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
            this.setCurrentUser(user);
          }
        })
      );
   }

   register(model: any) {
    console.log('register - account service');
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    )
   }

   setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
   }

   logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
   }

   getDecodedToken(token: string) {
    // atob has two overloads. first is deprecated.  second is not.  VSCode is showing deprecated because of first method
    return JSON.parse(atob(token.split('.')[1]));
   }
}
