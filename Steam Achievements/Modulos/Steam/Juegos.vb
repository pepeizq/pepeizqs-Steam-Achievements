Imports Microsoft.Toolkit.Uwp
Imports Microsoft.Toolkit.Uwp.UI.Controls

Module Juegos

    Public Async Sub Cargar(cuenta As Cuenta)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gvJuegos As GridView = pagina.FindName("gvJuegos")
        gvJuegos.Items.Clear()

        Dim gridBusqueda As Grid = pagina.FindName("gridBusquedaJuegos")
        gridBusqueda.Visibility = Visibility.Collapsed

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

                            Dim temp7, temp8 As String
                            Dim int7, int8 As Integer

                            int7 = htmlJuegos.IndexOf(ChrW(34) + "img_icon_url" + ChrW(34))
                            temp7 = htmlJuegos.Remove(0, int7)

                            int7 = temp7.IndexOf(":")
                            temp7 = temp7.Remove(0, int7 + 1)

                            int7 = temp7.IndexOf(ChrW(34))
                            temp7 = temp7.Remove(0, int7 + 1)

                            int8 = temp7.IndexOf(ChrW(34))
                            temp8 = temp7.Remove(int8, temp7.Length - int8)

                            Dim icono As String = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/apps/" + id + "/" + temp8.Trim + ".jpg"

                            Dim juego As New Juego(id, titulo, imagen, icono)

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
                CargarXaml(listaJuegos)

                gridBusqueda.Visibility = Visibility.Visible
            End If
        End If

        pr.Visibility = Visibility.Collapsed

    End Sub

    Public Sub CargarXaml(listaJuegos As List(Of Juego))

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gvJuegos As GridView = pagina.FindName("gvJuegos")
        gvJuegos.Items.Clear()

        For Each juego In listaJuegos
            Dim grid As New Grid With {
                .Tag = juego
            }

            Dim imagen As New ImageEx With {
                .Stretch = Stretch.UniformToFill,
                .IsCacheEnabled = True,
                .Width = 184,
                .Height = 69
            }

            Dim boolImagen As Boolean = False

            Try
                imagen.Source = New BitmapImage(New Uri(juego.Imagen))
            Catch ex As Exception
                boolImagen = True
            End Try

            If boolImagen = False Then
                grid.Children.Add(imagen)
                gvJuegos.Items.Add(grid)
            End If
        Next

    End Sub

End Module
