import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users: any;

  //httpClient is initialized and lets us make async calls.
  constructor(private http: HttpClient) {}

  //ngOnInit execute after data structures are established in the component.
  ngOnInit() {
    this.getUsers();
  }

  //this is our http request to the api we build in .net
  getUsers() {
    this.http.get('https://localhost:5001/api/users').subscribe(
      (res) => {
        this.users = res;
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
