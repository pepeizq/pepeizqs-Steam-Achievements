Namespace Interfaz
    Module Pestañas

        Public Sub Visibilidad_Pestañas(gridMostrar As Grid, tag As String)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbTitulo As TextBlock = pagina.FindName("tbTitulo")
            tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ")"

            If Not tag = Nothing Then
                tbTitulo.Text = tbTitulo.Text + " • " + tag
            End If

            Dim gridCuentas As Grid = pagina.FindName("gridCuentas")
            gridCuentas.Visibility = Visibility.Collapsed

            Dim gridJuegos As Grid = pagina.FindName("gridJuegos")
            gridJuegos.Visibility = Visibility.Collapsed

            Dim gridLogros As Grid = pagina.FindName("gridLogros")
            gridLogros.Visibility = Visibility.Collapsed

            Dim gridPermisos As Grid = pagina.FindName("gridPermisos")
            gridPermisos.Visibility = Visibility.Collapsed

            Dim gridMasCosas As Grid = pagina.FindName("gridMasCosas")
            gridMasCosas.Visibility = Visibility.Collapsed

            gridMostrar.Visibility = Visibility.Visible

        End Sub

    End Module
End Namespace

