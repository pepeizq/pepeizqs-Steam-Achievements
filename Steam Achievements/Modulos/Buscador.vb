Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls

Module Buscador

    Public Sub Cargar()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tbBuscador As TextBox = pagina.FindName("tbBuscarJuegos")

        AddHandler tbBuscador.TextChanged, AddressOf Buscar

    End Sub

    Private Async Sub Buscar(sender As Object, e As TextChangedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim tbBuscador As TextBox = sender
        Dim gv As AdaptiveGridView = pagina.FindName("gvJuegos")

        Dim nvPrincipal As NavigationView = pagina.FindName("nvPrincipal")

        Dim nvJuegos As NavigationViewItem = nvPrincipal.MenuItems(1)
        Dim cuenta As Cuenta = nvJuegos.Tag

        Dim helper As New LocalObjectStorageHelper

        Dim listaJuegos As New List(Of Juego)

        If Await helper.FileExistsAsync("listaJuegos" + cuenta.ID64) = True Then
            listaJuegos = Await helper.ReadFileAsync(Of List(Of Juego))("listaJuegos" + cuenta.ID64)
        End If

        If Not listaJuegos Is Nothing Then
            gv.Items.Clear()

            listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

            If tbBuscador.Text.Trim.Length > 0 Then
                For Each juego In listaJuegos
                    Dim busqueda As String = tbBuscador.Text.Trim

                    If LimpiarBusqueda(juego.Titulo).ToString.Contains(LimpiarBusqueda(busqueda)) Then
                        Steam.Juegos.BotonEstilo(juego, gv)
                    End If
                Next
            Else
                For Each juego In listaJuegos
                    Steam.Juegos.BotonEstilo(juego, gv)
                Next
            End If
        End If

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
