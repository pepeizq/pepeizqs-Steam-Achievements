Imports FontAwesome.UWP
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
        Dim listaCuentas As List(Of Cuenta) = Nothing

        If Await helper.FileExistsAsync("listaCuentas2") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas2")
        Else
            listaCuentas = New List(Of Cuenta)
        End If

        Dim htmlID As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&vanityurl=" + usuario))

        If Not htmlID = Nothing Then
            Dim cuentaVanidad As CuentaVanidad = JsonConvert.DeserializeObject(Of CuentaVanidad)(htmlID)

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
                    Dim cuenta As Cuenta = JsonConvert.DeserializeObject(Of Cuenta)(htmlDatos)

                    Dim añadir As Boolean = True
                    Dim k As Integer = 0
                    While k < listaCuentas.Count
                        If listaCuentas(k).Respuesta.Jugador(0).ID64 = cuenta.Respuesta.Jugador(0).ID64 Then
                            añadir = False
                        End If
                        k += 1
                    End While

                    If añadir = True Then
                        listaCuentas.Add(cuenta)
                        Await helper.SaveFileAsync(Of List(Of Cuenta))("listaCuentas2", listaCuentas)
                        CargarXaml()
                    End If
                End If
            End If
        End If

        tb.IsEnabled = True
        tb.Text = String.Empty
        boton.IsEnabled = True

    End Sub

    Public Async Sub CargarXaml()

        Dim helper As New LocalObjectStorageHelper
        Dim listaCuentas As List(Of Cuenta) = Nothing

        If Await helper.FileExistsAsync("listaCuentas2") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas2")
        Else
            listaCuentas = New List(Of Cuenta)
        End If

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        If listaCuentas.Count > 0 Then
            Dim columna As ColumnDefinition = pagina.FindName("gridColumnaUsuarios")
            columna.Width = New GridLength(1, GridUnitType.Star)

            Dim gridLista As Grid = pagina.FindName("gridListaUsuarios")
            gridLista.Visibility = Visibility.Visible

            Dim lvUsuarios As ListView = pagina.FindName("lvUsuarios")
            lvUsuarios.Items.Clear()

            For Each cuenta In listaCuentas
                Dim sp As New StackPanel With {
                    .Orientation = Orientation.Horizontal,
                    .Tag = cuenta
                }

                Dim panel1 As New DropShadowPanel With {
                    .BlurRadius = 10,
                    .ShadowOpacity = 0.3,
                    .Color = Colors.Black,
                    .Margin = New Thickness(5, 5, 5, 5)
                }

                Dim grid As New Grid With {
                    .Tag = cuenta,
                    .Padding = New Thickness(10, 10, 10, 10),
                    .Background = New SolidColorBrush(App.Current.Resources("ColorSecundario")),
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
                    imagen.Source = New BitmapImage(New Uri(cuenta.Respuesta.Jugador(0).Avatar))
                Catch ex As Exception

                End Try

                imagen.SetValue(Grid.ColumnProperty, 0)
                grid.Children.Add(imagen)

                '-------------------------------

                Dim textoNombre As New TextBlock With {
                    .Text = cuenta.Respuesta.Jugador(0).Nombre,
                    .VerticalAlignment = VerticalAlignment.Center,
                    .TextWrapping = TextWrapping.Wrap,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .MaxWidth = 400,
                    .Margin = New Thickness(10, 0, 10, 0)
                }

                textoNombre.SetValue(Grid.ColumnProperty, 1)
                grid.Children.Add(textoNombre)

                AddHandler grid.PointerEntered, AddressOf UsuarioEntraBoton
                AddHandler grid.PointerExited, AddressOf UsuarioSaleBoton

                panel1.Content = grid
                sp.Children.Add(panel1)

                '-------------------------------

                Dim panel2 As New DropShadowPanel With {
                    .BlurRadius = 10,
                    .ShadowOpacity = 0.3,
                    .Color = Colors.Black,
                    .Margin = New Thickness(25, 5, 5, 5),
                    .VerticalAlignment = VerticalAlignment.Center,
                    .HorizontalAlignment = HorizontalAlignment.Center
                }

                Dim iconoBorrar As New FontAwesome.UWP.FontAwesome With {
                    .Icon = FontAwesomeIcon.Times
                }

                Dim botonBorrar As New Button With {
                    .Background = New SolidColorBrush(App.Current.Resources("ColorSecundario")),
                    .Foreground = New SolidColorBrush(Colors.White),
                    .Content = iconoBorrar,
                    .Tag = cuenta
                }

                AddHandler botonBorrar.Click, AddressOf BorrarCuenta
                AddHandler botonBorrar.PointerEntered, AddressOf UsuarioEntraBoton2
                AddHandler botonBorrar.PointerExited, AddressOf UsuarioSaleBoton2

                panel2.Content = botonBorrar
                sp.Children.Add(panel2)

                lvUsuarios.Items.Add(sp)
            Next
        End If

    End Sub

    Private Async Sub BorrarCuenta(sender As Object, e As RoutedEventArgs)

        Dim boton As Button = sender
        Dim cuenta As Cuenta = boton.Tag

        Dim helper As New LocalObjectStorageHelper

        If Await helper.FileExistsAsync("listaCuentas2") = True Then
            Dim listaCuentas As List(Of Cuenta) = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas2")

            For Each subcuenta In listaCuentas.ToList
                If cuenta.Respuesta.Jugador(0).ID64 = subcuenta.Respuesta.Jugador(0).ID64 Then
                    listaCuentas.Remove(subcuenta)
                End If
            Next

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim lista As ListView = pagina.FindName("lvUsuarios")

            For Each item In lista.Items
                Dim sp As StackPanel = item
                Dim cuentaSp As Cuenta = sp.Tag

                If cuentaSp.Respuesta.Jugador(0).ID64 = cuenta.Respuesta.Jugador(0).ID64 Then
                    lista.Items.Remove(item)
                End If
            Next

            Await helper.SaveFileAsync(Of List(Of Cuenta))("listaCuentas2", listaCuentas)
        End If

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim grid As Grid = sender
        Dim imagen As ImageEx = grid.Children(0)

        imagen.Saturation(0).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim grid As Grid = sender
        Dim imagen As ImageEx = grid.Children(0)

        imagen.Saturation(1).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

    Private Sub UsuarioEntraBoton2(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton2(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

End Module
