Imports FontAwesome.UWP
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Newtonsoft.Json
Imports Windows.ApplicationModel.Core
Imports Windows.UI
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Accounts"), FontAwesome5.EFontAwesomeIcon.Brands_Steam, 0))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Games"), FontAwesome5.EFontAwesomeIcon.Solid_Gamepad, 1))
        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Achievements"), FontAwesome5.EFontAwesomeIcon.Solid_Trophy, 2))
        nvPrincipal.MenuItems.Add(New NavigationViewItemSeparator)
        nvPrincipal.MenuItems.Add(MasCosas.Generar("https://github.com/pepeizq/Steam-Achievements", "https://poeditor.com/join/project/KTLlr1dy7d"))

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

                spCuenta.Visibility = Visibility.Collapsed
                imagenCuentaSeleccionada.Source = Nothing
                tbCuentaSeleccionada.Text = String.Empty

                spBuscador.Visibility = Visibility.Collapsed

            ElseIf item.Text = recursos.GetString("Games") Then
                GridVisibilidad(gridJuegos, item.Text)

                Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
                nvLogros.Visibility = Visibility.Collapsed

                spBuscador.Visibility = Visibility.Visible

            ElseIf item.Text = recursos.GetString("Achievements") Then
                GridVisibilidad(gridLogros, item.Text)

                spBuscador.Visibility = Visibility.Collapsed

            ElseIf item.Text = recursos.GetString("MoreThings") Then
                FlyoutBase.ShowAttachedFlyout(nvPrincipal.MenuItems.Item(nvPrincipal.MenuItems.Count - 1))
            End If
        End If

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        tbTitulo.Text = Package.Current.DisplayName

        Dim coreBarra As CoreApplicationViewTitleBar = CoreApplication.GetCurrentView.TitleBar
        coreBarra.ExtendViewIntoTitleBar = True

        Dim barra As ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar
        barra.ButtonBackgroundColor = Colors.Transparent
        barra.ButtonForegroundColor = Colors.White
        barra.ButtonInactiveBackgroundColor = Colors.Transparent
        barra.ButtonInactiveForegroundColor = Colors.White

        '---------------------------------------------

        Dim recursos As New Resources.ResourceLoader()

        GridVisibilidad(gridCuentas, recursos.GetString("Accounts"))

        Buscador.Cargar()
        Cuentas.BotonEstilo()

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        nvJuegos.Visibility = Visibility.Collapsed

        Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
        nvLogros.Visibility = Visibility.Collapsed

    End Sub

    Public Sub GridVisibilidad(grid As Grid, tag As String)

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

        'Dim cadenaBusqueda As String = logro.Juego.Titulo.Replace(" ", "+") + "+" + logro.Nombre.Replace(" ", "+")
        'Dim html As String = Await HttpClient(New Uri("https://www.youtube.com/results?search_query=" + cadenaBusqueda))

        'If html.Contains(ChrW(34) + "https://i.ytimg.com/") Then
        '    Dim temp, temp2 As String
        '    Dim int, int2 As Integer

        '    int = html.IndexOf(ChrW(34) + "https://i.ytimg.com/")
        '    temp = html.Remove(0, int + 1)

        '    int2 = temp.IndexOf("?")
        '    temp2 = temp.Remove(int2, temp.Length - int2)

        '    temp2 = temp2.Replace("https://i.ytimg.com/vi/", Nothing)
        '    temp2 = temp2.Replace("/hqdefault.jpg", Nothing)
        '    temp2 = temp2.Trim

        '    Dim id As String = temp2
        '    Dim html2 As String = Await Decompiladores.HttpClient(New Uri("https://www.youtube.com/oembed?url=http://www.youtube.com/watch?v=" + id + "&format=json"))

        '    If Not html2 = Nothing Then
        '        Dim video As YouTube = JsonConvert.DeserializeObject(Of YouTube)(html2)

        '        If Not video Is Nothing Then
        '            Dim html3 As String = video.Html
        '            html3 = html3.Replace("width=" + ChrW(34) + "480", "width=" + ChrW(34) + "780")
        '            html3 = html3.Replace("height=" + ChrW(34) + "270", "height=" + ChrW(34) + "485")

        '            wvLogros.Visibility = Visibility.Visible
        '            wvLogros.NavigateToString(html3)
        '        End If
        '    End If
        'End If

    End Sub

    Private Sub BotonVolverListadoLogros_Click(sender As Object, e As RoutedEventArgs) Handles botonVolverListadoLogros.Click

        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed
        botonVolverListadoLogros.Visibility = Visibility.Collapsed

        lvLogros.Visibility = Visibility.Visible
        wvLogros.Visibility = Visibility.Collapsed

    End Sub

End Class
