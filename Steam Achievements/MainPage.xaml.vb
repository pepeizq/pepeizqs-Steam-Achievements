Imports Microsoft.Services.Store.Engagement
Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyToolkit.Multimedia
Imports Windows.ApplicationModel.Core
Imports Windows.System
Imports Windows.UI

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Accounts"), New SymbolIcon(Symbol.People), 0, Visibility.Visible))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Games"), New SymbolIcon(Symbol.Contact), 1, Visibility.Collapsed))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Achievements"), New SymbolIcon(Symbol.OtherUser), 2, Visibility.Collapsed))
        nvPrincipal.MenuItems.Add(New NavigationViewItemSeparator)
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("MoreThings"), New SymbolIcon(Symbol.More), 3, Visibility.Visible))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        Dim item As TextBlock = args.InvokedItem

        If item.Tag = recursos.GetString("Accounts") Then
            GridVisibilidad(gridCuentas, item.Text)

            Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
            nvJuegos.Visibility = Visibility.Collapsed

            Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
            nvLogros.Visibility = Visibility.Collapsed

        ElseIf item.Tag = recursos.GetString("Games") Then
            GridVisibilidad(gridJuegos, item.Text)

            Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
            nvLogros.Visibility = Visibility.Collapsed

        ElseIf item.Tag = recursos.GetString("Achievements") Then
            GridVisibilidad(gridLogros, item.Text)
        ElseIf item.Tag = recursos.GetString("MoreThings") Then
            GridVisibilidad(gridMasCosas, item.Text)
            NavegarMasCosas(lvMasCosasMasApps, "https://pepeizqapps.com/")
        End If

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        Dim coreBarra As CoreApplicationViewTitleBar = CoreApplication.GetCurrentView.TitleBar
        coreBarra.ExtendViewIntoTitleBar = True

        Dim barra As ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar
        barra.ButtonBackgroundColor = Colors.Transparent
        barra.ButtonForegroundColor = Colors.White
        barra.ButtonInactiveBackgroundColor = Colors.Transparent

        '--------------------------------------------------------

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        GridVisibilidad(gridCuentas, recursos.GetString("Accounts"))
        nvPrincipal.IsPaneOpen = False

        Cuentas.CargarXaml()

    End Sub

    Private Sub GridVisibilidad(grid As Grid, tag As String)

        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + tag

        gridCuentas.Visibility = Visibility.Collapsed
        gridJuegos.Visibility = Visibility.Collapsed
        gridLogros.Visibility = Visibility.Collapsed
        gridMasCosas.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    'CUENTAS-----------------------------------------------

    Private Sub TbUsuarioCuenta_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbUsuarioCuenta.TextChanged

        If tbUsuarioCuenta.Text.Length > 4 Then
            botonAgregarUsuario.IsEnabled = True
        Else
            botonAgregarUsuario.IsEnabled = False
        End If

    End Sub

    Private Sub BotonAgregarUsuario_Click(sender As Object, e As RoutedEventArgs) Handles botonAgregarUsuario.Click

        Cuentas.Añadir(tbUsuarioCuenta.Text)

    End Sub

    Private Sub LvUsuarios_ItemClick(sender As Object, e As ItemClickEventArgs) Handles lvUsuarios.ItemClick

        tbBuscarJuegos.Text = String.Empty

        panelMensajeLogros.Visibility = Visibility.Visible
        lvLogros.Items.Clear()

        Dim recursos As New Resources.ResourceLoader()

        Dim grid As Grid = e.ClickedItem
        Dim cuenta As Cuenta = grid.Tag

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        nvJuegos.Visibility = Visibility.Visible

        Dim tbToolTip As TextBlock = New TextBlock With {
            .Text = cuenta.Nombre
        }

        ToolTipService.SetToolTip(nvJuegos, tbToolTip)

        Dim tb As New TextBlock With {
            .Text = cuenta.Nombre,
            .Foreground = New SolidColorBrush(Colors.White),
            .Tag = recursos.GetString("Games")
        }

        nvJuegos.Content = tb
        nvJuegos.Tag = cuenta

        nvPrincipal.SelectedItem = nvJuegos

        Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
        nvLogros.Visibility = Visibility.Collapsed

        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed
        botonVolverListadoLogros.Visibility = Visibility.Collapsed

        lvLogros.Visibility = Visibility.Visible
        meLogros.Visibility = Visibility.Collapsed

        GridVisibilidad(gridJuegos, cuenta.Nombre)

        Juegos.Cargar(cuenta)

    End Sub

    Private Async Sub TbBuscarJuegos_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbBuscarJuegos.TextChanged

        gvJuegos.IsEnabled = False

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaJuegosPrevia As List(Of Juego) = Nothing

        If Await helper.FileExistsAsync("listaJuegos") = True Then
            listaJuegosPrevia = Await helper.ReadFileAsync(Of List(Of Juego))("listaJuegos")
        Else
            listaJuegosPrevia = New List(Of Juego)
        End If

        Dim listaJuegosNueva As List(Of Juego) = New List(Of Juego)

        If tbBuscarJuegos.Text.Length > 0 Then
            For Each juego In listaJuegosPrevia
                If juego.Titulo.ToLower.Contains(tbBuscarJuegos.Text.ToLower) = True Then
                    listaJuegosNueva.Add(juego)
                End If
            Next
        Else
            For Each juego In listaJuegosPrevia
                listaJuegosNueva.Add(juego)
            Next
        End If

        Juegos.CargarXaml(listaJuegosNueva)
        gvJuegos.IsEnabled = True

    End Sub

    Private Async Sub GvJuegos_ItemClick(sender As Object, e As ItemClickEventArgs) Handles gvJuegos.ItemClick

        Dim recursos As New Resources.ResourceLoader()

        Dim grid As Grid = e.ClickedItem
        Dim juego As Juego = grid.Tag

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        Dim cuenta As Cuenta = nvJuegos.Tag

        Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
        nvLogros.Visibility = Visibility.Visible

        Dim tbToolTip As TextBlock = New TextBlock With {
            .Text = juego.Titulo
        }

        ToolTipService.SetToolTip(nvLogros, tbToolTip)

        Dim tb As New TextBlock With {
            .Text = juego.Titulo,
            .Foreground = New SolidColorBrush(Colors.White),
            .Tag = recursos.GetString("Games")
        }

        nvLogros.Content = tb

        nvPrincipal.SelectedItem = nvLogros

        imagenJuegoSeleccionado.Source = New Uri(juego.Imagen)
        tbJuegoSeleccionado.Text = juego.Titulo

        GridVisibilidad(gridLogros, juego.Titulo)

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaCuentas As List(Of Cuenta) = Nothing

        If Await helper.FileExistsAsync("listaCuentas") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas")
        End If

        Logros.Cargar(cuenta, juego, listaCuentas)

    End Sub

    Private Async Sub LvLogros_ItemClick(sender As Object, e As ItemClickEventArgs) Handles lvLogros.ItemClick

        Dim grid As Grid = e.ClickedItem
        Dim logro As Logro = grid.Tag

        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Collapsed
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Visible
        botonVolverListadoLogros.Visibility = Visibility.Visible

        Try
            imagenJuegoSeleccionadoLogro.Source = logro.Imagen
            tbJuegoSeleccionadoLogro.Text = logro.Nombre
        Catch ex As Exception

        End Try

        lvLogros.Visibility = Visibility.Collapsed

        Dim cadenaBusqueda As String = logro.Juego.Titulo.Replace(" ", "+") + "+" + logro.Nombre.Replace(" ", "+")
        Dim html As String = Await HttpClient(New Uri("https://www.youtube.com/results?search_query=" + cadenaBusqueda))

        If html.Contains(ChrW(34) + "https://i.ytimg.com/") Then
            Dim temp, temp2 As String
            Dim int, int2 As Integer

            int = html.IndexOf(ChrW(34) + "https://i.ytimg.com/")
            temp = html.Remove(0, int + 1)

            int2 = temp.IndexOf("?")
            temp2 = temp.Remove(int2, temp.Length - int2)

            temp2 = temp2.Replace("https://i.ytimg.com/vi/", Nothing)
            temp2 = temp2.Replace("/hqdefault.jpg", Nothing)
            temp2 = temp2.Trim

            Dim id As String = temp2
            Dim video As YouTubeUri = Await YouTube.GetVideoUriAsync(id, YouTubeQuality.Quality1080P)

            melogros.visibility = Visibility.Visible
            Melogros.source = video.Uri
            melogros.play
        End If

    End Sub

    Private Sub BotonVolverListadoLogros_Click(sender As Object, e As RoutedEventArgs) Handles botonVolverListadoLogros.Click

        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed
        botonVolverListadoLogros.Visibility = Visibility.Collapsed

        lvLogros.Visibility = Visibility.Visible
        meLogros.Visibility = Visibility.Collapsed
        meLogros.Stop()

    End Sub

    'MASCOSAS-----------------------------------------

    Private Async Sub LvMasCosasItemClick(sender As Object, args As ItemClickEventArgs)

        Dim sp As StackPanel = args.ClickedItem

        If sp.Tag.ToString = 0 Then

            Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))

        ElseIf sp.Tag.ToString = 1 Then

            NavegarMasCosas(lvMasCosasMasApps, "https://pepeizqapps.com/")

        ElseIf sp.Tag.ToString = 2 Then

            NavegarMasCosas(lvMasCosasActualizaciones, "https://pepeizqapps.com/updates/")

        ElseIf sp.Tag.ToString = 3 Then

            NavegarMasCosas(lvMasCosasContacto, "https://pepeizqapps.com/contact/")

        ElseIf sp.Tag.ToString = 4 Then

            If StoreServicesFeedbackLauncher.IsSupported = True Then
                Dim ejecutador As StoreServicesFeedbackLauncher = StoreServicesFeedbackLauncher.GetDefault()
                Await ejecutador.LaunchAsync()
            Else
                NavegarMasCosas(lvMasCosasReportarFallo, "https://pepeizqapps.com/contact/")
            End If

        ElseIf sp.Tag.ToString = 6 Then

            NavegarMasCosas(lvMasCosasCodigoFuente, "https://github.com/pepeizq/Steam-Achievements")

        End If

    End Sub

    Private Sub NavegarMasCosas(lvItem As ListViewItem, url As String)

        lvMasCosasMasApps.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasActualizaciones.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasContacto.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasReportarFallo.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        lvMasCosasCodigoFuente.Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))

        lvItem.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))

        pbMasCosas.Visibility = Visibility.Visible

        wvMasCosas.Navigate(New Uri(url))

    End Sub

    Private Sub WvMasCosas_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs) Handles wvMasCosas.NavigationCompleted

        pbMasCosas.Visibility = Visibility.Collapsed

    End Sub

End Class
