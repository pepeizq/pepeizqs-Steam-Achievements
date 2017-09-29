Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.UI

Module Cuentas

    Public Async Sub Añadir(usuario As String)

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
                        CargarXaml()
                    End If
                End If
            End If
        End If

        tb.IsEnabled = True
        boton.IsEnabled = True

    End Sub

    Public Async Sub CargarXaml()

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

End Module
