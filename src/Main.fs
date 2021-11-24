module Main

open Feliz
open App
open Browser.Dom
open Fable.Core.JsInterop
open Styles
open Authorization


importAll "../node_modules/@azure/msal-common"
importAll "../node_modules/@azure/msal-browser"
importAll "../node_modules/@azure/msal-react"
importSideEffects "./styles/global.scss"

let props =  {
        Page = ""
        IsDrawerOpen=false
        IsLoggedIn=false
    }


[<ReactComponent>]
let App() =
    MsalProvider.create[
        MsalProvider.instance client
        MsalProvider.children[
            Components.AppBar(props)
        ]
    ]
    


ReactDOM.render(App(), document.getElementById "feliz-app")