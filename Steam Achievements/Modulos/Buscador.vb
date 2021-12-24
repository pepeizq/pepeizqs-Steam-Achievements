Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Steam_Achievements.Interfaz
Imports Windows.ApplicationModel.Core
Imports Windows.Storage
Imports Windows.Storage.FileProperties
Imports Windows.UI.Core

Module Buscador

    Public Sub Cargar()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tbBuscador As TextBox = pagina.FindName("tbBuscarJuegos")

        AddHandler tbBuscador.TextChanged, AddressOf BuscadorTextChanged
        AddHandler tbBuscador.PointerEntered, AddressOf EfectosHover.Entra_Basico
        AddHandler tbBuscador.PointerExited, AddressOf EfectosHover.Sale_Basico

    End Sub

    Private Async Sub BuscadorTextChanged(sender As Object, e As TextChangedEventArgs)

        If CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess = True Then
            Await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                                                              Sub()
                                                                  Buscar(sender)
                                                              End Sub)
        End If

    End Sub

    Private Async Sub Buscar(tb As TextBox)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        Dim cuenta As Cuenta = nvJuegos.Tag

        Dim prBuscador As ProgressRing = pagina.FindName("prBuscador")
        prBuscador.IsActive = True
        prBuscador.Visibility = Visibility.Visible

        Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")
        gv.Items.Clear()

        Dim helper As New LocalObjectStorageHelper

        Dim listaJuegos As New List(Of Juego)
        Dim listaBusqueda As New List(Of BusquedaFichero)

        If tb.Text.Trim.Length > 3 Then
            If Await helper.FileExistsAsync("busqueda_" + cuenta.ID64) Then
                listaBusqueda = Await helper.ReadFileAsync(Of List(Of BusquedaFichero))("busqueda_" + cuenta.ID64)
            End If

            If Not listaBusqueda Is Nothing Then
                If listaBusqueda.Count > 0 Then
                    For Each busqueda In listaBusqueda
                        If LimpiarBusqueda(busqueda.Titulo).ToString.Contains(LimpiarBusqueda(tb.Text.Trim.ToLower)) Then
                            Dim temp As Juego = Nothing

                            Try
                                temp = Await helper.ReadFileAsync(Of Juego)(busqueda.Fichero)
                            Catch ex As Exception

                            End Try

                            If Not temp Is Nothing Then
                                Dim añadir As Boolean = True

                                If Not listaJuegos Is Nothing Then
                                    If listaJuegos.Count > 0 Then
                                        For Each juego In listaJuegos
                                            If temp.ID = juego.ID Then
                                                añadir = False
                                            End If
                                        Next
                                    End If
                                End If

                                If añadir = True Then
                                    listaJuegos.Add(temp)
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            If listaJuegos Is Nothing Then
                listaJuegos = New List(Of Juego)
            End If

            gv.Items.Clear()

            If listaJuegos.Count > 0 Then
                listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

                For Each juego In listaJuegos
                    Steam.Juegos.BotonEstilo(juego, gv)
                Next
            End If

        ElseIf tb.Text.Trim.Length = 0 Then

            Dim carpetaFicheros As StorageFolder = Nothing

            Try
                carpetaFicheros = Await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\Juegos_" + cuenta.ID64)
            Catch ex As Exception

            End Try

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

            gv.Items.Clear()

            If listaJuegos.Count > 0 Then
                listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

                For Each juego In listaJuegos
                    Steam.Juegos.BotonEstilo(juego, gv)
                Next
            End If
        End If

        prBuscador.IsActive = False
        prBuscador.Visibility = Visibility.Collapsed

    End Sub

    Private Function LimpiarBusqueda(texto As String)

        Dim listaCaracteres As New List(Of String) From {"Early Access", " ", "•", ">", "<", "¿", "?", "!", "¡", ":",
                ".", "_", "–", "-", ";", ",", "™", "®", "'", "’", "´", "`", "(", ")", "/", "\", "|", "&", "#", "=", ChrW(34),
                "@", "^", "[", "]", "ª", "«"}

        For Each item In listaCaracteres
            texto = texto.Replace(item, Nothing)
        Next

        texto = texto.ToLower
        texto = texto.Trim

        Return texto
    End Function

End Module

Public Class BusquedaFichero

    Public Titulo As String
    Public Fichero As String

    Public Sub New(titulo As String, fichero As String)
        Me.Titulo = titulo
        Me.Fichero = fichero
    End Sub
End Class