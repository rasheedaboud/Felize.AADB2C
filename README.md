



# Feliz Template with AADB2C

This template gets you up and running with a simple web app using [Fable](http://fable.io/), [Elmish](https://fable-elmish.github.io/) and [Feliz](https://github.com/Zaid-Ajaj/Feliz).

Demonstrates login/out using F# and azure B2C. 

More information on msal react [here](https://www.npmjs.com/package/@azure/msal-react). 

## Requirements

* [dotnet SDK](https://www.microsoft.com/net/download/core) 5.0 or higher
* [node.js](https://nodejs.org) 10.0.0 or higher


## Running Sample
Before doing anything, follow these steps to setup B2C tennant etc. [Docs](https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).  

Next run `dotnet tool restore`.  

Then instal npm dependencies using `npm install`.  

Navigate to Auth.fs and update AADB2C configuration  

```
let msalConfig ={|
    auth={|
          clientId=""
          authority=""
          knownAuthorities=[|""|]
          redirectUri= "https://localhost:8080/"
          postLogoutRedirectUri = "https://localhost:8080/"|};
    cache={|cacheLocation="sessionStorage"; storeAuthStateInCookie=false|}
  |}
```

Then to start development mode with hot module reloading, run: `npm run`.

Try logging in.

## Supported Actions

For this sample only following methods are available on PublicClientApplication

```
[<Import("PublicClientApplication", from="@azure/msal-browser")>]
type PublicClientApplication (config:obj) =
    abstract member loginRedirect: request:obj -> Promise<unit>;
    abstract member loginPopup: request:obj -> Promise<AuthenticationResult option>
    abstract member logout: unit-> unit
    abstract member getAllAccounts: unit-> account[] 
    abstract member acquireTokenSilent: request:obj -> Promise<AuthenticationResult>;
    abstract member getAccountByUsername:userName: string -> AccountInfo option
```
Extend this as required.

To show or hide UI when user in authenticated or not use either of the following
```
AuthenticatedTemplate.create[
    AuthenticatedTemplate.children[

    ]
]

UnauthenticatedTemplate.create[
    UnauthenticatedTemplate.children[

    ]
] 
```
## Claims

In Auth.fs use type IdTokenClaims to retrive information about response from auth request. Initial model only has minimal claims faimily_name and given_name. Extend this as you see fit.
```
type IdTokenClaims =
  {
    aud: string
    auth_time: string
    emails: string[]
    exp: int
    family_name: string
    given_name: string
    iat: int
    iss: string
    nbf: int
    nonce: string
    sub: string
    tfp: string
    ver: string }
```

## Token
use `aquireTokenSilent(request:TokenRequest)` to try and get JWT Token from B2C. This can be used to make authenticated request to server.

use TokenRequest type to request specific scopes
```
type TokenRequest ={
  account:AccountInfo
  scopes:string[]
  forceRefresh:bool
}
```
AccountInfo can be aquired by calling `getAccountByUsername()` on PublicClientApplication instance.


## Hooks

the following react hook from msal react are supported;  

1. useAccount
2. useIsAuthenticated
3. useMsal
4. useMsalAuthentication
