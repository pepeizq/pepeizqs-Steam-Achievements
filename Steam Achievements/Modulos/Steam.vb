Imports Microsoft.Toolkit.Uwp
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.UI

Module Steam

    Public Async Sub AñadirCuenta(usuario As String)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tb As TextBox = pagina.FindName("tbUsuarioCuenta")
        tb.IsEnabled = False

        Dim boton As Button = pagina.FindName("botonAgregarUsuario")
        boton.IsEnabled = False

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaCuentas As List(Of Cuenta) = Nothing

        If Await helper.FileExistsAsync("listaCuentas") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas")
        Else
            listaCuentas = New List(Of Cuenta)
        End If

        Dim htmlID As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key=488AE837ADDDA0201B51693B28F1B389&vanityurl=" + usuario))

        If Not htmlID = Nothing Then
            Dim id64 As String = Nothing

            If htmlID.Contains("steamid") Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = htmlID.IndexOf("steamid" + ChrW(34))
                temp = htmlID.Remove(0, int)

                int2 = temp.IndexOf(":")
                temp2 = temp.Remove(0, int2 + 1)

                int2 = temp2.IndexOf(ChrW(34))
                temp2 = temp2.Remove(0, int2 + 1)

                int2 = temp2.IndexOf(ChrW(34))
                temp2 = temp2.Remove(int2, temp2.Length - int2)

                id64 = temp2.Trim
            End If

            If id64 = Nothing Then
                id64 = usuario
            End If

            If Not id64 = Nothing Then
                Dim htmlDatos As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key=488AE837ADDDA0201B51693B28F1B389&steamids=" + id64))

                Dim temp3, temp4 As String
                Dim int3, int4 As Integer

                If htmlDatos.Contains(ChrW(34) + "personaname" + ChrW(34)) Then
                    int3 = htmlDatos.IndexOf(ChrW(34) + "personaname" + ChrW(34))
                    temp3 = htmlDatos.Remove(0, int3)

                    int3 = temp3.IndexOf(":")
                    temp3 = temp3.Remove(0, int3 + 1)

                    int3 = temp3.IndexOf(ChrW(34))
                    temp3 = temp3.Remove(0, int3 + 1)

                    int4 = temp3.IndexOf(ChrW(34))
                    temp4 = temp3.Remove(int4, temp3.Length - int4)

                    Dim nombre As String = temp4.Trim

                    Dim temp5, temp6 As String
                    Dim int5, int6 As Integer

                    int5 = htmlDatos.IndexOf(ChrW(34) + "avatarfull" + ChrW(34))
                    temp5 = htmlDatos.Remove(0, int5)

                    int5 = temp5.IndexOf(":")
                    temp5 = temp5.Remove(0, int5 + 1)

                    int5 = temp5.IndexOf(ChrW(34))
                    temp5 = temp5.Remove(0, int5 + 1)

                    int6 = temp5.IndexOf(ChrW(34))
                    temp6 = temp5.Remove(int6, temp5.Length - int6)

                    Dim avatar As String = temp6.Trim

                    Dim cuenta As New Cuenta(id64, usuario, nombre, avatar)

                    Dim nombreBool As Boolean = False
                    Dim k As Integer = 0
                    While k < listaCuentas.Count
                        If listaCuentas(k).NombreUrl = cuenta.NombreUrl Then
                            nombreBool = True
                        End If
                        k += 1
                    End While

                    If nombreBool = False Then
                        listaCuentas.Add(cuenta)
                        Await helper.SaveFileAsync(Of List(Of Cuenta))("listaCuentas", listaCuentas)
                        CargarCuentas()
                    End If
                End If
            End If
        End If

        tb.IsEnabled = True
        boton.IsEnabled = True

    End Sub

    Public Async Sub CargarCuentas()

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaCuentas As List(Of Cuenta) = Nothing

        If Await helper.FileExistsAsync("listaCuentas") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas")
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
                Dim grid As New Grid With {
                    .Tag = cuenta,
                    .Padding = New Thickness(5, 5, 5, 5)
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
                    .Width = 64,
                    .Height = 64
                }

                Try
                    imagen.Source = New BitmapImage(New Uri(cuenta.Avatar))
                Catch ex As Exception

                End Try

                imagen.SetValue(Grid.ColumnProperty, 0)
                grid.Children.Add(imagen)

                '-------------------------------

                Dim textoNombre As New TextBlock With {
                    .Text = cuenta.Nombre,
                    .VerticalAlignment = VerticalAlignment.Center,
                    .TextWrapping = TextWrapping.Wrap,
                    .Foreground = New SolidColorBrush(Colors.White),
                    .MaxWidth = 400,
                    .Margin = New Thickness(10, 0, 10, 0)
                }

                textoNombre.SetValue(Grid.ColumnProperty, 1)
                grid.Children.Add(textoNombre)

                lvUsuarios.Items.Add(grid)
            Next
        End If

    End Sub

    Public Async Sub CargarJuegos(cuenta As Cuenta)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim lvJuegos As ListView = pagina.FindName("lvJuegos")
        lvJuegos.Items.Clear()

        Dim gridBusqueda As Grid = pagina.FindName("gridBusquedaJuegos")
        gridBusqueda.Visibility = Visibility.Collapsed

        Dim gridLogros As Grid = pagina.FindName("gridLogros")
        gridLogros.Visibility = Visibility.Collapsed

        Dim columnaJuegos As ColumnDefinition = pagina.FindName("gridColumnaJuegos")
        columnaJuegos.Width = New GridLength(1, GridUnitType.Star)

        Dim columnaLogros As ColumnDefinition = pagina.FindName("gridColumnaLogros")
        columnaLogros.Width = New GridLength(1, GridUnitType.Auto)

        Dim pr As ProgressRing = pagina.FindName("prJuegos")
        pr.Visibility = Visibility.Visible

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaJuegos As List(Of Juego) = New List(Of Juego)

        Dim htmlJuegos As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key=488AE837ADDDA0201B51693B28F1B389&steamid=" + cuenta.ID64 + "&include_appinfo=1&include_played_free_games=1"))

        If Not htmlJuegos = Nothing Then
            If htmlJuegos.Contains("game_count") Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = htmlJuegos.IndexOf("game_count")
                temp = htmlJuegos.Remove(0, int)

                int2 = temp.IndexOf(",")
                temp2 = temp.Remove(int2, temp.Length - int2)

                temp2 = temp2.Replace("game_count", Nothing)
                temp2 = temp2.Replace(ChrW(34), Nothing)
                temp2 = temp2.Replace(":", Nothing)
                temp2 = temp2.Replace(vbNullChar, Nothing)
                temp2 = temp2.Trim

                If Not temp2 = Nothing Then

                    Dim i As Integer = 0
                    While i < temp2
                        If htmlJuegos.Contains(ChrW(34) + "appid" + ChrW(34)) Then
                            Dim temp3, temp4 As String
                            Dim int3, int4 As Integer

                            int3 = htmlJuegos.IndexOf(ChrW(34) + "appid" + ChrW(34))
                            temp3 = htmlJuegos.Remove(0, int3 + 7)

                            htmlJuegos = temp3

                            int4 = temp3.IndexOf(",")
                            temp4 = temp3.Remove(int4, temp3.Length - int4)

                            temp4 = temp4.Replace(":", Nothing)
                            temp4 = temp4.Trim

                            Dim id As String = temp4

                            Dim temp5, temp6 As String
                            Dim int5, int6 As Integer

                            int5 = htmlJuegos.IndexOf(ChrW(34) + "name" + ChrW(34))
                            temp5 = htmlJuegos.Remove(0, int5)

                            int5 = temp5.IndexOf(":")
                            temp5 = temp5.Remove(0, int5 + 1)

                            int5 = temp5.IndexOf(ChrW(34))
                            temp5 = temp5.Remove(0, int5 + 1)

                            int6 = temp5.IndexOf(ChrW(34))
                            temp6 = temp5.Remove(int6, temp5.Length - int6)

                            Dim titulo As String = temp6.Trim

                            Dim imagen As String = "http://cdn.edgecast.steamstatic.com/steam/apps/" + id + "/capsule_184x69.jpg"

                            Dim juego As New Juego(id, titulo, imagen)

                            Dim tituloBool As Boolean = False
                            Dim k As Integer = 0
                            While k < listaJuegos.Count
                                If listaJuegos(k).ID = juego.ID Then
                                    tituloBool = True
                                End If
                                k += 1
                            End While

                            If tituloBool = False Then
                                listaJuegos.Add(juego)
                            End If
                        End If
                        i += 1
                    End While
                End If
            End If
        End If

        If Not listaJuegos Is Nothing Then
            If listaJuegos.Count > 0 Then
                listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))
                Await helper.SaveFileAsync(Of List(Of Juego))("listaJuegos", listaJuegos)
                CargarListadoJuegos(listaJuegos)

                columnaJuegos.Width = New GridLength(1, GridUnitType.Auto)
                columnaLogros.Width = New GridLength(1, GridUnitType.Star)

                gridBusqueda.Visibility = Visibility.Visible
                gridLogros.Visibility = Visibility.Visible
            End If
        End If

        pr.Visibility = Visibility.Collapsed

    End Sub

    Public Sub CargarListadoJuegos(listaJuegos As List(Of Juego))

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim lvJuegos As ListView = pagina.FindName("lvJuegos")
        lvJuegos.Items.Clear()

        For Each juego In listaJuegos
            Dim grid As New Grid With {
                .Tag = juego,
                .Padding = New Thickness(5, 5, 5, 5)
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
                .Width = 184,
                .Height = 69
            }

            Try
                imagen.Source = New BitmapImage(New Uri(juego.Imagen))
            Catch ex As Exception

            End Try

            imagen.SetValue(Grid.ColumnProperty, 0)
            grid.Children.Add(imagen)

            '-------------------------------

            Dim textoTitulo As New TextBlock With {
                .Text = juego.Titulo,
                .VerticalAlignment = VerticalAlignment.Center,
                .TextWrapping = TextWrapping.Wrap,
                .MaxWidth = 200,
                .Margin = New Thickness(10, 0, 10, 0)
            }

            textoTitulo.SetValue(Grid.ColumnProperty, 1)
            grid.Children.Add(textoTitulo)

            lvJuegos.Items.Add(grid)
        Next

    End Sub

    Public Async Sub CargarLogros(cuenta As Cuenta, juego As Juego)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim panel As DropShadowPanel = pagina.FindName("panelMensajeLogros")
        panel.Visibility = Visibility.Collapsed

        Dim columnaLogros As ColumnDefinition = pagina.FindName("gridColumnaLogros")
        columnaLogros.Width = New GridLength(1, GridUnitType.Star)

        Dim columnaSubLogros As ColumnDefinition = pagina.FindName("gridSubColumnaLogros")
        columnaSubLogros.Width = New GridLength(1, GridUnitType.Star)

        Dim pr As ProgressRing = pagina.FindName("prLogros")
        pr.Visibility = Visibility.Visible

        Dim lvLogros As ListView = pagina.FindName("lvLogros")
        lvLogros.Items.Clear()
        lvLogros.Visibility = Visibility.Visible

        Dim htmlLogros As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=488AE837ADDDA0201B51693B28F1B389&steamid=" + cuenta.ID64 + "&appid=" + juego.ID))
        Dim htmlInfo As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key=488AE837ADDDA0201B51693B28F1B389&appid=" + juego.ID))

        Dim listaLogros As New List(Of Logro)

        If Not htmlLogros = Nothing Then
            If Not htmlInfo = Nothing Then
                Dim i As Integer = 0
                While i < 5000
                    If htmlLogros.Contains(ChrW(34) + "apiname" + ChrW(34)) Then
                        Dim temp, temp2 As String
                        Dim int, int2 As Integer

                        int = htmlLogros.IndexOf(ChrW(34) + "apiname" + ChrW(34))
                        temp = htmlLogros.Remove(0, int + 5)

                        htmlLogros = temp

                        int2 = temp.IndexOf("}")
                        temp2 = temp.Remove(int2, temp.Length - int2)

                        Dim temp3, temp4 As String
                        Dim int3, int4 As Integer

                        int3 = temp2.IndexOf(":")
                        temp3 = temp2.Remove(0, int3 + 1)

                        int4 = temp3.IndexOf(ChrW(34))
                        temp4 = temp3.Remove(0, int4 + 1)

                        int4 = temp4.IndexOf(ChrW(34))
                        temp4 = temp4.Remove(int4, temp4.Length - int4)

                        Dim id As String = temp4.Trim

                        Dim temp5, temp6 As String
                        Dim int5, int6 As Integer

                        int5 = temp2.IndexOf(ChrW(34) + "achieved" + ChrW(34))
                        temp5 = temp2.Remove(0, int5)

                        int5 = temp5.IndexOf(":")
                        temp5 = temp5.Remove(0, int5 + 1)

                        int6 = temp5.IndexOf(",")
                        temp6 = temp5.Remove(int6, temp5.Length - int6)

                        Dim estado As Boolean = False

                        If temp6.Trim = "1" Then
                            estado = True
                        End If

                        If htmlInfo.Contains(ChrW(34) + id + ChrW(34)) Then
                            Dim temp7 As String
                            Dim int7 As Integer

                            int7 = htmlInfo.IndexOf(ChrW(34) + id + ChrW(34))
                            temp7 = htmlInfo.Remove(0, int7 + 4)

                            Dim temp8, temp9 As String
                            Dim int8, int9 As Integer

                            int8 = temp7.IndexOf(ChrW(34) + "displayName" + ChrW(34))
                            temp8 = temp7.Remove(0, int8 + 2)

                            int8 = temp8.IndexOf(":")
                            temp8 = temp8.Remove(0, int8 + 1)

                            int8 = temp8.IndexOf(ChrW(34))
                            temp8 = temp8.Remove(0, int8 + 1)

                            int9 = temp8.IndexOf(ChrW(34))
                            temp9 = temp8.Remove(int9, temp8.Length - int9)

                            Dim nombre As String = temp9.Trim

                            Dim temp10, temp11 As String
                            Dim int10, int11 As Integer

                            int10 = temp7.IndexOf(ChrW(34) + "description" + ChrW(34))

                            Dim descripcion As String = Nothing

                            If Not int10 = -1 Then
                                temp10 = temp7.Remove(0, int10)

                                int10 = temp10.IndexOf(":")
                                temp10 = temp10.Remove(0, int10 + 1)

                                int10 = temp10.IndexOf(ChrW(34))
                                temp10 = temp10.Remove(0, int10 + 1)

                                int11 = temp10.IndexOf(ChrW(34))
                                temp11 = temp10.Remove(int11, temp10.Length - int11)

                                descripcion = temp11.Trim
                            End If

                            Dim temp12, temp13 As String
                            Dim int12, int13 As Integer

                            If estado = True Then
                                int12 = temp7.IndexOf(ChrW(34) + "icon" + ChrW(34))
                            Else
                                int12 = temp7.IndexOf(ChrW(34) + "icongray" + ChrW(34))
                            End If

                            temp12 = temp7.Remove(0, int12 + 2)

                            int12 = temp12.IndexOf(":")
                            temp12 = temp12.Remove(0, int12 + 1)

                            int12 = temp12.IndexOf(ChrW(34))
                            temp12 = temp12.Remove(0, int12 + 1)

                            int13 = temp12.IndexOf(ChrW(34))
                            temp13 = temp12.Remove(int13, temp12.Length - int13)

                            Dim imagen As String = temp13.Trim

                            Dim logro As New Logro(id, estado, nombre, descripcion, imagen, juego)

                            Dim tituloBool As Boolean = False
                            Dim k As Integer = 0
                            While k < listaLogros.Count
                                If listaLogros(k).ID = logro.ID Then
                                    tituloBool = True
                                End If
                                k += 1
                            End While

                            If tituloBool = False Then
                                listaLogros.Add(logro)
                            End If
                        End If
                    Else
                        Exit While
                    End If
                    i += 1
                End While
            End If
        End If

        If listaLogros.Count > 0 Then
            columnaLogros.Width = New GridLength(1, GridUnitType.Auto)
            columnaSubLogros.Width = New GridLength(1, GridUnitType.Auto)

            listaLogros.Sort(Function(x As Logro, y As Logro)
                                 Dim resultado As Integer = y.Estado.CompareTo(x.Estado)
                                 If resultado = 0 Then
                                     resultado = x.Nombre.CompareTo(y.Nombre)
                                 End If
                                 Return resultado
                             End Function)

            For Each logro In listaLogros
                Dim grid As New Grid With {
                    .Tag = logro,
                    .Padding = New Thickness(5, 5, 5, 5)
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
                    .Width = 64,
                    .Height = 64
                }

                Try
                    imagen.Source = New BitmapImage(New Uri(logro.Imagen))
                Catch ex As Exception

                End Try

                imagen.SetValue(Grid.ColumnProperty, 0)
                grid.Children.Add(imagen)

                '-------------------------------

                Dim gridDatos As New Grid

                Dim row1 As New RowDefinition
                Dim row2 As New RowDefinition

                row1.Height = New GridLength(1, GridUnitType.Auto)
                row2.Height = New GridLength(1, GridUnitType.Auto)

                gridDatos.RowDefinitions.Add(row1)
                gridDatos.RowDefinitions.Add(row2)

                Dim textoNombre As New TextBlock With {
                    .Text = logro.Nombre,
                    .VerticalAlignment = VerticalAlignment.Center,
                    .TextWrapping = TextWrapping.Wrap,
                    .MaxWidth = 600,
                    .Margin = New Thickness(10, 5, 10, 5)
                }

                textoNombre.SetValue(Grid.RowProperty, 0)
                gridDatos.Children.Add(textoNombre)

                If Not logro.Descripcion = Nothing Then
                    Dim textoDescripcion As New TextBlock With {
                    .Text = logro.Descripcion,
                    .VerticalAlignment = VerticalAlignment.Center,
                    .TextWrapping = TextWrapping.Wrap,
                    .MaxWidth = 600,
                    .Margin = New Thickness(10, 5, 10, 5)
                }

                    textoDescripcion.SetValue(Grid.RowProperty, 1)
                    gridDatos.Children.Add(textoDescripcion)
                End If

                '-------------------------------

                gridDatos.SetValue(Grid.ColumnProperty, 1)
                grid.Children.Add(gridDatos)

                lvLogros.Items.Add(grid)
            Next
        End If

        pr.Visibility = Visibility.Collapsed

    End Sub

    Public Async Sub CargarLogroDatos(cuentaMaestra As Cuenta, logro As Logro, listaCuentas As List(Of Cuenta))

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim listaNuevaCuentas As New List(Of Cuenta)

        For Each cuenta In listaCuentas
            Dim htmlLogros As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=488AE837ADDDA0201B51693B28F1B389&steamid=" + cuenta.ID64 + "&appid=" + logro.Juego.ID))

            If Not htmlLogros = Nothing Then
                If htmlLogros.Contains(ChrW(34) + logro.ID + ChrW(34)) Then
                    Dim temp As String
                    Dim int As Integer

                    int = htmlLogros.IndexOf(ChrW(34) + logro.ID + ChrW(34))
                    temp = htmlLogros.Remove(0, int)

                    Dim temp2, temp3 As String
                    Dim int2, int3 As Integer

                    int2 = temp.IndexOf(ChrW(34) + "achieved" + ChrW(34))
                    temp2 = temp.Remove(0, int2)

                    int2 = temp2.IndexOf(":")
                    temp2 = temp2.Remove(0, int2 + 1)

                    int3 = temp2.IndexOf(",")
                    temp3 = temp2.Remove(int3, temp2.Length - int3)

                    Dim estado As Boolean = False

                    If temp3.Trim = "1" Then
                        estado = True
                    End If

                    If estado = True Then
                        If Not cuenta.ID64 = cuentaMaestra.ID64 Then
                            listaNuevaCuentas.Add(cuenta)
                        End If
                    End If
                End If
            End If
        Next

        Dim gvCuentas As GridView = pagina.FindName("gvCuentasLogro")
        gvCuentas.Items.Clear()

        Dim tbCuentas As TextBlock = pagina.FindName("tbCuentasLogro")

        If listaNuevaCuentas.Count > 0 Then
            tbCuentas.Visibility = Visibility.Visible

            For Each cuenta In listaNuevaCuentas
                Dim grid As New Grid With {
                    .Padding = New Thickness(5, 5, 5, 5)
                }

                Dim col1 As New ColumnDefinition With {
                    .Width = New GridLength(1, GridUnitType.Auto)
                }

                grid.ColumnDefinitions.Add(col1)

                Dim imagen As New ImageEx With {
                    .Stretch = Stretch.UniformToFill,
                    .IsCacheEnabled = True,
                    .Width = 64,
                    .Height = 64
                }

                Try
                    imagen.Source = New BitmapImage(New Uri(cuenta.Avatar))
                Catch ex As Exception

                End Try

                imagen.SetValue(Grid.ColumnProperty, 0)
                grid.Children.Add(imagen)

                gvCuentas.Items.Add(grid)
            Next
        Else
            tbCuentas.Visibility = Visibility.Collapsed
        End If

    End Sub

End Module
