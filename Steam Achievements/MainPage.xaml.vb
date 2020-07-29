Imports Microsoft.Toolkit.Uwp.Helpers
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

        meYoutube.Stop()
        meYoutube.Visibility = Visibility.Collapsed

        Dim logros As NavigationViewItem = nvPrincipal.MenuItems(2)
        Dim juegoTitulo As String = String.Empty

        If logros.Visibility = Visibility.Visible Then
            Dim juego As Juego = logros.Tag
            juegoTitulo = juego.Titulo
        End If

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

            ElseIf item.Text = juegoTitulo Then
                GridVisibilidad(gridLogros, item.Text)

                spBuscador.Visibility = Visibility.Collapsed

                gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible
                gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed
                svLogrosJuego.Visibility = Visibility.Visible
                gridJuegoSeleccionadoLogroControles.Visibility = Visibility.Collapsed

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

    Private Sub UsuarioEntraBotonRojo(sender As Object, e As PointerRoutedEventArgs)

        Dim boton As Button = sender
        Dim sp As StackPanel = boton.Content

        sp.Background = New SolidColorBrush("#892a2a".ToColor)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBotonRojo(sender As Object, e As PointerRoutedEventArgs)

        Dim boton As Button = sender
        Dim sp As StackPanel = boton.Content

        sp.Background = New SolidColorBrush("#c84d4d".ToColor)

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

End Class
