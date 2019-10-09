Imports FontAwesome.UWP
Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyToolkit.Multimedia
Imports Newtonsoft.Json
Imports Windows.UI
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Accounts"), FontAwesomeIcon.Steam, 0))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Games"), FontAwesomeIcon.Gamepad, 1))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Achievements"), FontAwesomeIcon.Trophy, 2))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        Dim item As TextBlock = args.InvokedItem

        If Not item Is Nothing Then
            If item.Text = recursos.GetString("Accounts") Then
                GridVisibilidad(gridCuentas, item.Text)

                Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
                nvJuegos.Visibility = Visibility.Collapsed

                Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
                nvLogros.Visibility = Visibility.Collapsed

                imagenCuentaSeleccionada.Source = Nothing
                tbCuentaSeleccionada.Text = String.Empty

            ElseIf item.Text = recursos.GetString("Games") Then
                GridVisibilidad(gridJuegos, item.Text)

                Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
                nvLogros.Visibility = Visibility.Collapsed

            ElseIf item.Text = recursos.GetString("Achievements") Then
                GridVisibilidad(gridLogros, item.Text)
            End If
        End If

    End Sub

    Private Sub Nv_ItemFlyout(sender As NavigationViewItem, args As TappedRoutedEventArgs)

        FlyoutBase.ShowAttachedFlyout(sender)

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        MasCosas.Generar()

        Dim recursos As New Resources.ResourceLoader()

        GridVisibilidad(gridCuentas, recursos.GetString("Accounts"))
        nvPrincipal.IsPaneOpen = False

        Cuentas.CargarXaml()

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        nvJuegos.Visibility = Visibility.Collapsed

        Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
        nvLogros.Visibility = Visibility.Collapsed

    End Sub

    Private Sub GridVisibilidad(grid As Grid, tag As String)

        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + tag

        gridCuentas.Visibility = Visibility.Collapsed
        gridJuegos.Visibility = Visibility.Collapsed
        gridLogros.Visibility = Visibility.Collapsed
        gridPermisos.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

    'CUENTAS-----------------------------------------------

    Private Sub BotonAbrirPermisos_Click(sender As Object, e As RoutedEventArgs) Handles botonAbrirPermisos.Click

        gridPermisos.Visibility = Visibility.Visible

    End Sub

    Private Sub BotonVolverPermisos_Click(sender As Object, e As RoutedEventArgs) Handles botonVolverPermisos.Click

        gridPermisos.Visibility = Visibility.Collapsed

    End Sub

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

        lvLogros.Items.Clear()

        Dim recursos As New Resources.ResourceLoader()

        Dim sp As StackPanel = e.ClickedItem
        Dim cuenta As Cuenta = sp.Tag

        imagenCuentaSeleccionada.Source = cuenta.Respuesta.Jugador(0).Avatar
        tbCuentaSeleccionada.Text = cuenta.Respuesta.Jugador(0).Nombre

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        nvJuegos.Visibility = Visibility.Visible

        Dim tbToolTip As TextBlock = New TextBlock With {
            .Text = cuenta.Respuesta.Jugador(0).Nombre
        }

        ToolTipService.SetToolTip(nvJuegos, tbToolTip)
        nvJuegos.Tag = cuenta

        nvPrincipal.SelectedItem = nvJuegos

        Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
        nvLogros.Visibility = Visibility.Collapsed

        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed
        botonVolverListadoLogros.Visibility = Visibility.Collapsed

        lvLogros.Visibility = Visibility.Visible
        wvLogros.Visibility = Visibility.Collapsed

        GridVisibilidad(gridJuegos, cuenta.Respuesta.Jugador(0).Nombre)

        Juegos.Cargar(cuenta)

    End Sub

    Private Async Sub TbBuscarJuegos_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbBuscarJuegos.TextChanged

        gvJuegos.IsEnabled = False

        Dim helper As New LocalObjectStorageHelper
        Dim listaJuegosPrevia As List(Of Juego) = Nothing

        If Await helper.FileExistsAsync("listaJuegos") = True Then
            listaJuegosPrevia = Await helper.ReadFileAsync(Of List(Of Juego))("listaJuegos")
        Else
            listaJuegosPrevia = New List(Of Juego)
        End If

        Dim listaJuegosNueva As New List(Of Juego)

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
            .Foreground = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
            .Tag = recursos.GetString("Games")
        }

        nvLogros.Content = tb

        nvPrincipal.SelectedItem = nvLogros

        imagenJuegoSeleccionado.Source = New Uri(juego.Imagen)
        tbJuegoSeleccionado.Text = juego.Titulo

        gridLogros.Visibility = Visibility.Visible

        Dim transpariencia As New UISettings

        If transpariencia.AdvancedEffectsEnabled = True Then
            gridLogros.Background = App.Current.Resources("GridAcrilico")
        Else
            gridLogros.Background = New SolidColorBrush(Colors.LightGray)
        End If

        Dim helper As New LocalObjectStorageHelper
        Dim listaCuentas As List(Of Cuenta) = Nothing

        If Await helper.FileExistsAsync("listaCuentas2") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas2")
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
            Dim html2 As String = Await Decompiladores.HttpClient(New Uri("https://www.youtube.com/oembed?url=http://www.youtube.com/watch?v=" + id + "&format=json"))

            If Not html2 = Nothing Then
                Dim video As YouTube = JsonConvert.DeserializeObject(Of YouTube)(html2)

                If Not video Is Nothing Then
                    Dim html3 As String = video.Html
                    html3 = html3.Replace("width=" + ChrW(34) + "480", "width=" + ChrW(34) + "780")
                    html3 = html3.Replace("height=" + ChrW(34) + "270", "height=" + ChrW(34) + "485")

                    wvLogros.Visibility = Visibility.Visible
                    wvLogros.NavigateToString(html3)
                End If
            End If
        End If

    End Sub

    Private Sub BotonVolverListadoLogros_Click(sender As Object, e As RoutedEventArgs) Handles botonVolverListadoLogros.Click

        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed
        botonVolverListadoLogros.Visibility = Visibility.Collapsed

        lvLogros.Visibility = Visibility.Visible
        wvLogros.Visibility = Visibility.Collapsed

    End Sub

End Class
