Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Animations
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.UI
Imports Windows.UI.Core

Module Cuentas

    Public Async Sub Añadir(usuario As String)

        usuario = usuario.Replace("https://steamcommunity.com/id/", Nothing)
        usuario = usuario.Replace("http://steamcommunity.com/id/", Nothing)

        usuario = usuario.Trim

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tb As TextBox = pagina.FindName("tbUsuarioCuenta")
        tb.IsEnabled = False

        Dim boton As Button = pagina.FindName("botonAgregarUsuario")
        boton.IsEnabled = False

        Dim helper As New LocalObjectStorageHelper
        Dim listaCuentas As New List(Of SteamCuenta)

        If Await helper.FileExistsAsync("listaCuentas2") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of SteamCuenta))("listaCuentas2")
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
                    Dim cuenta As SteamCuenta = JsonConvert.DeserializeObject(Of SteamCuenta)(htmlDatos)

                    Dim añadir As Boolean = True
                    Dim k As Integer = 0
                    While k < listaCuentas.Count
                        If listaCuentas(k).Datos.Jugador(0).ID64 = cuenta.Datos.Jugador(0).ID64 Then
                            añadir = False
                        End If
                        k += 1
                    End While

                    If añadir = True Then
                        listaCuentas.Add(cuenta)
                        Await helper.SaveFileAsync(Of List(Of SteamCuenta))("listaCuentas2", listaCuentas)
                        BotonEstilo()
                    End If
                End If
            End If
        End If

        tb.IsEnabled = True
        tb.Text = String.Empty
        boton.IsEnabled = True

    End Sub

    Public Async Sub BotonEstilo()

        Dim helper As New LocalObjectStorageHelper
        Dim listaCuentas As New List(Of SteamCuenta)

        If Await helper.FileExistsAsync("listaCuentas2") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of SteamCuenta))("listaCuentas2")
        End If

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        If listaCuentas.Count > 0 Then
            Dim columna As ColumnDefinition = pagina.FindName("gridColumnaUsuarios")
            columna.Width = New GridLength(1, GridUnitType.Star)

            Dim gridUsuarios As Grid = pagina.FindName("gridUsuarios")
            gridUsuarios.Visibility = Visibility.Visible

            Dim spUsuarios As StackPanel = pagina.FindName("spUsuarios")
            spUsuarios.Children.Clear()

            For Each cuenta In listaCuentas
                Dim sp As New StackPanel With {
                    .Orientation = Orientation.Horizontal,
                    .Tag = cuenta
                }

                Dim panel1 As New DropShadowPanel With {
                    .BlurRadius = 10,
                    .ShadowOpacity = 0.3,
                    .Color = Colors.Black,
                    .Margin = New Thickness(0, 0, 0, 20)
                }

                Dim grid As New Grid With {
                    .Tag = cuenta,
                    .Padding = New Thickness(20, 20, 20, 20),
                    .Width = 400
                }

                Dim col1 As New ColumnDefinition
                Dim col2 As New ColumnDefinition

                col1.Width = New GridLength(1, GridUnitType.Auto)
                col2.Width = New GridLength(1, GridUnitType.Auto)

                grid.ColumnDefinitions.Add(col1)
                grid.ColumnDefinitions.Add(col2)

                Dim imagen As New ImageEx With {
                    .Stretch = Stretch.UniformToFill,
                    .IsCacheEnabled = True,
                    .Width = 50,
                    .Height = 50
                }

                Try
                    imagen.Source = New BitmapImage(New Uri(cuenta.Datos.Jugador(0).Avatar))
                Catch ex As Exception

                End Try

                imagen.SetValue(Grid.ColumnProperty, 0)
                grid.Children.Add(imagen)

                '-------------------------------

                Dim textoNombre As New TextBlock With {
                    .Text = cuenta.Datos.Jugador(0).Nombre,
                    .VerticalAlignment = VerticalAlignment.Center,
                    .TextWrapping = TextWrapping.Wrap,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .MaxWidth = 400,
                    .Margin = New Thickness(15, 0, 10, 0)
                }

                textoNombre.SetValue(Grid.ColumnProperty, 1)
                grid.Children.Add(textoNombre)

                Dim botonCuenta As New Button With {
                    .Background = New SolidColorBrush(App.Current.Resources("ColorSecundario")),
                    .Content = grid,
                    .Tag = cuenta,
                    .Padding = New Thickness(0, 0, 0, 0),
                    .Margin = New Thickness(0, 0, 0, 0)
                }

                AddHandler botonCuenta.Click, AddressOf AbrirCuenta
                AddHandler botonCuenta.PointerEntered, AddressOf UsuarioEntraBoton
                AddHandler botonCuenta.PointerExited, AddressOf UsuarioSaleBoton

                panel1.Content = botonCuenta
                sp.Children.Add(panel1)

                '-------------------------------

                Dim panel2 As New DropShadowPanel With {
                    .BlurRadius = 10,
                    .ShadowOpacity = 0.3,
                    .Color = Colors.Black,
                    .Margin = New Thickness(35, 5, 5, 20),
                    .VerticalAlignment = VerticalAlignment.Center,
                    .HorizontalAlignment = HorizontalAlignment.Center
                }

                Dim iconoBorrar As New FontAwesome5.FontAwesome With {
                    .Icon = FontAwesome5.EFontAwesomeIcon.Solid_Times,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .FontSize = 20
                }

                Dim botonBorrar As New Button With {
                    .Background = New SolidColorBrush(App.Current.Resources("ColorSecundario")),
                    .Content = iconoBorrar,
                    .Padding = New Thickness(15, 12, 15, 12),
                    .Tag = cuenta
                }

                AddHandler botonBorrar.Click, AddressOf BorrarCuenta
                AddHandler botonBorrar.PointerEntered, AddressOf UsuarioEntraBoton2
                AddHandler botonBorrar.PointerExited, AddressOf UsuarioSaleBoton2

                panel2.Content = botonBorrar
                sp.Children.Add(panel2)

                spUsuarios.Children.Add(sp)
            Next
        End If

    End Sub

    Private Sub AbrirCuenta(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tbBuscarJuegos As TextBox = pagina.FindName("tbBuscarJuegos")
        tbBuscarJuegos.Text = String.Empty

        Dim lvLogros As ListView = pagina.FindName("lvLogros")
        lvLogros.Items.Clear()

        Dim recursos As New Resources.ResourceLoader()

        Dim boton As Button = sender
        Dim cuenta As SteamCuenta = boton.Tag

        Dim spCuenta As StackPanel = pagina.FindName("spCuenta")
        spCuenta.Visibility = Visibility.Visible

        Dim imagenCuentaSeleccionada As ImageEx = pagina.FindName("imagenCuentaSeleccionada")
        imagenCuentaSeleccionada.Source = cuenta.Datos.Jugador(0).Avatar

        Dim tbCuentaSeleccionada As TextBlock = pagina.FindName("tbCuentaSeleccionada")
        tbCuentaSeleccionada.Text = cuenta.Datos.Jugador(0).Nombre

        Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        nvJuegos.Visibility = Visibility.Visible

        Dim tbToolTip As TextBlock = New TextBlock With {
            .Text = cuenta.Datos.Jugador(0).Nombre
        }

        ToolTipService.SetToolTip(nvJuegos, tbToolTip)
        nvJuegos.Tag = cuenta

        nvPrincipal.SelectedItem = nvJuegos

        Dim nvLogros As NavigationViewItem = nvPrincipal.MenuItems(2)
        nvLogros.Visibility = Visibility.Collapsed

        Dim gridCuentas As Grid = pagina.FindName("gridCuentas")
        gridCuentas.Visibility = Visibility.Collapsed

        Dim gridJuegos As Grid = pagina.FindName("gridJuegos")
        gridJuegos.Visibility = Visibility.Visible

        Dim tbTitulo As TextBlock = pagina.FindName("tbTitulo")
        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + cuenta.Datos.Jugador(0).Nombre

        Juegos.Cargar(cuenta)

    End Sub

    Private Async Sub BorrarCuenta(sender As Object, e As RoutedEventArgs)

        Dim boton As Button = sender
        Dim cuenta As SteamCuenta = boton.Tag

        Dim helper As New LocalObjectStorageHelper

        If Await helper.FileExistsAsync("listaCuentas2") = True Then
            Dim listaCuentas As List(Of SteamCuenta) = Await helper.ReadFileAsync(Of List(Of SteamCuenta))("listaCuentas2")

            For Each subcuenta In listaCuentas.ToList
                If cuenta.Datos.Jugador(0).ID64 = subcuenta.Datos.Jugador(0).ID64 Then
                    listaCuentas.Remove(subcuenta)
                End If
            Next

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim spUsuarios As StackPanel = pagina.FindName("spUsuarios")

            For Each hijo In spUsuarios.Children
                Dim sp As StackPanel = hijo
                Dim cuentaSp As SteamCuenta = sp.Tag

                If cuentaSp.Datos.Jugador(0).ID64 = cuenta.Datos.Jugador(0).ID64 Then
                    spUsuarios.Children.Remove(hijo)
                End If
            Next

            Await helper.SaveFileAsync(Of List(Of SteamCuenta))("listaCuentas2", listaCuentas)

            If listaCuentas.Count = 0 Then
                Dim columna As ColumnDefinition = pagina.FindName("gridColumnaUsuarios")
                columna.Width = New GridLength(1, GridUnitType.Auto)

                Dim gridUsuarios2 As Grid = pagina.FindName("gridUsuarios")
                gridUsuarios2.Visibility = Visibility.Collapsed
            End If
        End If

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim boton As Button = sender
        Dim grid As Grid = boton.Content
        Dim imagen As ImageEx = grid.Children(0)

        imagen.Saturation(0).Scale(1.1, 1.1, imagen.ActualWidth / 2, imagen.ActualHeight / 2).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim boton As Button = sender
        Dim grid As Grid = boton.Content
        Dim imagen As ImageEx = grid.Children(0)

        imagen.Saturation(1).Scale(1, 1, imagen.ActualWidth / 2, imagen.ActualHeight / 2).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

    Private Sub UsuarioEntraBoton2(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton2(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

End Module
