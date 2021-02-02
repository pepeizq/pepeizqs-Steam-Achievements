Imports Windows.ApplicationModel.Core
Imports Windows.Storage
Imports Windows.System.Threading
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(Interfaz.NavigationViewItems.Generar(recursos.GetString("Accounts"), FontAwesome5.EFontAwesomeIcon.Brands_Steam))
        nvPrincipal.MenuItems.Add(Interfaz.NavigationViewItems.Generar(recursos.GetString("Games"), FontAwesome5.EFontAwesomeIcon.Solid_Gamepad))
        nvPrincipal.MenuItems.Add(Interfaz.NavigationViewItems.Generar(recursos.GetString("Achievements"), FontAwesome5.EFontAwesomeIcon.Solid_Trophy))
        nvPrincipal.MenuItems.Add(New NavigationViewItemSeparator)

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        meYoutube.Stop()
        spLogroVideo.Visibility = Visibility.Collapsed

        Dim logros As NavigationViewItem = nvPrincipal.MenuItems(2)
        Dim juegoTitulo As String = String.Empty

        If logros.Visibility = Visibility.Visible Then
            Dim juego As Juego = logros.Tag
            juegoTitulo = juego.Titulo
        End If

        Dim recursos As New Resources.ResourceLoader

        If TypeOf args.InvokedItem Is TextBlock Then
            Dim item As TextBlock = args.InvokedItem

            If Not item Is Nothing Then
                If item.Text = recursos.GetString("Accounts") Then
                    Interfaz.Pestañas.Visibilidad(gridCuentas, item.Text, item)

                    Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
                    nvJuegos.Visibility = Visibility.Collapsed

                    Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
                    nvLogros.Visibility = Visibility.Collapsed

                    spCuenta.Visibility = Visibility.Collapsed
                    imagenCuentaSeleccionada.Source = Nothing
                    tbCuentaSeleccionada.Text = String.Empty

                    spBuscador.Visibility = Visibility.Collapsed

                ElseIf item.Text = recursos.GetString("Games") Then
                    Interfaz.Pestañas.Visibilidad(gridJuegos, item.Text, item)

                    Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
                    nvLogros.Visibility = Visibility.Collapsed

                    spBuscador.Visibility = Visibility.Visible

                ElseIf item.Text = juegoTitulo Then
                    Interfaz.Pestañas.Visibilidad(gridLogros, item.Text, item)

                    spBuscador.Visibility = Visibility.Collapsed

                    gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible
                    gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed
                    svLogrosJuego.Visibility = Visibility.Visible
                End If
            End If
        End If

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        Dim recursos As New Resources.ResourceLoader

        Interfaz.Logros.Cargar()
        Steam.Cuentas.Cargar()
        Buscador.Cargar()
        MasSteam.Cargar()
        MasCosas.Cargar()
        Interfaz.Pestañas.Visibilidad(gridCuentas, recursos.GetString("Accounts"), Nothing)

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        nvJuegos.Visibility = Visibility.Collapsed

        Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
        nvLogros.Visibility = Visibility.Collapsed

        Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        If config.Values("Calificar_App") = 0 Then
            Dim periodoCalificar As TimeSpan = TimeSpan.FromSeconds(300)
            Dim contadorCalificar As ThreadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer(Async Sub()
                                                                                               Await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                                                                                                                                                MasCosas.CalificarApp(True)
                                                                                                                                                                                            End Sub)
                                                                                           End Sub, periodoCalificar)
        End If

    End Sub

End Class
