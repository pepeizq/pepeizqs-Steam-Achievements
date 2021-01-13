﻿Imports System.Globalization
Imports Microsoft.Toolkit.Uwp.UI.Animations
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.Globalization.NumberFormatting
Imports Windows.System
Imports Windows.UI
Imports Windows.UI.Core

Module MasSteam

    Public Async Sub Cargar()

        Dim recursos As New Resources.ResourceLoader()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim boton As Button = pagina.FindName("botonMasSteam")
        boton.Background = New SolidColorBrush(Colors.Transparent)
        boton.BorderBrush = New SolidColorBrush(Colors.Transparent)
        boton.BorderThickness = New Thickness(0, 0, 0, 0)
        boton.Style = App.Current.Resources("ButtonRevealStyle")

        AddHandler boton.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Basico
        AddHandler boton.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Basico

        Dim spBoton As New StackPanel With {
            .Orientation = Orientation.Horizontal
        }

        Dim icono As New FontAwesome5.FontAwesome With {
            .Foreground = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
            .Icon = FontAwesome5.EFontAwesomeIcon.Brands_Steam,
            .Margin = New Thickness(0, 0, 8, 0),
            .FontSize = 15
        }

        spBoton.Children.Add(icono)

        Dim tb As New TextBlock With {
            .Foreground = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
            .Text = recursos.GetString("MoreSteam")
        }

        spBoton.Children.Add(tb)
        boton.Content = spBoton

        '------------------------------------------

        Dim sp As StackPanel = pagina.FindName("spMasSteam")
        sp.Children.Clear()

        Try
            Dim idsConsultar As New List(Of String) From {
                "9NBLGGH51SB3", 'Tiles
                "9nsd0jrnlmb7", 'Achievements
                "9p7836m1tw15", 'Deals
                "9nblggh55b7f" 'Skins
            }

            Dim i As Integer = 0
            For Each id In idsConsultar
                If id = "9nsd0jrnlmb7" Then
                    Exit For
                End If
                i += 1
            Next
            idsConsultar.RemoveAt(i)

            Dim ids As String = String.Empty

            For Each id In idsConsultar
                ids = ids + id + ","
            Next

            If ids.Length > 0 Then
                ids = ids.Remove(ids.Length - 1)

                Dim idiomas As IReadOnlyList(Of String) = UserProfile.GlobalizationPreferences.Languages
                Dim idioma As String = String.Empty

                If idiomas.Count > 0 Then
                    idioma = idiomas(0)
                Else
                    idioma = "en-us"
                End If

                Dim pais As New Windows.Globalization.GeographicRegion
                Dim html As String = Await HttpClient(New Uri("https://displaycatalog.mp.microsoft.com/v7.0/products?bigIds=" + ids + "&market=" + pais.CodeTwoLetter + "&languages=" + idioma + "&MS-CV=DGU1mcuYo0WMMp+F.1"))

                If Not html = Nothing Then
                    Dim apps As MicrosoftStoreBBDDDetalles = JsonConvert.DeserializeObject(Of MicrosoftStoreBBDDDetalles)(html)

                    If Not apps Is Nothing Then
                        If apps.Apps.Count > 0 Then
                            For Each app2 In apps.Apps
                                Dim fondo As New SolidColorBrush With {
                                    .Opacity = 0.8,
                                    .Color = App.Current.Resources("ColorCuarto")
                                }

                                Dim spApp As New StackPanel With {
                                    .Orientation = Orientation.Vertical,
                                    .Padding = New Thickness(10, 10, 10, 10),
                                    .BorderBrush = New SolidColorBrush(App.Current.Resources("ColorPrimario")),
                                    .BorderThickness = New Thickness(1.5, 1.5, 1.5, 1.5),
                                    .Background = fondo
                                }

                                Dim gridApp As New Grid

                                Dim col1 As New ColumnDefinition
                                Dim col2 As New ColumnDefinition
                                Dim col3 As New ColumnDefinition

                                col1.Width = New GridLength(1, GridUnitType.Auto)
                                col2.Width = New GridLength(1, GridUnitType.Star)
                                col3.Width = New GridLength(1, GridUnitType.Auto)

                                gridApp.ColumnDefinitions.Add(col1)
                                gridApp.ColumnDefinitions.Add(col2)
                                gridApp.ColumnDefinitions.Add(col3)

                                Dim imagenS As String = app2.Detalles(0).Imagenes(0).Enlace

                                If Not imagenS.Contains("http:") Then
                                    imagenS = "http:" + imagenS
                                End If

                                Dim imagen As New ImageEx With {
                                    .IsCacheEnabled = True,
                                    .Width = 60,
                                    .Height = 60,
                                    .Source = imagenS,
                                    .EnableLazyLoading = True
                                }

                                imagen.SetValue(Grid.ColumnProperty, 0)
                                gridApp.Children.Add(imagen)

                                Dim tbTitulo As New TextBlock With {
                                    .Foreground = New SolidColorBrush(Colors.White),
                                    .Text = app2.Detalles(0).Titulo,
                                    .TextWrapping = TextWrapping.Wrap,
                                    .Margin = New Thickness(10, 0, 15, 0),
                                    .VerticalAlignment = VerticalAlignment.Center
                                }

                                tbTitulo.SetValue(Grid.ColumnProperty, 1)
                                gridApp.Children.Add(tbTitulo)

                                Dim precio As String = String.Empty

                                Try
                                    Dim tempDouble As Double = Double.Parse(app2.Propiedades2(0).Disponible(0).Datos.Precio.PrecioRebajado, CultureInfo.InvariantCulture).ToString

                                    Dim moneda As String = app2.Propiedades2(0).Disponible(0).Datos.Precio.Divisa

                                    Dim formateador As New CurrencyFormatter(moneda) With {
                                        .Mode = CurrencyFormatterMode.UseSymbol
                                    }

                                    precio = formateador.Format(tempDouble)
                                Catch ex As Exception

                                End Try

                                If Not precio = String.Empty Then
                                    Dim tbPrecio As New TextBlock With {
                                        .Foreground = New SolidColorBrush(Colors.White),
                                        .Text = precio,
                                        .VerticalAlignment = VerticalAlignment.Center
                                    }

                                    tbPrecio.SetValue(Grid.ColumnProperty, 2)
                                    gridApp.Children.Add(tbPrecio)
                                End If

                                spApp.Children.Add(gridApp)

                                Dim tbDescripcion As New TextBlock With {
                                    .Text = app2.Detalles(0).Descripcion.Trim,
                                    .Foreground = New SolidColorBrush(Colors.White),
                                    .FontSize = 14,
                                    .VerticalAlignment = VerticalAlignment.Center,
                                    .HorizontalAlignment = HorizontalAlignment.Left,
                                    .Margin = New Thickness(0, 15, 0, 0),
                                    .TextWrapping = TextWrapping.Wrap
                                }

                                spApp.Children.Add(tbDescripcion)


                                Dim botonApp As New Button With {
                                    .Tag = app2.ID,
                                    .Content = spApp,
                                    .Padding = New Thickness(0, 0, 0, 0),
                                    .BorderBrush = New SolidColorBrush(Colors.Transparent),
                                    .BorderThickness = New Thickness(0, 0, 0, 0),
                                    .Style = App.Current.Resources("ButtonRevealStyle"),
                                    .Margin = New Thickness(0, 0, 0, 15),
                                    .HorizontalAlignment = HorizontalAlignment.Stretch,
                                    .HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                    .MaxWidth = 400
                                }

                                AddHandler botonApp.Click, AddressOf ComprarAppClick
                                AddHandler botonApp.PointerEntered, AddressOf Entra_Boton
                                AddHandler botonApp.PointerExited, AddressOf Sale_Boton

                                sp.Children.Add(botonApp)
                            Next
                        End If
                    End If
                End If

                If sp.Children.Count > 1 Then
                    Dim botonUltimo As Button = sp.Children(sp.Children.Count - 1)
                    botonUltimo.Margin = New Thickness(0, 0, 0, 0)
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Async Sub ComprarAppClick(sender As Object, e As RoutedEventArgs)

        Dim boton As Button = sender
        Await Launcher.LaunchUriAsync(New Uri("ms-windows-store://pdp/?ProductId=" + boton.Tag))

    End Sub

    Private Sub Entra_Boton(sender As Object, e As PointerRoutedEventArgs)

        Dim boton As Button = sender
        Dim sp As StackPanel = boton.Content

        Dim fondo As New SolidColorBrush With {
            .Opacity = 1,
            .Color = App.Current.Resources("ColorCuarto")
        }

        sp.Background = fondo
        sp.Saturation(1).Scale(1.02, 1.02, sp.ActualWidth / 2, sp.ActualHeight / 2).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub Sale_Boton(sender As Object, e As PointerRoutedEventArgs)

        Dim boton As Button = sender
        Dim sp As StackPanel = boton.Content

        Dim fondo As New SolidColorBrush With {
            .Opacity = 0.8,
            .Color = App.Current.Resources("ColorCuarto")
        }

        sp.Background = fondo
        sp.Saturation(1).Scale(1, 1, sp.ActualWidth / 2, sp.ActualHeight / 2).Start()

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

End Module

Public Class MicrosoftStoreBBDDDetalles

    <JsonProperty("Products")>
    Public Apps As List(Of MicrosoftStoreBBDDDetallesJuego)

End Class

Public Class MicrosoftStoreBBDDDetallesJuego

    <JsonProperty("LocalizedProperties")>
    Public Detalles As List(Of MicrosoftStoreBBDDDetallesJuego2)

    <JsonProperty("ProductId")>
    Public ID As String

    <JsonProperty("DisplaySkuAvailabilities")>
    Public Propiedades2 As List(Of MicrosoftStoreBBDDDetallesPropiedades2)

End Class

Public Class MicrosoftStoreBBDDDetallesJuego2

    <JsonProperty("ProductTitle")>
    Public Titulo As String

    <JsonProperty("ProductDescription")>
    Public Descripcion As String

    <JsonProperty("Images")>
    Public Imagenes As List(Of MicrosoftStoreBBDDDetallesJuego2Imagen)

End Class

Public Class MicrosoftStoreBBDDDetallesJuego2Imagen

    <JsonProperty("ImagePurpose")>
    Public Proposito As String

    <JsonProperty("Uri")>
    Public Enlace As String

    <JsonProperty("ImagePositionInfo")>
    Public Posicion As String

End Class

Public Class MicrosoftStoreBBDDDetallesPropiedades2

    <JsonProperty("Availabilities")>
    Public Disponible As List(Of MicrosoftStoreBBDDDetallesPropiedades2Disponibilidad)

End Class

Public Class MicrosoftStoreBBDDDetallesPropiedades2Disponibilidad

    <JsonProperty("OrderManagementData")>
    Public Datos As MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatos

End Class

Public Class MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatos

    <JsonProperty("Price")>
    Public Precio As MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatosPrecio

End Class

Public Class MicrosoftStoreBBDDDetallesPropiedades2DisponibilidadDatosPrecio

    <JsonProperty("ListPrice")>
    Public PrecioRebajado As String

    <JsonProperty("MSRP")>
    Public PrecioBase As String

    <JsonProperty("CurrencyCode")>
    Public Divisa As String

End Class