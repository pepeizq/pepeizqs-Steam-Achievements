Imports Microsoft.Toolkit.Uwp.UI.Animations
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.UI.Core

Module Logros

    Public Async Sub Cargar(cuentaMaestra As Cuenta, juego As Juego, listaCuentas As List(Of Cuenta))

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim pr As ProgressRing = pagina.FindName("prLogros")
        pr.Visibility = Visibility.Visible

        Dim lvLogros As ListView = pagina.FindName("lvLogros")
        lvLogros.Items.Clear()
        lvLogros.Visibility = Visibility.Visible

        Dim panel As Grid = pagina.FindName("panelMensajeNoLogros")
        panel.Visibility = Visibility.Collapsed

        Dim iconoYoutube As FontAwesome.UWP.FontAwesome = pagina.FindName("iconoYoutube")
        iconoYoutube.Visibility = Visibility.Visible

        Dim pb As ProgressBar = pagina.FindName("pbJuegoSeleccionado")
        pb.Visibility = Visibility.Collapsed
        pb.Maximum = 100
        pb.Value = 0

        Dim tb As TextBlock = pagina.FindName("tbJuegoSeleccionadoLogros")
        tb.Visibility = Visibility.Collapsed

        Dim listaCuentasHtml As New List(Of CuentaHtml)

        If Not listaCuentas Is Nothing Then
            For Each cuenta In listaCuentas
                Dim htmlCuenta As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + cuenta.ID64 + "&appid=" + juego.ID))
                listaCuentasHtml.Add(New CuentaHtml(cuenta, htmlCuenta))
            Next
        End If

        Dim htmlLogros As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + cuentaMaestra.ID64 + "&appid=" + juego.ID))
        Dim htmlInfo As String = Await Decompiladores.HttpClient(New Uri("https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&appid=" + juego.ID))

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

            For Each logro In listaLogros
                If logro.Estado = True Then
                    conseguidosLogros += 1
                End If

                Dim grid As New Grid With {
                    .Tag = logro,
                    .Padding = New Thickness(0, 5, 0, 5)
                }

                Dim col1 As New ColumnDefinition
                Dim col2 As New ColumnDefinition
                Dim col3 As New ColumnDefinition

                col1.Width = New GridLength(1, GridUnitType.Auto)
                col2.Width = New GridLength(1, GridUnitType.Star)

                grid.ColumnDefinitions.Add(col1)
                grid.ColumnDefinitions.Add(col2)

                Dim borde As New Border With {
                    .BorderBrush = New SolidColorBrush(App.Current.Resources("ColorSecundario")),
                    .BorderThickness = New Thickness(1, 1, 1, 1),
                    .Margin = New Thickness(5, 0, 0, 0),
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

                Dim gridDatos As New Grid With {
                    .Padding = New Thickness(0, 5, 0, 5)
                }

                Dim row1 As New RowDefinition
                Dim row2 As New RowDefinition

                row1.Height = New GridLength(1, GridUnitType.Star)
                row2.Height = New GridLength(1, GridUnitType.Star)

                gridDatos.RowDefinitions.Add(row1)
                gridDatos.RowDefinitions.Add(row2)

                Dim textoNombre As New TextBlock With {
                    .Text = logro.Nombre,
                    .Margin = New Thickness(10, 5, 10, 5),
                    .TextWrapping = TextWrapping.Wrap
                }

                textoNombre.SetValue(Grid.RowProperty, 0)
                gridDatos.Children.Add(textoNombre)

                If Not logro.Descripcion = Nothing Then
                    Dim textoDescripcion As New TextBlock With {
                        .Text = logro.Descripcion,
                        .Margin = New Thickness(10, 5, 10, 5),
                        .TextWrapping = TextWrapping.Wrap
                    }

                    textoDescripcion.SetValue(Grid.RowProperty, 1)
                    gridDatos.Children.Add(textoDescripcion)
                End If

                '-------------------------------

                gridDatos.SetValue(Grid.ColumnProperty, 1)
                grid.Children.Add(gridDatos)

                '-------------------------------

                If listaCuentasHtml.Count > 0 Then
                    Dim sp As New StackPanel With {
                        .Orientation = Orientation.Vertical
                    }

                    Dim spCuentas As New StackPanel With {
                        .Orientation = Orientation.Horizontal,
                        .Margin = New Thickness(0, 10, 0, 0)
                    }

                    For Each cuenta In listaCuentasHtml
                        If Not cuenta.HtmlLogros = Nothing Then
                            If cuenta.HtmlLogros.Contains(ChrW(34) + logro.ID + ChrW(34)) Then
                                Dim temp As String
                                Dim int As Integer

                                int = cuenta.HtmlLogros.IndexOf(ChrW(34) + logro.ID + ChrW(34))
                                temp = cuenta.HtmlLogros.Remove(0, int)

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
                                    If Not cuenta.Cuenta.ID64 = cuentaMaestra.ID64 Then
                                        Dim imagenCuenta As New ImageEx With {
                                            .Stretch = Stretch.UniformToFill,
                                            .IsCacheEnabled = True,
                                            .Width = 50,
                                            .Height = 50,
                                            .Margin = New Thickness(5, 0, 5, 0)
                                        }

                                        Try
                                            imagenCuenta.Source = New BitmapImage(New Uri(cuenta.Cuenta.Avatar))
                                        Catch ex As Exception

                                        End Try

                                        spCuentas.Children.Add(imagenCuenta)
                                    End If
                                End If
                            End If
                        End If
                    Next

                    If spCuentas.Children.Count > 0 Then
                        Dim recursos As New Resources.ResourceLoader()
                        Dim tbUsuarios As TextBlock = New TextBlock With {
                            .Text = recursos.GetString("OtherUsersHasAchievement"),
                            .FontSize = 16
                        }

                        sp.Children.Add(tbUsuarios)
                        sp.Children.Add(spCuentas)

                        ToolTipService.SetToolTip(grid, sp)
                        ToolTipService.SetPlacement(grid, PlacementMode.Mouse)
                    End If

                End If

                AddHandler grid.PointerEntered, AddressOf UsuarioEntraBoton
                AddHandler grid.PointerExited, AddressOf UsuarioSaleBoton

                lvLogros.Items.Add(grid)
            Next
        End If

        If totalLogros > 0 Then
            panel.Visibility = Visibility.Collapsed
            iconoYoutube.Visibility = Visibility.Visible

            pb.Visibility = Visibility.Visible
            pb.Maximum = totalLogros
            pb.Value = conseguidosLogros

            tb.Visibility = Visibility.Visible
            tb.Text = "(" + conseguidosLogros.ToString + "/" + totalLogros.ToString + ")"
        Else
            panel.Visibility = Visibility.Visible
            iconoYoutube.Visibility = Visibility.Visible
        End If

        pr.Visibility = Visibility.Collapsed

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim grid As Grid = sender
        Dim borde As Border = grid.Children(0)
        Dim imagen As ImageEx = borde.Child

        imagen.Saturation(0).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Dim grid As Grid = sender
        Dim borde As Border = grid.Children(0)
        Dim imagen As ImageEx = borde.Child

        imagen.Saturation(1).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

End Module
