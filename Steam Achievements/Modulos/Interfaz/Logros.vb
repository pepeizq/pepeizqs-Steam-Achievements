Imports Windows.System

Namespace Interfaz
    Module Logros

        Public Async Sub Cargar()

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim botonAbrirTienda As Button = pagina.FindName("botonJuegoAbrirTienda")

            AddHandler botonAbrirTienda.Click, AddressOf AbrirEnlace
            AddHandler botonAbrirTienda.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_02
            AddHandler botonAbrirTienda.PointerExited, AddressOf EfectosHover.Sale_Boton_1_02

            Dim spLogrosBotonesAdicionales As StackPanel = pagina.FindName("spLogrosBotonesAdicionales")
            Dim spMensajeTrial As StackPanel = pagina.FindName("spMensajeTrial")

            If Await Trial.Detectar = False Then
                spLogrosBotonesAdicionales.Visibility = Visibility.Visible
                spMensajeTrial.Visibility = Visibility.Collapsed

                Dim botonJuegoAbrirGuias As Button = pagina.FindName("botonJuegoAbrirGuias")

                AddHandler botonJuegoAbrirGuias.Click, AddressOf AbrirEnlace
                AddHandler botonJuegoAbrirGuias.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_02
                AddHandler botonJuegoAbrirGuias.PointerExited, AddressOf EfectosHover.Sale_Boton_1_02

                Dim botonJuegoAbrirVideo As Button = pagina.FindName("botonJuegoAbrirVideo")

                AddHandler botonJuegoAbrirVideo.Click, AddressOf AbrirEnlace
                AddHandler botonJuegoAbrirVideo.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_02
                AddHandler botonJuegoAbrirVideo.PointerExited, AddressOf EfectosHover.Sale_Boton_1_02
            Else
                spLogrosBotonesAdicionales.Visibility = Visibility.Collapsed
                spMensajeTrial.Visibility = Visibility.Visible

                Dim botonComprarApp As Button = pagina.FindName("botonComprarApp")

                AddHandler botonComprarApp.Click, AddressOf Trial.ComprarAppClick
                AddHandler botonComprarApp.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_02
                AddHandler botonComprarApp.PointerExited, AddressOf EfectosHover.Sale_Boton_1_02
            End If

            Dim botonVolver As Button = pagina.FindName("botonVolverLogros")

            AddHandler botonVolver.Click, AddressOf Volver
            AddHandler botonVolver.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_05
            AddHandler botonVolver.PointerExited, AddressOf EfectosHover.Sale_Boton_1_05

        End Sub

        Private Async Sub AbrirEnlace(sender As Object, e As RoutedEventArgs)

            Dim boton As Button = sender
            Dim enlace As String = boton.Tag

            Await Launcher.LaunchUriAsync(New Uri(enlace))

        End Sub

        Private Sub Volver(sender As Object, e As RoutedEventArgs)

            Dim recursos As New Resources.ResourceLoader

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gridJuegoSeleccionadoLogro As Grid = pagina.FindName("gridJuegoSeleccionadoLogro")

            If gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed Then

                Dim gridJuegos As Grid = pagina.FindName("gridJuegos")
                Pestañas.Visibilidad(gridJuegos, recursos.GetString("Games"), Nothing)

                Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")
                Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
                nvLogros.Visibility = Visibility.Collapsed

                Dim spBuscador As StackPanel = pagina.FindName("spBuscador")
                spBuscador.Visibility = Visibility.Visible

            ElseIf gridJuegoSeleccionadoLogro.Visibility = Visibility.Visible Then

                gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed

                Dim gridJuegoSeleccionadoProgreso As Grid = pagina.FindName("gridJuegoSeleccionadoProgreso")
                gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible

                Dim botonJuegoAbrirGuias As Button = pagina.FindName("botonJuegoAbrirGuias")
                botonJuegoAbrirGuias.Visibility = Visibility.Visible

                Dim botonJuegoAbrirVideo As Button = pagina.FindName("botonJuegoAbrirVideo")
                botonJuegoAbrirVideo.Visibility = Visibility.Collapsed

                Dim sv As ScrollViewer = pagina.FindName("svLogrosJuego")
                sv.Visibility = Visibility.Visible

                Dim mel As MediaElement = pagina.FindName("meYoutube")
                mel.Stop()

                Dim spVideo As StackPanel = pagina.FindName("spLogroVideo")
                spVideo.Visibility = Visibility.Collapsed

            End If

        End Sub

    End Module
End Namespace