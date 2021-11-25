module Authorization

open Fable.Core
open Fable.Core.JsInterop
open Feliz
open System
open Fable.Core.JS
open System.Collections.Generic



let msalProvider : obj = import "MsalProvider" "@azure/msal-react"
let authenticatedTemplate : obj = import "AuthenticatedTemplate"   "@azure/msal-react"
let unauthenticatedTemplate : obj = import "UnauthenticatedTemplate"   "@azure/msal-react"



///<summary>The useIsAuthenticated hook returns a boolean indicating whether or not an account is signed in. 
/// It optionally accepts an accountIdentifier object you can provide if you need to know whether or not 
/// a specific account is signed in.</summary
let useIsAuthenticated() : bool= import "useIsAuthenticated" "@azure/msal-react"



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



type AccountInfo = {
    homeAccountId: string;
    environment: string;
    tenantId: string;
    username: string;
    localAccountId: string;
    name: string;
    idTokenClaims: IdTokenClaims;
};

type AccountIdentifiers = 
  | [<CompiledName("localAccount")>]LocalAccount of string
  | [<CompiledName("homeAccount")>]HomeAccount of string
  | [<CompiledName("username")>]Username of string

///<summary>The useAccount hook accepts an accountIdentifier parameter and returns the AccountInfo object for 
/// that account if it is signed in or null if it is not. You can read more about the AccountInfo object 
/// returned in the @azure/msal-browser docs here.</summary
/// <seealso cref="https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/login-user.md#account-apis"/>
let useAccount (identifier:AccountIdentifiers) : AccountInfo= import "useAccount " "@azure/msal-react"


///<summary>Used to request scopes when requesting token</summary>
type TokenRequest ={
  account:AccountInfo
  scopes:string[]
  forceRefresh:bool
}

type CommonSilentFlowRequest = {
    account: AccountInfo
    forceRefresh: bool
    tokenQueryParameters: Dictionary<string,string>;
}

type AuthenticationResult = {
    authority: string;
    uniqueId: string;
    tenantId: string;
    scopes: string[];
    account: AccountInfo option;
    idToken: string;
    idTokenClaims: IdTokenClaims;
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
}

type RedirectRequest = {
    account: AccountInfo
    postLogoutRedirectUri: string
}


type IPublicClientApplication = 
    abstract member loginRedirect: request:obj -> Promise<unit>;
    abstract member loginPopup: request:obj -> Promise<AuthenticationResult option>
    abstract member logout: unit-> unit
    abstract member logoutRedirect: request:RedirectRequest -> Promise<unit>
    abstract member getAllAccounts: unit-> AccountInfo[] 
    abstract member acquireTokenSilent: request:obj -> Promise<AuthenticationResult option>;
    abstract member getAccountByUsername:userName: string -> AccountInfo option

[<Import("PublicClientApplication", from="@azure/msal-browser")>]
type PublicClientApplication (config:obj) =
    interface IPublicClientApplication with
      member _.loginRedirect(request:obj) = jsNative  
      member _.loginPopup(request:obj) = jsNative
      member _.logout() = jsNative
      member _.logoutRedirect(request:RedirectRequest) = jsNative
      member _.getAllAccounts() : AccountInfo[] = jsNative
      member _.acquireTokenSilent(request:obj) = jsNative
      member _.getAccountByUsername(userName:string) = jsNative



type [<StringEnum>] [<RequireQualifiedAccess>] InteractionStatus =
    /// Initial status before interaction occurs
    | Startup
    /// Status set when all login calls occuring
    | Login
    /// Status set when logout call occuring
    | Logout
    /// Status set for acquireToken calls
    | AcquireToken
    /// Status set for ssoSilent calls
    | SsoSilent
    /// Status set when handleRedirect in progress
    | HandleRedirect
    /// Status set when interaction is complete
    | None

type IMsalContext =
    abstract member instance: IPublicClientApplication with get,set
    abstract member inProgress: InteractionStatus;
    abstract member accounts: AccountInfo[];

///<summary>The useAccount hook accepts an accountIdentifier parameter and returns the AccountInfo object for 
/// that account if it is signed in or null if it is not. You can read more about the AccountInfo object 
/// returned in the @azure/msal-browser docs here.</summary
/// <seealso cref="https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/login-user.md#account-apis"/>
let useMsal(): IMsalContext= import "useMsal " "@azure/msal-react"



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




let client = PublicClientApplication(msalConfig) :> IPublicClientApplication