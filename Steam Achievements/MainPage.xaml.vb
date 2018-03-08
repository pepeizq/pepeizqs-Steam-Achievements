Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyToolkit.Multimedia
Imports Windows.UI
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Accounts"), New SymbolIcon(Symbol.People), 0, Visibility.Visible))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Games"), New SymbolIcon(Symbol.Contact), 1, Visibility.Collapsed))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Achievements"), New SymbolIcon(Symbol.OtherUser), 2, Visibility.Collapsed))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        Dim item As TextBlock = args.InvokedItem

        If item.Text = recursos.GetString("Accounts") Then
            GridVisibilidad(gridCuentas, item.Text)

            Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
            nvJuegos.Visibility = Visibility.Collapsed

            Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
            nvLogros.Visibility = Visibility.Collapsed

        ElseIf item.Text = recursos.GetString("Games") Then
            GridVisibilidad(gridJuegos, item.Text)

            Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
            nvLogros.Visibility = Visibility.Collapsed

        ElseIf item.Text = recursos.GetString("Achievements") Then
            GridVisibilidad(gridLogros, item.Text)
        End If

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        MasCosas.Generar()

        Dim recursos As New Resources.ResourceLoader()

        GridVisibilidad(gridCuentas, recursos.GetString("Accounts"))
        nvPrincipal.IsPaneOpen = False

        Cuentas.CargarXaml()

        '--------------------------------------------------------

        Dim transpariencia As New UISettings
        TransparienciaEfectosFinal(transpariencia.AdvancedEffectsEnabled)
        AddHandler transpariencia.AdvancedEffectsEnabledChanged, AddressOf TransparienciaEfectosCambia

    End Sub

    Private Sub TransparienciaEfectosCambia(sender As UISettings, e As Object)

        TransparienciaEfectosFinal(sender.AdvancedEffectsEnabled)

    End Sub

    Private Async Sub TransparienciaEfectosFinal(estado As Boolean)

        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If estado = True Then
                                                                       gridMasCosas.Background = App.Current.Resources("GridAcrilico")
                                                                   Else
                                                                       gridMasCosas.Background = New SolidColorBrush(Colors.LightGray)
                                                                   End If
                                                               End Sub)

    End Sub

    Private Sub GridVisibilidad(grid As Grid, tag As String)

        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + tag

        gridCuentas.Visibility = Visibility.Collapsed
        gridJuegos.Visibility = Visibility.Collapsed
        gridLogros.Visibility = Visibility.Collapsed
        gridMasCosas.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

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

            meLogros.Visibility = Visibility.Visible
            meLogros.Source = video.Uri
            meLogros.Play()
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

End Class
