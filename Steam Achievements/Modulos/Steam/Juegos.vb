Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.UI

Namespace Steam
    Module Juegos

        Public anchoColumna As Integer = 200
        Dim dominioImagenes As String = "https://cdn.cloudflare.steamstatic.com"

        Dim listaJuegos As New List(Of Juego)
        Dim listaJuegosOcultos As New List(Of JuegoOculto)

        Public Async Sub Cargar(cuenta As Cuenta)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gvJuegos As GridView = pagina.FindName("gvJuegos")
            gvJuegos.Items.Clear()

            Dim pr As ProgressRing = pagina.FindName("prJuegos")
            pr.Visibility = Visibility.Visible

            Dim spBuscador As StackPanel = pagina.FindName("spBuscador")
            spBuscador.Visibility = Visibility.Collapsed

            Dim helper As New LocalObjectStorageHelper

            listaJuegos.Clear()
            If Await helper.FileExistsAsync("listaJuegos" + cuenta.ID64) = True Then
                listaJuegos = Await helper.ReadFileAsync(Of List(Of Juego))("listaJuegos" + cuenta.ID64)
            End If

            listaJuegosOcultos.Clear()
            If Await helper.FileExistsAsync("listaJuegosOcultos" + cuenta.ID64) = True Then
                listaJuegosOcultos = Await helper.ReadFileAsync(Of List(Of JuegoOculto))("listaJuegosOcultos" + cuenta.ID64)
            End If

            Dim htmlJuegos As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + cuenta.ID64 + "&include_appinfo=1&include_played_free_games=1"))

            If Not htmlJuegos = Nothing Then
                Dim juegos As SteamJuegos = JsonConvert.DeserializeObject(Of SteamJuegos)(htmlJuegos)

                If Not juegos Is Nothing Then
                    For Each juego In juegos.Respuesta.Juegos
                        Dim imagen As String = dominioImagenes + "/steam/apps/" + juego.ID + "/library_600x900.jpg"

                        Dim icono As String = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/" + juego.ID + "/" + juego.Icono + ".jpg"

                        Dim juego2 As New Juego(juego.ID, juego.Titulo, imagen, icono, New List(Of Logro))

                        Dim añadir As Boolean = True
                        Dim k As Integer = 0
                        While k < listaJuegos.Count
                            If listaJuegos(k).ID = juego2.ID Then
                                añadir = False
                            End If
                            k += 1
                        End While

                        If añadir = True Then
                            listaJuegos.Add(juego2)
                        End If
                    Next
                End If
            End If

            If Not listaJuegos Is Nothing Then
                If listaJuegos.Count > 0 Then
                    listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))
                    Await helper.SaveFileAsync(Of List(Of Juego))("listaJuegos" + cuenta.ID64, listaJuegos)

                    Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")
                    gv.Items.Clear()

                    For Each juego In listaJuegos
                        Dim añadir As Boolean = True

                        If Not listaJuegosOcultos Is Nothing Then
                            If listaJuegosOcultos.Count > 0 Then
                                For Each oculto In listaJuegosOcultos
                                    If oculto.ID = juego.ID Then
                                        añadir = False
                                    End If
                                Next
                            End If
                        End If

                        If añadir = True Then
                            BotonEstilo(juego, gv)
                        End If
                    Next

                    spBuscador.Visibility = Visibility.Visible
                End If
            End If

            pr.Visibility = Visibility.Collapsed

        End Sub

        Public Sub BotonEstilo(juego As Juego, gv As AdaptiveGridView)

            Dim sp As New StackPanel With {
                .Orientation = Orientation.Vertical,
                .Tag = juego
            }

            Dim panel As New DropShadowPanel With {
                .Margin = New Thickness(10, 10, 10, 10),
                .ShadowOpacity = 0.9,
                .BlurRadius = 10,
                .MaxWidth = anchoColumna + 20,
                .HorizontalAlignment = HorizontalAlignment.Center,
                .VerticalAlignment = VerticalAlignment.Center,
                .Tag = juego
            }

            Dim boton As New Button

            Dim imagen As New ImageEx With {
                .Source = juego.Imagen,
                .IsCacheEnabled = True,
                .Stretch = Stretch.Uniform,
                .Padding = New Thickness(0, 0, 0, 0),
                .HorizontalAlignment = HorizontalAlignment.Center,
                .VerticalAlignment = VerticalAlignment.Center,
                .Tag = juego,
                .EnableLazyLoading = True
            }

            AddHandler imagen.ImageExOpened, AddressOf ImagenCarga
            AddHandler imagen.ImageExFailed, AddressOf ImagenFalla

            boton.Tag = juego
            boton.Content = imagen
            boton.Padding = New Thickness(0, 0, 0, 0)
            boton.Background = New SolidColorBrush(Colors.Transparent)

            panel.Content = boton

            Dim tbToolTip As TextBlock = New TextBlock With {
                .Text = juego.Titulo,
                .FontSize = 16,
                .TextWrapping = TextWrapping.Wrap
            }

            ToolTipService.SetToolTip(boton, tbToolTip)
            ToolTipService.SetPlacement(boton, PlacementMode.Mouse)

            AddHandler boton.Click, AddressOf AbrirLogros
            AddHandler boton.PointerEntered, AddressOf Interfaz.Entra_Boton_Imagen
            AddHandler boton.PointerExited, AddressOf Interfaz.Sale_Boton_Imagen

            sp.Children.Add(panel)

            '-----------------------------------------------------

            Dim tbLogros As New TextBlock With {
                .HorizontalAlignment = HorizontalAlignment.Right,
                .Foreground = New SolidColorBrush(Colors.White),
                .FontSize = 15,
                .Margin = New Thickness(0, 2, 10, 15)
            }

            If juego.Logros Is Nothing Then
                tbLogros.Text = String.Empty
            Else
                If juego.Logros.Count = 0 Then
                    tbLogros.Text = String.Empty
                Else
                    Dim contadorLogrosTerminados As Integer = 0

                    For Each logro In juego.Logros
                        If logro.Estado = "1" Then
                            contadorLogrosTerminados += 1
                        End If
                    Next

                    tbLogros.Text = contadorLogrosTerminados.ToString + "/" + juego.Logros.Count.ToString + " • " + (CInt((100 / juego.Logros.Count) * contadorLogrosTerminados)).ToString + "%"
                End If
            End If

            sp.Children.Add(tbLogros)

            gv.Items.Add(sp)

        End Sub

        Private Async Sub AbrirLogros(sender As Object, e As RoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim recursos As New Resources.ResourceLoader()

            Dim boton As Button = sender
            Dim imagen As ImageEx = boton.Content
            Dim juego As Juego = boton.Tag

            Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

            Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
            Dim cuenta As Cuenta = nvJuegos.Tag

            Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
            nvLogros.Visibility = Visibility.Visible
            nvLogros.Tag = juego

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

            Dim gridLogros As Grid = pagina.FindName("gridLogros")
            Interfaz.Pestañas.Visibilidad_Pestañas(gridLogros, juego.Titulo)

            Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
            imagenJuegoSeleccionado.Source = imagen.Source
            imagenJuegoSeleccionado.Tag = juego

            Dim gridLogrosJuego As Grid = pagina.FindName("gridLogrosJuego")
            Dim fondo As String = dominioImagenes + "/steam/apps/" + juego.ID + "/page_bg_generated_v6b.jpg"
            Dim fondoBrush As New ImageBrush With {
                .ImageSource = New BitmapImage(New Uri(fondo)),
                .Stretch = Stretch.UniformToFill
            }
            gridLogrosJuego.Background = fondoBrush

            Dim helper As New LocalObjectStorageHelper
            Dim listaCuentas As New List(Of Cuenta)

            If Await helper.FileExistsAsync("listaCuentas3") = True Then
                listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas3")
            End If

            Logros.Cargar(cuenta, juego, listaCuentas, listaJuegos, listaJuegosOcultos)

        End Sub

        Private Async Sub ImagenCarga(sender As Object, e As ImageExOpenedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")

            Dim imagen As ImageEx = sender
            Dim juego As Juego = imagen.Tag

            If juego.Logros.Count = 0 Then
                Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

                Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
                Dim cuenta As Cuenta = nvJuegos.Tag

                Dim logros2 As List(Of Logro) = Await Logros.SacarJuegoLogros(cuenta.ID64, juego)

                If logros2.Count = 0 Then
                    Dim id As String = String.Empty

                    For Each item In gv.Items
                        Dim sp As StackPanel = item
                        Dim juegoItem As Juego = sp.Tag

                        If juego.ID = juegoItem.ID Then
                            gv.Items.Remove(item)
                            id = juego.ID
                        End If
                    Next

                    If listaJuegosOcultos.Count > 0 Then
                        Dim añadir As Boolean = True

                        For Each oculto In listaJuegosOcultos
                            If oculto.ID = id Then
                                añadir = False
                            End If
                        Next

                        If añadir = True Then
                            listaJuegosOcultos.Add(New JuegoOculto(id, True))
                        End If
                    Else
                        listaJuegosOcultos.Add(New JuegoOculto(id, True))
                    End If
                Else
                    For Each juego2 In listaJuegos
                        If juego2.ID = juego.ID Then
                            For Each logro2 In logros2
                                juego2.Logros.Add(logro2)
                            Next
                        End If
                    Next

                    For Each item In gv.Items
                        Dim sp As StackPanel = item
                        Dim juegoItem As Juego = sp.Tag

                        If juego.ID = juegoItem.ID Then
                            Dim tbLogros As TextBlock = sp.Children(1)

                            Dim contadorLogrosTerminados As Integer = 0

                            For Each logro In logros2
                                If logro.Estado = "1" Then
                                    contadorLogrosTerminados += 1
                                End If
                            Next

                            If juego.Logros.Count > 0 Then
                                tbLogros.Text = contadorLogrosTerminados.ToString + "/" + logros2.Count.ToString + " • " + (CInt((100 / juego.Logros.Count) * contadorLogrosTerminados)).ToString + "%"
                            End If
                        End If
                    Next
                End If
            End If

        End Sub

        Private Sub ImagenFalla(sender As Object, e As ImageExFailedEventArgs)

            Dim cambiar As Boolean = True

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")

            Dim imagen As ImageEx = sender
            Dim imagenFuente As String = imagen.Source

            If imagenFuente.Contains("/library_600x900.jpg") Then
                imagen.Source = imagenFuente.Replace("/library_600x900.jpg", "/header.jpg")
                cambiar = False
            End If

            If cambiar = True Then
                Dim juego As Juego = imagen.Tag

                Dim id As String = String.Empty

                For Each item In gv.Items
                    Dim sp As StackPanel = item
                    Dim juegoItem As Juego = sp.Tag

                    If juego.ID = juegoItem.ID Then
                        gv.Items.Remove(item)
                        id = juego.ID
                    End If
                Next

                If Not id = String.Empty Then
                    If listaJuegosOcultos.Count > 0 Then
                        Dim añadir As Boolean = True

                        For Each oculto In listaJuegosOcultos
                            If oculto.ID = id Then
                                añadir = False
                            End If
                        Next

                        If añadir = True Then
                            listaJuegosOcultos.Add(New JuegoOculto(id, True))
                        End If
                    Else
                        listaJuegosOcultos.Add(New JuegoOculto(id, True))
                    End If
                End If
            End If

        End Sub

    End Module
End Namespace