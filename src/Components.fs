module App

open Authorization
open Fable.Core.JsInterop
open Fable.MaterialUI.Icons
open Feliz
open Feliz.MaterialUI
open Feliz.Router
open Styles
open Browser.Dom

////////////////////////////
///  Imports
/// ////////////////////////
let Navlinks : Fable.Core.JS.Promise<unit -> ReactElement> = importDynamic "./NavLinks.fs"


////////////////////////////
///  User Info From Token
/// ////////////////////////
type User = {
    FirstName:string
    Lastname:string
    DisplayName:string
    IsAdmin:bool
}
    with 
        static member Default = {
            FirstName=""
            Lastname=""
            DisplayName=""
            IsAdmin=false
        }


////////////////////////////
///  Pull user info from token
/// ////////////////////////
let CreateUser (u:IdTokenClaims):User = {
    FirstName= u.given_name
    Lastname= u.family_name
    DisplayName =u.DisplayName
    IsAdmin = u.IsAdmin
}



type Components =
    [<ReactComponent>]
    static member Project() = 
        Html.div [
            Html.h1 [
                prop.text "Project"
            ]         
        ]
    [<ReactComponent>]
    static member Secret() = 
        Html.div [
            Html.h1 [
                prop.text "F# is fun!!"
            ]         
        ]

    [<ReactComponent>]
    static member NotFound() = 
        Html.div [
            Html.h1 [
                prop.text "Something went wrong if you're seeing this..."
            ]         
        ]


    [<ReactComponent>]
    static member Index(props:Props) = 
        let (token,setSetToken) = React.useState("")
        ///Grab a new token for any api calls.
        ///This is just an example. This should be pulled out into helper funtion and used
        ///before api calls etc.
        let aquireToken() =
            match client.getAccountByUsername("rasheedaboud@arcweldinginspection.com") with
            | Some account ->
                promise {               
                    let request = tokenRequest account

                    let! token =  client.acquireTokenSilent(request)
                    match token with
                    | Some result -> 
                        setSetToken result.accessToken
                        Browser.Dom.console.log(result.accessToken)
                        return () 
                    | None -> return ()               
                }
            | None -> failwith "Something went Wrong..."

        Html.div [
            UnauthenticatedTemplate.create [
                UnauthenticatedTemplate.children[
                    Html.h1 [
                        prop.text "Welcome to AADB2C using Felize!!"
                    ]
                    Html.p [
                        prop.text "Login to get started."
                    ] 
                ]

            ]
            AuthenticatedTemplate.create [
                AuthenticatedTemplate.children[
                    Html.h1 [
                        prop.text "Click below to get a token from auth provider!!"
                    ]
                    Mui.button[
                        button.color.primary   
                        prop.text "Aquire Token"
                        prop.onClick (fun _ -> aquireToken() |> ignore )
                    ]
                    Html.p [
                        prop.text token
                    ]
                ]

            ]        
        ]


    [<ReactComponent>]
    static member AppBar(props:Props) =  
        let appStyles = Styles.useStyles props
        let (isDrawerOpen, setOpen) = React.useState(false)
        let (currentUrl, updateCurrentUrl) = React.useState(Router.currentUrl())            
        let accounts = client.getAllAccounts()
        //MSAL React hook to check if user is authenticated.
        let isLoggedIn = useIsAuthenticated()
        let (currentUser,setUser) = React.useState(User.Default )

        let login() =
            client.loginPopup()
            |> Promise.map (fun response ->
                // Do something with IdToken or let msal log user in and use elmish or react hook
                //to create user object
                ()
            )
            ///with msal you have to catch login error check for forgot password code AADB2C90118
            /// Then redirect using forgot passowrd flow...
            |> Promise.catch(fun error -> 
                if forgotPassword error.Message then 
                    client.loginRedirect forgotPasswordRequest
                else
                    failwith error.Message
            )                   
            
        

        let createUser() =
            if isLoggedIn  
            then 
                setUser(CreateUser accounts.[0].idTokenClaims)

        React.useEffect( createUser,[| box isLoggedIn |])

        Mui.themeProvider [
            themeProvider.theme darkTheme
            themeProvider.children [
                Html.div[
                    prop.className [appStyles.root]
                    prop.children[
                        Mui.appBar [
                            appBar.color.default'
                            appBar.position.absolute
                            prop.className[
                                match isDrawerOpen with
                                | true -> appStyles.appBarShift
                                | false -> appStyles.appBar
                            ]
                            prop.children[
                                Mui.toolbar[
                                    prop.className [appStyles.toolbar]
                                    prop.children[
                                        AuthenticatedTemplate.create[
                                            AuthenticatedTemplate.children[
                                                Mui.iconButton [
                                                    iconButton.edge.start
                                                    iconButton.color.inherit'
                                                    prop.className [
                                                        match isDrawerOpen with
                                                        | true -> appStyles.menuButtonHidden
                                                        | false->appStyles.menuButton
                                                    ]
                                                    prop.onClick(fun _ ->  setOpen true )
                                                    iconButton.children [
                                                        menuIcon []                                                                                         
                                                    ]
                                                ]
                                            ]
                                        ]

                                        Mui.typography[
                                            typography.variant.h6
                                            prop.className appStyles.title
                                            prop.text "AADB2C"

                                        ]
                                        Html.div[
                                            AuthenticatedTemplate.create[
                                                AuthenticatedTemplate.children[
                                                    Mui.button[
                                                    button.color.inherit'
                                                    prop.text $"Welcome, {currentUser.DisplayName}!"                                                   
                                                    ]
                                                    Mui.button[
                                                    button.color.inherit'
                                                    prop.text "Log Out"
                                                    prop.onClick (fun _ -> client.logout()) 
                                                    ]
                                                ]
                                            ]
                                            UnauthenticatedTemplate.create[
                                                UnauthenticatedTemplate.children[
                                                    Mui.button[
                                                    button.color.inherit'
                                                    prop.text "Login"
                                                    prop.onClick (fun _ -> login() |> ignore )
                                                    ]
                                                ]
                                            ] 
                                        ]                                            
                                    ]
                                ]
                            ]
                        ]
                        Mui.drawer[
                            drawer.variant.temporary
                            drawer.open' isDrawerOpen                                                   
                            prop.children[
                                Html.div[
                                    prop.className appStyles.toolbarIcon
                                    prop.children[

                                        Mui.iconButton[                             
                                        prop.onClick(fun _ ->  setOpen false )
                                        iconButton.children[                                       
                                            chevronLeftIcon []                                                                                         
                                            ]
                                        ]
                                    ]
                                ]
                                Mui.divider []
                                React.suspense([React.lazy'(Navlinks,{|isAdmin=currentUser.IsAdmin|})])
                                Mui.divider []
                            ]
                        ]
                        Html.main [
                            prop.className appStyles.content
                            prop.children[
                                Html.div[
                                    prop.className appStyles.appBarSpacer
                                ]
                                Mui.container[
                                    container.maxWidth.false'
                                    prop.className appStyles.container
                                    prop.children[
                                        React.router [
                                            router.onUrlChanged updateCurrentUrl
                                            router.children [
                                                match currentUrl with
                                                | [ ] -> Components.Index(props)
                                                | [ "projects"] -> Components.Project()
                                                | ["secret"] -> Components.Secret()
                                                | _ -> Components.NotFound()
                                            ]
                                        ]                              
                                    ]
                                ]
                            ]
                        ]                 
                    ]
                ]
            ]
        ]