# OAuth 2.0

A basic OAuth Server (with rate limiting) written in .NET Core. This solution created based on the tutorials https://0xnf.github.io/series/implement-an-oauth-2.0-server/ and https://kevinchalet.com/2016/07/13/creating-your-own-openid-connect-server-with-asos-introduction/.

## Generates tokens for a sample API running on the same server.
* GET api/v1/hello - Unauthenticated Method - Available to the public
* GET api/v1/clientcount - Authenticated Method - Scope: None
* GET api/v1/birthdate - Authenticated Method - Scope: user-read-birthdate
* GET api/v1/email - Authenticated Method - Scope: user-read-email
* PUT api/v1/birthdate - Authenticated Method - Scope: user-modify-birthdate
* PUT api/v1/email - Authenticated Method - Scope: user-modify-email

## Getting Tokens
### Authorization Code Flow
``` 
GET /authorize/?client_id={id}&response_type=code&redirect_uri={redirectUri}&scope={scopes}
```
```
POST /api/v1/token HTTP/1.1
Host: localhost:5001
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&code={authorizationCode}&redirect_uri={redirectUri}&client_id={id}
```

### Implicit Flow 
```
GET /authorize/?client_id={id}&response_type=token&redirect_uri={redirectUri}&scope={scopes}
```

### Client Credentials Flow
```
POST /api/v1/token HTTP/1.1
Host: localhost:5001
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials&redirect_uri={redirectUri}&client_id={id}&client_secret={secret}
```

### Proof Key For Code Exchange (PKCE)
```
GET /authorize/?scope={scopes}&response_type=code&client_id={id}&redirect_uri={redirectUri}&code_challenge={sha256HashOfCodeVerifier}&code_challenge_method=S256
```

```
POST /api/v1/token HTTP/1.1
Host: localhost:5001
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&code={code}&redirect_uri={redirectUri}&client_id={id}&code_verifier={codeVerifier}
```
