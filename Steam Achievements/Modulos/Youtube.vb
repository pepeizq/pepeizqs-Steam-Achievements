Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports VideoLibrary
Imports Windows.System

Module Youtube

    Public Async Sub Cargar(juego As Juego, logro As Logro)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridJuegoSeleccionadoProgreso As Grid = pagina.FindName("gridJuegoSeleccionadoProgreso")
        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Collapsed

        Dim gridJuegoSeleccionadoLogro As Grid = pagina.FindName("gridJuegoSeleccionadoLogro")
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Visible

        Dim imagenJuegoSeleccionadoLogro As ImageEx = pagina.FindName("imagenJuegoSeleccionadoLogro")
        imagenJuegoSeleccionadoLogro.Source = logro.Imagen

        Dim tbJuegoSeleccionadoLogro As TextBlock = pagina.FindName("tbJuegoSeleccionadoLogro")
        tbJuegoSeleccionadoLogro.Text = logro.Nombre

        Dim gridJuegoSeleccionadoLogroControles As Grid = pagina.FindName("gridJuegoSeleccionadoLogroControles")
        gridJuegoSeleccionadoLogroControles.Visibility = Visibility.Visible

        Dim botonVolver As Button = pagina.FindName("botonVolverListadoLogros")

        RemoveHandler botonVolver.Click, AddressOf Volver
        AddHandler botonVolver.Click, AddressOf Volver

        Dim sv As ScrollViewer = pagina.FindName("svLogrosJuego")
        sv.Visibility = Visibility.Collapsed

        Dim mel As MediaElement = pagina.FindName("meYoutube")
        mel.Visibility = Visibility.Visible
        mel.Source = Nothing

        Dim cadenaBusqueda As String = juego.Titulo + " achievement " + logro.Nombre

        Dim htmlBusqueda As String = Await Decompiladores.HttpClient(New Uri("https://www.googleapis.com/youtube/v3/search?part=snippet&q=" + cadenaBusqueda + "&type=video&order=relevance&key=AIzaSyADBmNBeOc0PKACOJgjYL2aX_fpe0kLIbQ"))
        Dim enlaceYoutube As String = String.Empty

        If Not htmlBusqueda = String.Empty Then
            Dim videos As VideosYoutube = JsonConvert.DeserializeObject(Of VideosYoutube)(htmlBusqueda)

            If Not videos Is Nothing Then
                If videos.Resultados.Count > 0 Then
                    enlaceYoutube = "https://www.youtube.com/watch?v=" + videos.Resultados(0).ID.VideoID

                    Dim libreria As VideoLibrary.YouTube = VideoLibrary.YouTube.Default
                    Dim resultados As IEnumerable(Of YouTubeVideo) = libreria.GetAllVideos("https://www.youtube.com/watch?v=" + videos.Resultados(0).ID.VideoID)
                    Dim enlaceMel As String = String.Empty
                    Dim resolucion As Integer = 0

                    For Each resultado In resultados
                        If resultado.Resolution > resolucion Then
                            resolucion = resultado.Resolution

                            enlaceMel = resultado.Uri

                            If resolucion >= 720 Then
                                Exit For
                            End If
                        End If
                    Next

                    mel.Source = New Uri(enlaceMel)
                    mel.Play()
                End If
            End If
        End If

        Dim botonAbrir As Button = pagina.FindName("botonAbrirVideoYoutube")
        botonAbrir.Tag = enlaceYoutube

        RemoveHandler botonAbrir.Click, AddressOf AbrirVideo
        AddHandler botonAbrir.Click, AddressOf AbrirVideo

    End Sub

    Private Sub Volver(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridJuegoSeleccionadoProgreso As Grid = pagina.FindName("gridJuegoSeleccionadoProgreso")
        gridJuegoSeleccionadoProgreso.Visibility = Visibility.Visible

        Dim gridJuegoSeleccionadoLogro As Grid = pagina.FindName("gridJuegoSeleccionadoLogro")
        gridJuegoSeleccionadoLogro.Visibility = Visibility.Collapsed

        Dim gridJuegoSeleccionadoLogroControles As Grid = pagina.FindName("gridJuegoSeleccionadoLogroControles")
        gridJuegoSeleccionadoLogroControles.Visibility = Visibility.Collapsed

        Dim sv As ScrollViewer = pagina.FindName("svLogrosJuego")
        sv.Visibility = Visibility.Visible

        Dim mel As MediaElement = pagina.FindName("meYoutube")
        mel.Visibility = Visibility.Collapsed
        mel.Stop()

    End Sub

    Private Async Sub AbrirVideo(sender As Object, e As RoutedEventArgs)

        Dim boton As Button = sender
        Dim enlace As String = boton.Tag

        Await Launcher.LaunchUriAsync(New Uri(enlace))

    End Sub

End Module
