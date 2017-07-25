Imports Microsoft.Services.Store.Engagement
Imports Microsoft.Toolkit.Uwp
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Windows.ApplicationModel.Core
Imports Windows.System
Imports Windows.UI

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        Acrilico.Generar(gridTopAcrilico)
        Acrilico.Generar(gridMenuAcrilico)

        Dim barra As ApplicationViewTitleBar = ApplicationView.GetForCurrentView().TitleBar
        barra.ButtonBackgroundColor = Colors.Transparent
        barra.ButtonForegroundColor = Colors.White
        barra.ButtonPressedBackgroundColor = Colors.SaddleBrown
        barra.ButtonInactiveBackgroundColor = Colors.Transparent
        Dim coreBarra As CoreApplicationViewTitleBar = CoreApplication.GetCurrentView.TitleBar
        coreBarra.ExtendViewIntoTitleBar = True

        Application.Current.Resources("SystemControlHighlightListAccentLowBrush") = New SolidColorBrush(Colors.Sienna)
        Application.Current.Resources("SystemControlHighlightListAccentMediumBrush") = New SolidColorBrush(Colors.Sienna)
        Application.Current.Resources("SystemControlHighlightAltBaseHighBrush") = New SolidColorBrush(Colors.White)

        '--------------------------------------------------------

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        botonCuentasTexto.Text = recursos.GetString("Cuentas")
        botonVotarTexto.Text = recursos.GetString("Boton Votar")
        botonMasCosasTexto.Text = recursos.GetString("Boton Cosas")

        botonMasAppsTexto.Text = recursos.GetString("Boton Web")
        botonContactoTexto.Text = recursos.GetString("Boton Contacto")
        botonReportarTexto.Text = recursos.GetString("Boton Reportar")
        botonCodigoFuenteTexto.Text = recursos.GetString("Boton Codigo Fuente")

        tbInfoUsuarioCuenta.Text = recursos.GetString("Info Agregar Usuario")
        botonAgregarUsuarioTexto.Text = recursos.GetString("Boton Agregar Usuario")
        tbInfoUsuarioSeleccionar.Text = recursos.GetString("Info Seleccionar Usuario")

        tbAvisoLogros.Text = recursos.GetString("Info Aviso Logros")

        tbCuentasLogro.Text = recursos.GetString("Info Cuentas Logro")
        botonBuscarGoogleLogroTexto.Text = recursos.GetString("Boton Logro Buscar Google")
        botonBuscarYoutubeLogroTexto.Text = recursos.GetString("Boton Logro Buscar Youtube")

        '----------------------------------------------

        GridVisibilidad(gridCuentas, botonCuentas, recursos.GetString("Cuentas"))
        Cuentas.CargarXaml()

    End Sub

    Private Sub GridVisibilidad(grid As Grid, boton As Button, mensaje As String)

        tbTitulo.Text = "Steam Games Achievements (" + SystemInformation.ApplicationVersion.Major.ToString + "." + SystemInformation.ApplicationVersion.Minor.ToString + "." + SystemInformation.ApplicationVersion.Build.ToString + "." + SystemInformation.ApplicationVersion.Revision.ToString + ") - " + mensaje

        gridCuentas.Visibility = Visibility.Collapsed
        gridJuegos.Visibility = Visibility.Collapsed
        gridLogros.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

        botonCuentas.Background = New SolidColorBrush(Colors.Transparent)
        botonCuentaSeleccionada.Background = New SolidColorBrush(Colors.Transparent)
        botonJuegoSeleccionado.Background = New SolidColorBrush(Colors.Transparent)

        If Not boton Is Nothing Then
            boton.Background = New SolidColorBrush(Colors.Sienna)
        End If

    End Sub

    Private Sub BotonCuentas_Click(sender As Object, e As RoutedEventArgs) Handles botonCuentas.Click

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        GridVisibilidad(gridCuentas, botonCuentas, recursos.GetString("Cuentas"))

    End Sub

    Private Sub BotonCuentaSeleccionada_Click(sender As Object, e As RoutedEventArgs) Handles botonCuentaSeleccionada.Click

        Dim cuenta As Cuenta = imagenCuentaSeleccionada.Tag

        GridVisibilidad(gridJuegos, botonCuentaSeleccionada, cuenta.Nombre)

    End Sub

    Private Sub BotonJuegoSeleccionado_Click(sender As Object, e As RoutedEventArgs) Handles botonJuegoSeleccionado.Click

        Dim juego As Juego = imagenJuegoSeleccionado.Tag

        GridVisibilidad(gridLogros, botonJuegoSeleccionado, juego.Titulo)

    End Sub

    Private Async Sub BotonVotar_Click(sender As Object, e As RoutedEventArgs) Handles botonVotar.Click

        Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))

    End Sub

    Private Sub BotonMasCosas_Click(sender As Object, e As RoutedEventArgs) Handles botonMasCosas.Click

        If popupMasCosas.IsOpen = True Then
            botonMasCosas.Background = New SolidColorBrush(Colors.Transparent)
            popupMasCosas.IsOpen = False
        Else
            botonMasCosas.Background = New SolidColorBrush(Colors.Sienna)
            popupMasCosas.IsOpen = True
        End If

    End Sub

    Private Sub PopupMasCosas_LayoutUpdated(sender As Object, e As Object) Handles popupMasCosas.LayoutUpdated

        popupMasCosas.Height = spMasCosas.ActualHeight

    End Sub

    Private Async Sub BotonMasApps_Click(sender As Object, e As RoutedEventArgs) Handles botonMasApps.Click

        Await Launcher.LaunchUriAsync(New Uri("https://pepeizqapps.com/"))

    End Sub

    Private Async Sub BotonContacto_Click(sender As Object, e As RoutedEventArgs) Handles botonContacto.Click

        Await Launcher.LaunchUriAsync(New Uri("https://pepeizqapps.com/contact/"))

    End Sub

    Private Async Sub BotonReportar_Click(sender As Object, e As RoutedEventArgs) Handles botonReportar.Click

        If StoreServicesFeedbackLauncher.IsSupported = True Then
            Dim ejecutador As StoreServicesFeedbackLauncher = StoreServicesFeedbackLauncher.GetDefault()
            Await ejecutador.LaunchAsync()
        Else
            Await Launcher.LaunchUriAsync(New Uri("https://pepeizqapps.com/contact/"))
        End If

    End Sub

    Private Async Sub BotonCodigoFuente_Click(sender As Object, e As RoutedEventArgs) Handles botonCodigoFuente.Click

        Await Launcher.LaunchUriAsync(New Uri("https://github.com/pepeizq/Steam-Achievements"))

    End Sub

    'LOGROS-----------------------------------------------

    Private Sub TbUsuarioCuenta_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbUsuarioCuenta.TextChanged

        If tbInfoUsuarioCuenta.Text.Length > 4 Then
            botonAgregarUsuario.IsEnabled = True
        Else
            botonAgregarUsuario.IsEnabled = False
        End If

    End Sub

    Private Sub BotonAgregarUsuario_Click(sender As Object, e As RoutedEventArgs) Handles botonAgregarUsuario.Click

        Cuentas.Añadir(tbUsuarioCuenta.Text)

    End Sub

    Private Sub LvUsuarios_ItemClick(sender As Object, e As ItemClickEventArgs) Handles lvUsuarios.ItemClick

        tbBuscarJuegos.Text = String.Empty

        panelMensajeLogros.Visibility = Visibility.Visible
        lvLogros.Items.Clear()

        Dim grid As Grid = e.ClickedItem
        Dim cuenta As Cuenta = grid.Tag

        imagenCuentaSeleccionada.Source = New Uri(cuenta.Avatar)
        imagenCuentaSeleccionada.Tag = cuenta

        botonCuentaSeleccionada.Visibility = Visibility.Visible
        botonCuentaSeleccionadaTexto.Text = cuenta.Nombre

        botonJuegoSeleccionado.Visibility = Visibility.Collapsed

        GridVisibilidad(gridJuegos, botonCuentaSeleccionada, cuenta.Nombre)

        Juegos.Cargar(cuenta)

    End Sub

    Private Async Sub TbBuscarJuegos_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbBuscarJuegos.TextChanged

        gvJuegos.IsEnabled = False

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaJuegosPrevia As List(Of Juego) = Nothing

        If Await helper.FileExistsAsync("listaJuegos") = True Then
            listaJuegosPrevia = Await helper.ReadFileAsync(Of List(Of Juego))("listaJuegos")
        Else
            listaJuegosPrevia = New List(Of Juego)
        End If

        Dim listaJuegosNueva As List(Of Juego) = New List(Of Juego)

        If tbBuscarJuegos.Text.Length > 0 Then
            For Each juego In listaJuegosPrevia
                If juego.Titulo.ToLower.Contains(tbBuscarJuegos.Text.ToLower) = True Then
                    listaJuegosNueva.Add(juego)
                End If
            Next
        Else
            For Each juego In listaJuegosPrevia
                listaJuegosNueva.Add(juego)
            Next
        End If

        Juegos.CargarXaml(listaJuegosNueva)
        gvJuegos.IsEnabled = True

    End Sub

    Private Sub GvJuegos_ItemClick(sender As Object, e As ItemClickEventArgs) Handles gvJuegos.ItemClick

        gridLogrosMasDatos.Visibility = Visibility.Collapsed
        gridBotonesLogro.Visibility = Visibility.Collapsed

        Dim grid As Grid = e.ClickedItem
        Dim juego As Juego = grid.Tag

        Dim cuenta As Cuenta = imagenCuentaSeleccionada.Tag

        imagenJuegoSeleccionado.Source = New Uri(juego.Icono)
        imagenJuegoSeleccionado.Tag = cuenta

        botonJuegoSeleccionado.Visibility = Visibility.Visible
        botonJuegoSeleccionadoTexto.Text = juego.Titulo

        GridVisibilidad(gridLogros, botonJuegoSeleccionado, juego.Titulo)

        Logros.Cargar(cuenta, juego)

    End Sub

    Private Async Sub LvLogros_ItemClick(sender As Object, e As ItemClickEventArgs) Handles lvLogros.ItemClick

        Dim grid As Grid = e.ClickedItem
        Dim logro As Logro = grid.Tag

        If logro.Estado = False Then
            gridBotonesLogro.Visibility = Visibility.Visible
        Else
            gridBotonesLogro.Visibility = Visibility.Collapsed
        End If

        botonBuscarGoogleLogro.Tag = logro
        botonBuscarYoutubeLogro.Tag = logro

        Dim helper As LocalObjectStorageHelper = New LocalObjectStorageHelper
        Dim listaCuentas As List(Of Cuenta) = Nothing

        If Await helper.FileExistsAsync("listaCuentas") = True Then
            listaCuentas = Await helper.ReadFileAsync(Of List(Of Cuenta))("listaCuentas")

            gridLogrosMasDatos.Visibility = Visibility.Visible

            Dim cuenta As Cuenta = imagenCuentaSeleccionada.Tag

            Logros.CargarDatos(cuenta, logro, listaCuentas)
        End If

    End Sub

    Private Async Sub BotonBuscarGoogleLogro_Click(sender As Object, e As RoutedEventArgs) Handles botonBuscarGoogleLogro.Click

        Dim boton As Button = e.OriginalSource
        Dim logro As Logro = boton.Tag

        Dim cadenaBusqueda As String = logro.Juego.Titulo.Replace(" ", "+") + "+" + logro.Nombre.Replace(" ", "+")

        Await Launcher.LaunchUriAsync(New Uri("https://www.google.com/?gws_rd=ssl#q=" + cadenaBusqueda))

    End Sub

    Private Async Sub BotonBuscarYoutubeLogro_Click(sender As Object, e As RoutedEventArgs) Handles botonBuscarYoutubeLogro.Click

        Dim boton As Button = e.OriginalSource
        Dim logro As Logro = boton.Tag

        Dim cadenaBusqueda As String = logro.Juego.Titulo.Replace(" ", "+") + "+" + logro.Nombre.Replace(" ", "+")

        Await Launcher.LaunchUriAsync(New Uri("https://www.youtube.com/results?search_query=" + cadenaBusqueda))

    End Sub

End Class
