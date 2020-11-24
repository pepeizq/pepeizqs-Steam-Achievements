Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.UI

Namespace Steam
    Module Cuentas

        Public Sub Cargar()

            CargarBotonesUsuarios()

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbUsuarioCuenta As TextBox = pagina.FindName("tbUsuarioCuenta")

            RemoveHandler tbUsuarioCuenta.TextChanged, AddressOf UsuarioCuentaTextChanged
            AddHandler tbUsuarioCuenta.TextChanged, AddressOf UsuarioCuentaTextChanged

            Dim botonAñadirUsuario As Button = pagina.FindName("botonAñadirUsuario")

            RemoveHandler botonAñadirUsuario.Click, AddressOf AñadirUsuarioClick
            AddHandler botonAñadirUsuario.Click, AddressOf AñadirUsuarioClick

            RemoveHandler botonAñadirUsuario.PointerEntered, AddressOf Interfaz.Entra_Boton_IconoTexto
            AddHandler botonAñadirUsuario.PointerEntered, AddressOf Interfaz.Entra_Boton_IconoTexto

            RemoveHandler botonAñadirUsuario.PointerExited, AddressOf Interfaz.Sale_Boton_IconoTexto
            AddHandler botonAñadirUsuario.PointerExited, AddressOf Interfaz.Sale_Boton_IconoTexto

            Dim botonAbrirPermisos As Button = pagina.FindName("botonAbrirPermisos")

            RemoveHandler botonAbrirPermisos.Click, AddressOf AbrirPermisosClick
            AddHandler botonAbrirPermisos.Click, AddressOf AbrirPermisosClick

            RemoveHandler botonAbrirPermisos.PointerEntered, AddressOf Interfaz.Entra_Boton_IconoTexto
            AddHandler botonAbrirPermisos.PointerEntered, AddressOf Interfaz.Entra_Boton_IconoTexto

            RemoveHandler botonAbrirPermisos.PointerExited, AddressOf Interfaz.Sale_Boton_IconoTexto
            AddHandler botonAbrirPermisos.PointerExited, AddressOf Interfaz.Sale_Boton_IconoTexto

            Dim botonVolverPermisos As Button = pagina.FindName("botonVolverPermisos")

            RemoveHandler botonVolverPermisos.Click, AddressOf VolverPermisosClick
            AddHandler botonVolverPermisos.Click, AddressOf VolverPermisosClick

            RemoveHandler botonVolverPermisos.PointerEntered, AddressOf Interfaz.Entra_Boton_IconoTexto
            AddHandler botonVolverPermisos.PointerEntered, AddressOf Interfaz.Entra_Boton_IconoTexto

            RemoveHandler botonVolverPermisos.PointerExited, AddressOf Interfaz.Sale_Boton_IconoTexto
            AddHandler botonVolverPermisos.PointerExited, AddressOf Interfaz.Sale_Boton_IconoTexto

        End Sub

        Private Sub UsuarioCuentaTextChanged(sender As Object, e As TextChangedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim botonAñadirUsuario As Button = pagina.FindName("botonAñadirUsuario")

            Dim tb As TextBox = sender

            If tb.Text.Length > 4 Then
                botonAñadirUsuario.IsEnabled = True
            Else
                botonAñadirUsuario.IsEnabled = False
            End If

        End Sub

        Private Async Sub AñadirUsuarioClick(ByVal sender As Object, ByVal e As RoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbUsuarioCuenta As TextBox = pagina.FindName("tbUsuarioCuenta")
            Dim usuario As String = tbUsuarioCuenta.Text.Trim

            usuario = usuario.Replace("https://steamcommunity.com/id/", Nothing)
            usuario = usuario.Replace("http://steamcommunity.com/id/", Nothing)
            usuario = usuario.Replace("/", Nothing)

            Dim boton As Button = pagina.FindName("botonAñadirUsuario")
            boton.IsEnabled = False

            Dim helper As New LocalObjectStorageHelper
            Dim listaCuentas As New List(Of Cuenta)

            If Await helper.FileExistsAsync("listaCuentas3") = True Then
                listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas3")
            End If

            Dim htmlID As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&vanityurl=" + usuario))

            If Not htmlID = Nothing Then
                Dim cuentaVanidad As SteamCuentaVanidad = JsonConvert.DeserializeObject(Of SteamCuentaVanidad)(htmlID)

                Dim id64 As String = Nothing

                If Not cuentaVanidad Is Nothing Then
                    id64 = cuentaVanidad.Respuesta.ID64
                End If

                If id64 = Nothing Then
                    id64 = usuario
                End If

                If Not id64 = Nothing Then
                    Dim htmlDatos As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamids=" + id64))

                    If Not htmlDatos = Nothing Then
                        Dim steamCuenta As SteamCuenta = JsonConvert.DeserializeObject(Of SteamCuenta)(htmlDatos)

                        If Not steamCuenta Is Nothing Then
                            Dim añadir As Boolean = True

                            If listaCuentas.Count > 0 Then
                                Dim k As Integer = 0
                                While k < listaCuentas.Count
                                    If listaCuentas(k).ID64 = steamCuenta.Datos.Jugador(0).ID64 Then
                                        añadir = False
                                    End If
                                    k += 1
                                End While
                            End If

                            If añadir = True Then
                                If steamCuenta.Datos.Jugador.Count > 0 Then
                                    Dim cuenta As New Cuenta(steamCuenta.Datos.Jugador(0).ID64, steamCuenta.Datos.Jugador(0).Nombre, steamCuenta.Datos.Jugador(0).Avatar)

                                    listaCuentas.Add(cuenta)
                                    Await helper.SaveFileAsync(Of List(Of Cuenta))("listaCuentas3", listaCuentas)
                                    CargarBotonesUsuarios()
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            tbUsuarioCuenta.IsEnabled = True
            tbUsuarioCuenta.Text = String.Empty
            boton.IsEnabled = True

        End Sub

        Private Async Sub CargarBotonesUsuarios()

            Dim helper As New LocalObjectStorageHelper
            Dim listaCuentas As New List(Of Cuenta)

            If Await helper.FileExistsAsync("listaCuentas3") = True Then
                listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas3")
            End If

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim spUsuarios As StackPanel = pagina.FindName("spUsuarios")
            spUsuarios.Children.Clear()

            If listaCuentas.Count > 0 Then
                For Each cuenta In listaCuentas
                    If Not cuenta Is Nothing Then
                        Dim spUsuario As New StackPanel With {
                            .Orientation = Orientation.Horizontal,
                            .Margin = New Thickness(0, 0, 0, 20)
                        }

                        '-------------------------------

                        Dim colorFondo As New SolidColorBrush With {
                            .Color = App.Current.Resources("ColorCuarto"),
                            .Opacity = 0.5
                        }

                        Dim spCuenta As New StackPanel With {
                            .Orientation = Orientation.Horizontal,
                            .Tag = cuenta,
                            .Background = colorFondo,
                            .Padding = New Thickness(20, 20, 20, 20),
                            .Width = 400,
                            .BorderBrush = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
                            .BorderThickness = New Thickness(1, 1, 1, 1)
                        }

                        Dim imagen As New ImageEx With {
                            .Stretch = Stretch.UniformToFill,
                            .IsCacheEnabled = True,
                            .Width = 50,
                            .Height = 50
                        }

                        Try
                            imagen.Source = New BitmapImage(New Uri(cuenta.Avatar))
                        Catch ex As Exception

                        End Try

                        spCuenta.Children.Add(imagen)

                        Dim textoNombre As New TextBlock With {
                            .Text = cuenta.Nombre,
                            .VerticalAlignment = VerticalAlignment.Center,
                            .TextWrapping = TextWrapping.Wrap,
                            .Foreground = New SolidColorBrush(Colors.White),
                            .MaxWidth = 400,
                            .Margin = New Thickness(15, 0, 0, 0)
                        }

                        spCuenta.Children.Add(textoNombre)

                        Dim botonCuenta As New Button With {
                            .Background = New SolidColorBrush(Colors.Transparent),
                            .Content = spCuenta,
                            .Tag = cuenta,
                            .Padding = New Thickness(0, 0, 0, 0),
                            .Margin = New Thickness(0, 0, 0, 0),
                            .Style = App.Current.Resources("ButtonRevealStyle"),
                            .BorderThickness = New Thickness(0, 0, 0, 0)
                        }

                        AddHandler botonCuenta.Click, AddressOf AbrirCuenta
                        AddHandler botonCuenta.PointerEntered, AddressOf Interfaz.Entra_Boton_ImagenTexto
                        AddHandler botonCuenta.PointerExited, AddressOf Interfaz.Sale_Boton_ImagenTexto

                        spUsuario.Children.Add(botonCuenta)

                        '-------------------------------

                        Dim iconoBorrar As New FontAwesome5.FontAwesome With {
                            .Icon = FontAwesome5.EFontAwesomeIcon.Solid_Times,
                            .Foreground = New SolidColorBrush(Colors.White),
                            .FontSize = 20
                        }

                        Dim botonBorrar As New Button With {
                            .Background = New SolidColorBrush(Colors.Transparent),
                            .Content = iconoBorrar,
                            .Padding = New Thickness(15, 12, 15, 12),
                            .Margin = New Thickness(30, 0, 0, 0),
                            .Style = App.Current.Resources("ButtonRevealStyle"),
                            .Tag = cuenta,
                            .VerticalAlignment = VerticalAlignment.Center,
                            .BorderThickness = New Thickness(0, 0, 0, 0)
                        }

                        AddHandler botonBorrar.Click, AddressOf BorrarCuenta
                        AddHandler botonBorrar.PointerEntered, AddressOf Interfaz.Entra_Boton_Icono
                        AddHandler botonBorrar.PointerExited, AddressOf Interfaz.Entra_Boton_Icono

                        spUsuario.Children.Add(botonBorrar)

                        '-------------------------------

                        spUsuarios.Children.Add(spUsuario)
                    End If
                Next
            End If

            Dim columna As ColumnDefinition = pagina.FindName("gridColumnaUsuarios")
            Dim gridUsuarios As Grid = pagina.FindName("gridUsuarios")

            If spUsuarios.Children.Count > 0 Then
                columna.Width = New GridLength(1, GridUnitType.Star)
                gridUsuarios.Visibility = Visibility.Visible
            Else
                columna.Width = New GridLength(1, GridUnitType.Auto)
                gridUsuarios.Visibility = Visibility.Collapsed
            End If

        End Sub

        Private Sub AbrirCuenta(sender As Object, e As RoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbBuscarJuegos As TextBox = pagina.FindName("tbBuscarJuegos")
            tbBuscarJuegos.Text = String.Empty

            Dim recursos As New Resources.ResourceLoader()

            Dim boton As Button = sender
            Dim cuenta As Cuenta = boton.Tag

            Dim spCuenta As StackPanel = pagina.FindName("spCuenta")
            spCuenta.Visibility = Visibility.Visible

            Dim imagenCuentaSeleccionada As ImageEx = pagina.FindName("imagenCuentaSeleccionada")
            imagenCuentaSeleccionada.Source = cuenta.Avatar

            Dim tbCuentaSeleccionada As TextBlock = pagina.FindName("tbCuentaSeleccionada")
            tbCuentaSeleccionada.Text = cuenta.Nombre

            Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

            Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
            nvJuegos.Visibility = Visibility.Visible

            Dim tbToolTip As TextBlock = New TextBlock With {
                .Text = cuenta.Nombre
            }

            ToolTipService.SetToolTip(nvJuegos, tbToolTip)
            nvJuegos.Tag = cuenta

            nvPrincipal.SelectedItem = nvJuegos

            Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
            nvLogros.Visibility = Visibility.Collapsed

            Dim gridJuegos As Grid = pagina.FindName("gridJuegos")

            Interfaz.Pestañas.Visibilidad_Pestañas(gridJuegos, cuenta.Nombre)

            Juegos.Cargar(cuenta)

        End Sub

        Private Async Sub BorrarCuenta(sender As Object, e As RoutedEventArgs)

            Dim boton As Button = sender
            Dim cuenta As Cuenta = boton.Tag

            Dim helper As New LocalObjectStorageHelper

            If Await helper.FileExistsAsync("listaCuentas3") = True Then
                Dim listaCuentas As List(Of Cuenta) = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas3")

                For Each subcuenta In listaCuentas.ToList
                    If cuenta.ID64 = subcuenta.ID64 Then
                        listaCuentas.Remove(subcuenta)
                    End If
                Next

                Dim frame As Frame = Window.Current.Content
                Dim pagina As Page = frame.Content

                Dim spUsuarios As StackPanel = pagina.FindName("spUsuarios")

                For Each hijo In spUsuarios.Children
                    Dim sp As StackPanel = hijo
                    Dim cuentaSp As Cuenta = sp.Tag

                    If cuentaSp.ID64 = cuenta.ID64 Then
                        spUsuarios.Children.Remove(hijo)
                    End If
                Next

                Await helper.SaveFileAsync(Of List(Of Cuenta))("listaCuentas3", listaCuentas)

                If listaCuentas.Count = 0 Then
                    Dim columna As ColumnDefinition = pagina.FindName("gridColumnaUsuarios")
                    columna.Width = New GridLength(1, GridUnitType.Auto)

                    Dim gridUsuarios2 As Grid = pagina.FindName("gridUsuarios")
                    gridUsuarios2.Visibility = Visibility.Collapsed
                End If
            End If

        End Sub

        Private Sub AbrirPermisosClick(ByVal sender As Object, ByVal e As RoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gridPermisos As Grid = pagina.FindName("gridPermisos")
            Interfaz.Pestañas.Visibilidad_Pestañas(gridPermisos, Nothing)

        End Sub

        Private Sub VolverPermisosClick(ByVal sender As Object, ByVal e As RoutedEventArgs)

            Dim recursos As New Resources.ResourceLoader()

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim gridCuentas As Grid = pagina.FindName("gridCuentas")
            Interfaz.Pestañas.Visibilidad_Pestañas(gridCuentas, recursos.GetString("Accounts"))

        End Sub

    End Module
End Namespace