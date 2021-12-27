Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.Storage
Imports Windows.Storage.FileProperties
Imports Windows.UI

Namespace Steam
    Module Juegos

        Public anchoColumna As Integer = 200
        Dim dominioImagenes As String = "https://cdn.cloudflare.steamstatic.com"

        Dim listaJuegos As New List(Of Juego)

        Public Async Sub Cargar(cuenta As Cuenta)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim recursos As New Resources.ResourceLoader()

            Dim gvJuegos As GridView = pagina.FindName("gvJuegos")
            gvJuegos.Items.Clear()

            Dim gridCarga As Grid = pagina.FindName("gridJuegosCarga")
            gridCarga.Visibility = Visibility.Visible

            Dim tbMensajeCarga As TextBlock = pagina.FindName("tbJuegosCargaMensaje")
            tbMensajeCarga.Text = recursos.GetString("LoadingMessage1")

            Dim spBuscador As StackPanel = pagina.FindName("spBuscador")
            spBuscador.Visibility = Visibility.Collapsed

            Dim helper As New LocalObjectStorageHelper

            listaJuegos.Clear()

            Dim carpetaFicheros As StorageFolder = Nothing
            Dim errorCarpeta As Boolean = False

            Try
                carpetaFicheros = Await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\Juegos_" + cuenta.ID64)
            Catch ex As Exception
                errorCarpeta = True
            End Try

            If errorCarpeta = True Then
                Try
                    Await ApplicationData.Current.LocalFolder.CreateFolderAsync("Juegos_" + cuenta.ID64, CreationCollisionOption.ReplaceExisting)
                    carpetaFicheros = Await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\Juegos_" + cuenta.ID64)
                Catch ex As Exception

                End Try
            End If

            If Not carpetaFicheros Is Nothing Then
                Dim listaFicheros As IReadOnlyList(Of IStorageItem) = Await carpetaFicheros.GetFilesAsync

                If Not listaFicheros Is Nothing Then
                    If listaFicheros.Count > 0 Then
                        For Each fichero In listaFicheros
                            Dim propiedades As BasicProperties = Await fichero.GetBasicPropertiesAsync

                            If propiedades.Size > 0 Then
                                If fichero.Name.Contains("juego_") Then
                                    Dim temp As Juego = Await helper.ReadFileAsync(Of Juego)("Juegos_" + cuenta.ID64 + "\" + fichero.Name)
                                    listaJuegos.Add(temp)
                                End If
                            End If
                        Next
                    End If
                End If
            End If

            If listaJuegos Is Nothing Then
                listaJuegos = New List(Of Juego)
            End If

            tbMensajeCarga.Text = recursos.GetString("LoadingMessage2")

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

            Dim resultadosBusqueda As New List(Of BusquedaFichero)

            If Not listaJuegos Is Nothing Then
                If listaJuegos.Count > 0 Then
                    For Each juego In listaJuegos
                        If Not juego Is Nothing Then
                            resultadosBusqueda.Add(New BusquedaFichero(juego.Titulo, "Juegos_" + cuenta.ID64 + "\juego_" + juego.ID))
                        End If
                    Next
                End If
            End If

            Try
                Await helper.SaveFileAsync(Of List(Of BusquedaFichero))("busqueda_" + cuenta.ID64, resultadosBusqueda)
            Catch ex As Exception

            End Try

            If Not listaJuegos Is Nothing Then
                If listaJuegos.Count > 0 Then
                    listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

                    Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")
                    gv.Items.Clear()

                    For Each juego In listaJuegos
                        Dim añadir As Boolean = True

                        If juego.Escaneado = True Then
                            If juego.Logros.Count = 0 Then
                                añadir = False
                            End If
                        End If

                        If añadir = True Then
                            BotonEstilo(juego, gv)
                        End If
                    Next

                    spBuscador.Visibility = Visibility.Visible
                End If
            End If

            tbMensajeCarga.Text = String.Empty
            gridCarga.Visibility = Visibility.Collapsed

        End Sub

        Public Sub BotonEstilo(juego As Juego, gv As AdaptiveGridView)

            Dim sp As New StackPanel With {
                .Orientation = Orientation.Vertical,
                .Tag = juego,
                .VerticalAlignment = VerticalAlignment.Center
            }

            Dim imagen As New ImageEx With {
                .Tag = juego,
                .Source = juego.Imagen,
                .IsCacheEnabled = True,
                .Stretch = Stretch.UniformToFill,
                .Padding = New Thickness(0, 0, 0, 0),
                .HorizontalAlignment = HorizontalAlignment.Center,
                .VerticalAlignment = VerticalAlignment.Center,
                .EnableLazyLoading = True
            }

            AddHandler imagen.ImageExOpened, AddressOf ImagenCarga
            AddHandler imagen.ImageExFailed, AddressOf ImagenFalla

            sp.Children.Add(imagen)

            '-----------------------------------------------------

            Dim tbLogros As New TextBlock With {
                .HorizontalAlignment = HorizontalAlignment.Right,
                .Foreground = New SolidColorBrush(Colors.White),
                .FontSize = 15,
                .Margin = New Thickness(10, 10, 10, 12)
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

            '-----------------------------------------------------

            Dim boton As New Button With {
                .Tag = juego,
                .Content = sp,
                .Padding = New Thickness(0, 0, 0, 0),
                .Margin = New Thickness(10, 10, 10, 10),
                .MinHeight = 40,
                .MinWidth = 40,
                .MaxWidth = anchoColumna + 20,
                .Background = New SolidColorBrush(App.Current.Resources("ColorCuarto")),
                .BorderBrush = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
                .BorderThickness = New Thickness(1, 1, 1, 1),
                .HorizontalAlignment = HorizontalAlignment.Center,
                .VerticalAlignment = VerticalAlignment.Center
            }

            Dim tbToolTip As TextBlock = New TextBlock With {
                .Text = juego.Titulo,
                .FontSize = 16,
                .TextWrapping = TextWrapping.Wrap
            }

            ToolTipService.SetToolTip(boton, tbToolTip)
            ToolTipService.SetPlacement(boton, PlacementMode.Mouse)

            AddHandler boton.Click, AddressOf AbrirLogros
            AddHandler boton.PointerEntered, AddressOf Interfaz.Entra_Boton_StackPanel
            AddHandler boton.PointerExited, AddressOf Interfaz.Sale_Boton_StackPanel

            gv.Items.Add(boton)

        End Sub

        Private Async Sub AbrirLogros(sender As Object, e As RoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim recursos As New Resources.ResourceLoader()

            Dim boton As Button = sender
            Dim sp As StackPanel = boton.Content
            Dim imagen As ImageEx = sp.Children(0)
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
            Interfaz.Pestañas.Visibilidad(gridLogros, juego.Titulo, sender)

            Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
            Dim imagenJuegoNueva As String = imagen.Source.ToString

            If imagenJuegoNueva.Contains("/library_600x900.") Then
                imagenJuegoNueva = imagenJuegoNueva.Replace("/library_600x900.", "/header.")
            End If

            imagenJuegoSeleccionado.Source = imagenJuegoNueva
            imagenJuegoSeleccionado.Tag = juego

            Dim fondo As String = dominioImagenes + "/steam/apps/" + juego.ID + "/page_bg_generated_v6b.jpg"
            Dim fondoBrush As New ImageBrush With {
                .ImageSource = New BitmapImage(New Uri(fondo)),
                .Stretch = Stretch.UniformToFill
            }
            gridLogros.Background = fondoBrush

            Dim botonAbrirTienda As Button = pagina.FindName("botonJuegoAbrirTienda")
            botonAbrirTienda.Tag = "https://store.steampowered.com/app/" + juego.ID + "/?curator_clanid=33500256"

            Dim botonAbrirGuias As Button = pagina.FindName("botonJuegoAbrirGuias")
            botonAbrirGuias.Tag = "https://steamcommunity.com/app/" + juego.ID + "/guides/?curator_clanid=33500256"

            Dim botonAbrirVideo As Button = pagina.FindName("botonJuegoAbrirVideo")
            botonAbrirVideo.Visibility = Visibility.Collapsed

            Dim helper As New LocalObjectStorageHelper
            Dim listaCuentas As New List(Of Cuenta)

            If Await helper.FileExistsAsync("listaCuentas3") = True Then
                listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas3")
            End If

            Logros.Cargar(cuenta, juego, listaCuentas, listaJuegos)

        End Sub

        Private Async Sub ImagenCarga(sender As Object, e As ImageExOpenedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")

            Dim imagen As ImageEx = sender
            Dim juego As Juego = imagen.Tag

            Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

            Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
            Dim cuenta As Cuenta = nvJuegos.Tag

            juego.Logros = Await Logros.SacarJuegoLogros(cuenta.ID64, juego)

            If juego.Escaneado = Nothing Then
                juego.Escaneado = True
            End If

            If juego.Logros.Count = 0 Then
                For Each item In gv.Items
                    Dim boton As Button = item
                    Dim juegoItem As Juego = boton.Tag

                    If juego.ID = juegoItem.ID Then
                        gv.Items.Remove(item)
                    End If
                Next
            Else
                For Each item In gv.Items
                    Dim boton As Button = item
                    Dim juegoItem As Juego = boton.Tag
                    Dim sp As StackPanel = boton.Content

                    If juego.ID = juegoItem.ID Then
                        Dim tbLogros As TextBlock = sp.Children(1)

                        Dim contadorLogrosTerminados As Integer = 0

                        For Each logro In juego.Logros
                            If logro.Estado = "1" Then
                                contadorLogrosTerminados += 1
                            End If
                        Next

                        If juego.Logros.Count > 0 Then
                            tbLogros.Text = contadorLogrosTerminados.ToString + "/" + juego.Logros.Count.ToString + " • " + (CInt((100 / juego.Logros.Count) * contadorLogrosTerminados)).ToString + "%"
                        End If
                    End If
                Next
            End If

            Dim helper As New LocalObjectStorageHelper
            Try
                Await helper.SaveFileAsync(Of Juego)("Juegos_" + cuenta.ID64 + "\juego_" + juego.ID, juego)
            Catch ex As Exception

            End Try

        End Sub

        Private Async Sub ImagenFalla(sender As Object, e As ImageExFailedEventArgs)

            Dim cambiar As Boolean = True

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

            Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
            Dim cuenta As Cuenta = nvJuegos.Tag

            Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")

            Dim imagen As ImageEx = sender
            Dim juego As Juego = imagen.Tag
            Dim imagenFuente As String = imagen.Source

            If imagenFuente.Contains("/library_600x900.") Then
                imagen.Source = imagenFuente.Replace("/library_600x900.", "/header.")
                juego.Imagen = imagen.Source
                cambiar = False

                Dim helper As New LocalObjectStorageHelper
                Try
                    Await helper.SaveFileAsync(Of Juego)("Juegos_" + cuenta.ID64 + "\juego_" + juego.ID, juego)
                Catch ex As Exception

                End Try
            End If

            If cambiar = True Then
                For Each item In gv.Items
                    Dim boton As Button = item
                    Dim juegoItem As Juego = boton.Tag

                    If juego.ID = juegoItem.ID Then
                        gv.Items.Remove(item)
                    End If
                Next
            End If

        End Sub

    End Module
End Namespace