module Authorization

open Fable.Core
open Fable.Core.JsInterop
open Feliz
open System
open Fable.Core.JS

////////////////////////////////////
/// Extend this type with any additional claims you want to 
/// retireve from login request.
/// For example if you add custom claims in B2C you can add them here 
///  ex: extention_role:string
/// Then you can use this to add authorization in components.
/// ////////////////////////////////
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
  with 
    member this.IsAdmin =
        match this.given_name with
        | gn when String.IsNullOrEmpty(gn) |> not -> true
        | _ -> false       
    member this.DisplayName =
        $"{this.given_name.[0]}.{this.family_name}"

    static member Default() = {
        aud=""
        auth_time=""
        emails= [||]
        exp= 0
        family_name=""
        given_name=""
        iat=0
        iss=""
        nbf=0
        nonce=""
        sub=""
        tfp=""
        ver=""}

type account = {
  length:int
  environment: string
  homeAccountId: string
  idTokenClaims:IdTokenClaims
  localAccountId: string
  name: string
  tenantId: string
  username:string}
  with static member Deafult()= {
        length=0
        environment=""
        homeAccountId=""
        idTokenClaims=IdTokenClaims.Default()
        localAccountId=""
        name=""
        tenantId=""
        username=""}

type AccountInfo = {
    homeAccountId: string;
    environment: string;
    tenantId: string;
    username: string;
    localAccountId: string;
    name: string;
    idTokenClaims: obj;
};

///<summary>Used to request scopes when requesting token</summary>
type TokenRequest ={
  account:AccountInfo
  scopes:string[]
  forceRefresh:bool
}




type AuthenticationResult = {
    authority: string;
    uniqueId: string;
    tenantId: string;
    scopes: string[];
    account: AccountInfo option;
    idToken: string;
    idTokenClaims: obj;
    accessToken: string;
    fromCache: bool;
    expiresOn:DateTime option;
    tokenType: string;
    correlationId: string;
    extExpiresOn: DateTime;
    state: string;
    familyId: string;
    cloudGraphHostName: string;
    msGraphHost: string;
};


let msalProvider : obj = import "MsalProvider" "@azure/msal-react"
let authenticatedTemplate : obj = import "AuthenticatedTemplate"   "@azure/msal-react"
let unauthenticatedTemplate : obj = import "UnauthenticatedTemplate"   "@azure/msal-react"
let useIsAuthenticated() : bool= import "useIsAuthenticated" "@azure/msal-react"



[<Import("PublicClientApplication", from="@azure/msal-browser")>]
type PublicClientApplication (config:obj) =
    abstract member loginRedirect: request:obj -> Promise<unit>;
    default this.loginRedirect(request:obj) = jsNative  
    abstract member loginPopup: request:obj -> Promise<AuthenticationResult option>
    default this.loginPopup(request:obj) = jsNative
    abstract member logout: unit-> unit
    default this.logout() = jsNative
    abstract member getAllAccounts: unit-> account[] 
    default this.getAllAccounts() : account[] = jsNative
    abstract member acquireTokenSilent: request:obj -> Promise<AuthenticationResult option>;
    default this.acquireTokenSilent(request:obj) = jsNative
    abstract member getAccountByUsername:userName: string -> AccountInfo option
    default this.getAccountByUsername(userName:string) = jsNative

/// <summary>All components underneath MsalProvider will have access to the PublicClientApplication instance via 
/// context as well as all hooks and components provided by @azure/msal-react.
/// for more info see 
/// <seealso cref="https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md"/>
/// </summary>
/// <param name="instance">PublicClientApplication.</param>
type MsalProvider =
  static member inline instance (pca: obj) = "instance" ==> pca
  static member inline children (children: ReactElement list) = "children" ==> children
  static member inline create  props = Interop.reactApi.createElement (msalProvider, createObj !!props)


/// <summary>Used to show UI when user is authenticated
/// <seealso cref="https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md"/>
/// </summary>
type AuthenticatedTemplate =
  static member inline children (children: ReactElement list) = "children" ==> children
  static member inline create props = Interop.reactApi.createElement (authenticatedTemplate, createObj !!props)

/// <summary>Used to show UI when user is NOT authenticated
/// <seealso cref="https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md"/>
/// </summary>
type UnauthenticatedTemplate  =
  static member inline children (children: ReactElement list) = "children" ==> children
  static member inline create props = Interop.reactApi.createElement (unauthenticatedTemplate, createObj !!props)


//////////////////////////////////////////
/// CHANGE ME
/// MORE INFO https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant
/// There are a ton of possible options.
/// //////////////////////////////////////
let msalConfig ={|
    auth={|
          clientId="<clientId>"
          authority="https://<domain>.b2clogin.com/<domain>.onmicrosoft.com/<SingInFlow>"
          knownAuthorities=[|"https://<domain>.b2clogin.com"|]
          redirectUri= "https://localhost:8080/"
          postLogoutRedirectUri = "https://localhost:8080/"|};
    cache={|cacheLocation="sessionStorage"; storeAuthStateInCookie=false|}
  |}




let client = PublicClientApplication(msalConfig);