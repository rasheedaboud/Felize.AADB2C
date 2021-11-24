module NavLinks

open Feliz
open Feliz.MaterialUI
open Feliz.Router
open Fable.Core.JsInterop
open Fable.MaterialUI.Icons
open Feliz.UseListener

[<ReactComponent>]
let NavLinks(props: {| isAdmin: bool |})  =

    let (showAdminMenu,setShowAdmin) = React.useState(false)

    Html.div[
        prop.children[
            if not showAdminMenu then
                Mui.listItem [
                    listItem.button true
                    prop.text "Projects"
                    prop.onClick(fun _ ->  Router.navigate("projects"))
                ]
                Mui.divider []              
                if props.isAdmin then
                    Mui.listItem [
                        listItem.button true
                        listItemIcon.children[
                            Mui.listItemText[
                                prop.text "Admin"
                            ] 
                            chevronRightIcon []
                        ]
                        prop.onClick(fun _ -> setShowAdmin(true))                                                       
                    ]

            else 
                Mui.divider []
                Mui.listItem [
                    listItem.button true
                    prop.text "Secret Area"
                    prop.onClick(fun _ -> Router.navigate("secret") )              
                ]                
                Mui.divider []                                                    
                Mui.listItem [
                    listItem.button true
                    listItemIcon.children[
                        chevronLeftIcon []
                        Mui.listItemText[
                            prop.text "Back"
                        ] 
                    ]
                    prop.onClick(fun _ -> setShowAdmin(false))                                                       
                ]
                Mui.divider []                  
        ]
    ]

exportDefault NavLinks