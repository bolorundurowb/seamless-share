import { Component } from '@angular/core';

@Component({
  selector: 'ss-auth',
  standalone: false,
  template: `
    <div class="auth-page">
      <div class="logo">
        <img src="/images/logo.jpg" alt="App Logo">
      </div>

      <div class="auth-container">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: `
    .auth-page {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      height: 100vh;
      background-color: #f5f5f5;
      padding-bottom: 10rem;

      .logo {
        margin-bottom: 2rem;

        img {
          width: 5rem;
          height: auto;
        }
      }

      .auth-container {
        width: 100%;
        max-width: 30rem;
        padding: 2rem 1.5rem;
        border: 1px solid #ddd;
        border-radius: 0.5rem;
        background-color: #fff;

        @media (max-width: 480px) {
          width: 95%;
        }
      }
    }
  `
})
export class AuthPage {
}
