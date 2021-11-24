module Styles

open Feliz
open Feliz.MaterialUI

type Props =
    {
        Page:string
        IsDrawerOpen:bool
        IsLoggedIn:bool
    } 
let darkTheme = Styles.createMuiTheme([
  theme.overrides.muiDrawer.paperAnchorLeft[
    style.width (length.rem 15)
  ]
  theme.overrides.muiPaper.elevation1[
     style.backgroundColor.white
     style.color.black
  ]
  theme.palette.type'.dark
  theme.typography.h1.fontSize "3rem"
  theme.typography.h2.fontSize "2rem"
  theme.typography.h3.fontSize "1.5rem"
  theme.props.muiAppBar [
    appBar.color.default'
  ]
])

let useStyles : Props -> _= Styles.makeStyles(fun styles darkTheme ->

  let drawerWidth = 240
  {|
    
    root = styles.create [
      style.display.flex
      style.flexGrow 1
    ]
    toolbar = styles.create [
        style.paddingRight 24
    ]
    toolbarIcon = styles.create [
        style.display.flex
        style.alignItems.center
        style.justifyContent.flexEnd
        style.padding (0,8)
    ]
    
    appBar = styles.create [
      darkTheme.zIndex.drawer + 1 |>  style.zIndex
      style.custom ("transition", "width 195ms cubic-bezier(0.4, 0, 0.6, 1) 0ms;")            
    ]
    appBarShift = styles.create[
        style.marginLeft drawerWidth
        style.custom ("width", "calc(100% - 240px);")
        style.custom ("transition", "width 195ms cubic-bezier(0.4, 0, 0.6, 1) 0ms,margin 195ms cubic-bezier(0.4, 0, 0.6, 1) 0ms;")
    ]

    menuButton = styles.create[
        style.marginRight 36
    ]
    menuButtonHidden = styles.create[
        style.display.none
    ]    
    title = styles.create [
        style.flexGrow 1
    ]

    drawer = styles.create [
      style.width (length.px 240)
      style.flexShrink 0
    ]
    drawerPaper = styles.create[
        style.width (length.px drawerWidth)
        style.position.relative
        style.whitespace.nowrap
        style.custom ("transition","width 225ms cubic-bezier(0.4, 0, 0.6, 1) 0ms; white-space: nowrap;")
    ]
    drawerPaperClose = styles.create[
        style.overflowX.hidden
        style.custom ("transition","width 195ms cubic-bezier(0.4, 0, 0.6, 1) 0ms;}")
        darkTheme.spacing 9 |> style.width
    ]
    appBarSpacer = styles.create[
        style.flexGrow 1
    ]
    content = styles.create[
        style.flexGrow 1
        style.custom ("height","100%")
        style.overflow.auto
    ]

    container = styles.create[
        darkTheme.spacing 8 |> style.paddingTop
        darkTheme.spacing 1 |> style.paddingBottom
    ]
    paper = styles.create[
      (darkTheme.spacing 2) |> style.padding
      style.textAlign.left
      style.flexGrow 1
    ]
    fixedHeight = styles.create[
        style.height 240 
    ]
  |}
)