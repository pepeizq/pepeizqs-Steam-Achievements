Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.UI

Namespace Steam
    Module Logros

        Public Async Sub Cargar(cuentaMaestra As Cuenta, juego As Juego, otrasCuentas As List(Of Cuenta), listaJuegos As List(Of Juego), listaJuegosOcultos As List(Of JuegoOculto))

            Dim helper As New LocalObjectStorageHelper
            Await helper.SaveFileAsync(Of List(Of JuegoOculto))("listaJuegosOcultos" + cuentaMaestra.ID64, listaJuegosOcultos)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
            tbJuegoSeleccionado.Text = juego.Titulo

            Dim spBuscador As StackPanel = pagina.FindName("spBuscador")
            spBuscador.Visibility = Visibility.Collapsed

            Dim pr As ProgressRing = pagina.FindName("prLogros")
            pr.Visibility = Visibility.Visible

            Dim svLogros As ScrollViewer = pagina.FindName("svLogrosJuego")
            svLogros.Visibility = Visibility.Collapsed

            Dim pb As ProgressBar = pagina.FindName("pbJuegoSeleccionado")
            pb.Visibility = Visibility.Collapsed
            pb.Maximum = 100
            pb.Value = 0

            Dim tb As TextBlock = pagina.FindName("tbJuegoSeleccionadoLogros")
            tb.Visibility = Visibility.Collapsed

            Dim otrasCuentasLogros As New List(Of LogrosOtraCuenta)

            If Not otrasCuentas Is Nothing Then
                For Each otraCuenta In otrasCuentas
                    If Not otraCuenta.ID64 = cuentaMaestra.ID64 Then
                        Dim htmlOtraCuenta As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + otraCuenta.ID64 + "&appid=" + juego.ID))

                        If Not htmlOtraCuenta = String.Empty Then
                            Dim logrosOtraCuenta As SteamJugadorLogros = JsonConvert.DeserializeObject(Of SteamJugadorLogros)(htmlOtraCuenta)

                            If Not logrosOtraCuenta Is Nothing Then
                                If Not logrosOtraCuenta.Datos.Logros Is Nothing Then
                                    If logrosOtraCuenta.Datos.Logros.Count > 0 Then
                                        otrasCuentasLogros.Add(New LogrosOtraCuenta(otraCuenta, logrosOtraCuenta))
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            Dim htmlJugador As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + cuentaMaestra.ID64 + "&appid=" + juego.ID))
            Dim htmlJuego As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&appid=" + juego.ID))

            Dim listaLogros As New List(Of Logro)

            If Not htmlJugador = Nothing Then
                Dim logrosJugador As SteamJugadorLogros = JsonConvert.DeserializeObject(Of SteamJugadorLogros)(htmlJugador)

                If Not htmlJuego = Nothing Then
                    Dim logrosJuego As SteamJuegoLogros = JsonConvert.DeserializeObject(Of SteamJuegoLogros)(htmlJuego)

                    If Not logrosJugador Is Nothing Then
                        If Not logrosJuego Is Nothing Then
                            If logrosJugador.Datos.Logros.Count > 0 Then
                                For Each logroJugador In logrosJugador.Datos.Logros
                                    If logrosJuego.Datos.Datos2.Logros.Count > 0 Then
                                        For Each logroJuego In logrosJuego.Datos.Datos2.Logros
                                            If logroJugador.NombreAPI = logroJuego.NombreAPI Then
                                                Dim imagen As String = String.Empty

                                                If logroJugador.Estado = "1" Then
                                                    imagen = logroJuego.IconoCompletado
                                                Else
                                                    imagen = logroJuego.IconoPendiente
                                                End If

                                                Dim logro As New Logro(logroJuego.NombreAPI, logroJugador.Estado, logroJuego.NombreMostrar, logroJuego.Descripcion, logroJugador.Fecha, imagen)

                                                Dim añadir As Boolean = True
                                                Dim k As Integer = 0
                                                While k < listaLogros.Count
                                                    If listaLogros(k).ID = logro.ID Then
                                                        añadir = False
                                                    End If
                                                    k += 1
                                                End While

                                                If añadir = True Then
                                                    listaLogros.Add(logro)
                                                End If
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If

            Dim totalLogros As Integer = listaLogros.Count
            Dim conseguidosLogros As Integer = 0

            If listaLogros.Count > 0 Then
                listaLogros.Sort(Function(x As Logro, y As Logro)
                                     Dim resultado As Integer = y.Estado.CompareTo(x.Estado)
                                     If resultado = 0 Then
                                         resultado = x.Nombre.CompareTo(y.Nombre)
                                     End If
                                     Return resultado
                                 End Function)

                svLogros.Visibility = Visibility.Visible

                Dim spCompletados As StackPanel = pagina.FindName("spLogrosJuegoCompletados")
                spCompletados.Children.Clear()

                Dim spPendientes As StackPanel = pagina.FindName("spLogrosJuegoPendientes")
                spPendientes.Children.Clear()

                For Each logro In listaLogros
                    BotonEstilo(logro, spCompletados, spPendientes, otrasCuentasLogros)

                    If logro.Estado = "1" Then
                        conseguidosLogros += 1
                    End If
                Next

                Dim gridCompletados As Grid = pagina.FindName("gridLogrosJuegoCompletados")

                If spCompletados.Children.Count > 0 Then
                    gridCompletados.Visibility = Visibility.Visible
                Else
                    gridCompletados.Visibility = Visibility.Collapsed
                End If

                Dim gridPendientes As Grid = pagina.FindName("gridLogrosJuegoPendientes")

                If spPendientes.Children.Count > 0 Then
                    gridPendientes.Visibility = Visibility.Visible
                Else
                    gridPendientes.Visibility = Visibility.Collapsed
                End If

                Dim spSeparador As StackPanel = pagina.FindName("spLogrosSeparador")

                If gridCompletados.Visibility = Visibility.Visible And gridPendientes.Visibility = Visibility.Visible Then
                    spSeparador.Visibility = Visibility.Visible
                Else
                    spSeparador.Visibility = Visibility.Collapsed
                End If
            Else
                svLogros.Visibility = Visibility.Collapsed
            End If

            If totalLogros > 0 Then
                pb.Visibility = Visibility.Visible
                pb.Maximum = totalLogros
                pb.Value = conseguidosLogros

                tb.Visibility = Visibility.Visible
                tb.Text = "(" + conseguidosLogros.ToString + "/" + totalLogros.ToString + ")"

                For Each juego2 In listaJuegos
                    If juego2.ID = juego.ID Then
                        juego2.Logros = listaLogros
                    End If
                Next
            End If

            Await helper.SaveFileAsync(Of List(Of Juego))("listaJuegos" + cuentaMaestra.ID64, listaJuegos)

            pr.Visibility = Visibility.Collapsed

        End Sub

        Private Sub BotonEstilo(logro As Logro, spCompletados As StackPanel, spPendientes As StackPanel, otrasCuentasLogros As List(Of LogrosOtraCuenta))

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim grid As New Grid

            Dim col1 As New ColumnDefinition
            Dim col2 As New ColumnDefinition
            Dim col3 As New ColumnDefinition

            col1.Width = New GridLength(1, GridUnitType.Auto)
            col2.Width = New GridLength(1, GridUnitType.Star)
            col3.Width = New GridLength(1, GridUnitType.Auto)

            grid.ColumnDefinitions.Add(col1)
            grid.ColumnDefinitions.Add(col2)
            grid.ColumnDefinitions.Add(col3)

            Dim borde As New Border With {
                .BorderBrush = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
                .BorderThickness = New Thickness(1, 1, 1, 1),
                .VerticalAlignment = VerticalAlignment.Center
            }

            Dim imagen As New ImageEx With {
                .Stretch = Stretch.UniformToFill,
                .IsCacheEnabled = True,
                .Width = 64,
                .Height = 64
            }

            Try
                imagen.Source = New BitmapImage(New Uri(logro.Imagen))
            Catch ex As Exception

            End Try

            borde.Child = imagen
            borde.SetValue(Grid.ColumnProperty, 0)
            grid.Children.Add(borde)

            '-------------------------------

            Dim gridDatos As New Grid

            Dim row1 As New RowDefinition
            Dim row2 As New RowDefinition

            row1.Height = New GridLength(1, GridUnitType.Star)
            row2.Height = New GridLength(1, GridUnitType.Auto)

            gridDatos.RowDefinitions.Add(row1)
            gridDatos.RowDefinitions.Add(row2)

            Dim textoNombre As New TextBlock With {
                .Text = logro.Nombre,
                .Margin = New Thickness(15, 5, 10, 5),
                .TextWrapping = TextWrapping.Wrap,
                .Foreground = New SolidColorBrush(Colors.White),
                .VerticalAlignment = VerticalAlignment.Center
            }

            textoNombre.SetValue(Grid.RowProperty, 0)
            gridDatos.Children.Add(textoNombre)

            If Not logro.Descripcion = Nothing Then
                row2.Height = New GridLength(1, GridUnitType.Star)

                Dim textoDescripcion As New TextBlock With {
                    .Text = logro.Descripcion,
                    .Margin = New Thickness(15, 0, 10, 5),
                    .TextWrapping = TextWrapping.Wrap,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .FontSize = 14,
                    .VerticalAlignment = VerticalAlignment.Center
                }

                textoDescripcion.SetValue(Grid.RowProperty, 1)
                gridDatos.Children.Add(textoDescripcion)
            End If

            gridDatos.SetValue(Grid.ColumnProperty, 1)
            grid.Children.Add(gridDatos)

            '-------------------------------

            If logro.Estado = "1" Then
                Dim fecha As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                Try
                    fecha = fecha.AddSeconds(logro.Fecha)
                    fecha = fecha.ToLocalTime
                Catch ex As Exception

                End Try

                Dim tbFecha As New TextBlock With {
                    .Text = fecha.ToShortDateString,
                    .Margin = New Thickness(10, 10, 10, 10),
                    .TextWrapping = TextWrapping.Wrap,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .VerticalAlignment = VerticalAlignment.Center
                }

                tbFecha.SetValue(Grid.ColumnProperty, 2)
                grid.Children.Add(tbFecha)
            End If

            '-------------------------------

            Dim fondoBoton As New SolidColorBrush With {
                .Color = App.Current.Resources("ColorCuarto"),
                .Opacity = 0.6
            }

            Dim boton As New Button With {
                .Background = fondoBoton,
                .Padding = New Thickness(10, 10, 10, 10),
                .HorizontalAlignment = HorizontalAlignment.Stretch,
                .HorizontalContentAlignment = HorizontalAlignment.Stretch,
                .VerticalContentAlignment = VerticalAlignment.Stretch,
                .Tag = logro,
                .Content = grid,
                .Style = App.Current.Resources("ButtonRevealStyle")
            }

            AddHandler boton.Click, AddressOf AbrirYoutube
            AddHandler boton.PointerEntered, AddressOf Interfaz.Entra_Basico
            AddHandler boton.PointerExited, AddressOf Interfaz.Sale_Basico

            Dim sp As New StackPanel With {
                .Orientation = Orientation.Vertical,
                .Margin = New Thickness(0, 8, 0, 8)
            }

            sp.Children.Add(boton)

            If otrasCuentasLogros.Count > 0 Then
                Dim recursos As New Resources.ResourceLoader()

                Dim spCuentas As New StackPanel With {
                    .Orientation = Orientation.Horizontal,
                    .HorizontalAlignment = HorizontalAlignment.Right,
                    .Margin = New Thickness(5, 5, 5, 5)
                }

                Dim tbCuentas As New TextBlock With {
                    .VerticalAlignment = VerticalAlignment.Center,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .Text = recursos.GetString("OtherUsersHasAchievement"),
                    .FontSize = 14,
                    .Margin = New Thickness(0, 0, 10, 0)
                }

                spCuentas.Children.Add(tbCuentas)

                For Each otraCuenta In otrasCuentasLogros
                    If otraCuenta.LogrosJuego.Datos.Logros.Count > 0 Then
                        For Each otroLogro In otraCuenta.LogrosJuego.Datos.Logros
                            If logro.ID = otroLogro.NombreAPI Then
                                If otroLogro.Estado = "1" Then
                                    Dim imagenCuenta As New ImageEx With {
                                        .Stretch = Stretch.UniformToFill,
                                        .IsCacheEnabled = True,
                                        .Width = 32,
                                        .Height = 32,
                                        .Margin = New Thickness(5, 0, 0, 0)
                                    }

                                    Try
                                        imagenCuenta.Source = New BitmapImage(New Uri(otraCuenta.Cuenta.Avatar))
                                    Catch ex As Exception

                                    End Try

                                    spCuentas.Children.Add(imagenCuenta)
                                End If
                            End If
                        Next
                    End If
                Next

                If spCuentas.Children.Count > 1 Then
                    sp.Children.Add(spCuentas)
                End If
            End If

            If logro.Estado = "1" Then
                spCompletados.Children.Add(sp)
            Else
                spPendientes.Children.Add(sp)
            End If

        End Sub

        Public Async Function SacarJuegoLogros(idCuenta As String, juego As Juego) As Task(Of List(Of Logro))

            Dim htmlLogros As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + idCuenta + "&appid=" + juego.ID))
            Dim listaLogros As New List(Of Logro)

            If Not htmlLogros = Nothing Then
                Dim logros As SteamJugadorLogros = JsonConvert.DeserializeObject(Of SteamJugadorLogros)(htmlLogros)

                If Not logros Is Nothing Then
                    If Not logros.Datos.Logros Is Nothing Then
                        If logros.Datos.Logros.Count > 0 Then
                            For Each logro In logros.Datos.Logros
                                listaLogros.Add(New Logro(logro.NombreAPI, logro.Estado, Nothing, Nothing, logro.Fecha, Nothing))
                            Next
                        End If
                    End If
                End If
            End If

            Return listaLogros

        End Function

        Private Sub AbrirYoutube(sender As Object, e As RoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
            Dim juego As Juego = imagenJuegoSeleccionado.Tag

            Dim boton As Button = sender
            Dim logro As Logro = boton.Tag

            Youtube.Cargar(juego, logro)

        End Sub

    End Module
End Namespace