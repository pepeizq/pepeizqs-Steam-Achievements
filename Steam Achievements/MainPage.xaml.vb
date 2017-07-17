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
        barra.ButtonPressedBackgroundColor = Colors.DarkRed
        barra.ButtonInactiveBackgroundColor = Colors.Transparent
        Dim coreBarra As CoreApplicationViewTitleBar = CoreApplication.GetCurrentView.TitleBar
        coreBarra.ExtendViewIntoTitleBar = True

        '--------------------------------------------------------

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()

        botonLogrosIntroTexto.Text = recursos.GetString("Logros")
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

        '----------------------------------------------

        GridVisibilidad(gridLogrosIntro, botonLogrosIntro)
        Steam.CargarCuentas()

    End Sub

    Private Sub GridVisibilidad(grid As Grid, boton As Button)

        tbTitulo.Text = "Steam Achievements (" + SystemInformation.ApplicationVersion.Major.ToString + "." + SystemInformation.ApplicationVersion.Minor.ToString + "." + SystemInformation.ApplicationVersion.Build.ToString + "." + SystemInformation.ApplicationVersion.Revision.ToString + ")"

        gridLogrosIntro.Visibility = Visibility.Collapsed
        gridLogrosExpandido.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

        botonLogrosIntro.Background = New SolidColorBrush(Colors.Transparent)
        botonLogrosExpandido.Background = New SolidColorBrush(Colors.Transparent)

        If Not boton Is Nothing Then
            boton.Background = New SolidColorBrush(Colors.IndianRed)
        End If

    End Sub

    Private Sub BotonLogrosIntro_Click(sender As Object, e As RoutedEventArgs) Handles botonLogrosIntro.Click

        GridVisibilidad(gridLogrosIntro, botonLogrosIntro)

    End Sub

    Private Sub BotonLogrosExpandido_Click(sender As Object, e As RoutedEventArgs) Handles botonLogrosExpandido.Click

        GridVisibilidad(gridLogrosExpandido, botonLogrosExpandido)

    End Sub

    Private Async Sub BotonVotar_Click(sender As Object, e As RoutedEventArgs) Handles botonVotar.Click

        Await Launcher.LaunchUriAsync(New Uri("ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName))

    End Sub

    Private Sub BotonMasCosas_Click(sender As Object, e As RoutedEventArgs) Handles botonMasCosas.Click

        If popupMasCosas.IsOpen = True Then
            botonMasCosas.Background = New SolidColorBrush(Colors.Transparent)
            popupMasCosas.IsOpen = False
        Else
            botonMasCosas.Background = New SolidColorBrush(Colors.IndianRed)
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

        Steam.AñadirCuenta(tbUsuarioCuenta.Text)

    End Sub

    Private Sub LvUsuarios_ItemClick(sender As Object, e As ItemClickEventArgs) Handles lvUsuarios.ItemClick

        GridVisibilidad(gridLogrosExpandido, botonLogrosExpandido)

        tbBuscarJuegos.Text = String.Empty
        lvLogros.Items.Clear()

        Dim grid As Grid = e.ClickedItem
        Dim cuenta As Cuenta = grid.Tag

        imagenLogrosExpandido.Source = New Uri(cuenta.Avatar)
        imagenLogrosExpandido.Tag = cuenta

        botonLogrosExpandidoTexto.Text = cuenta.Nombre
        botonLogrosExpandido.Visibility = Visibility.Visible

        Steam.CargarJuegos(cuenta)

    End Sub

    Private Async Sub TbBuscarJuegos_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbBuscarJuegos.TextChanged

        lvJuegos.IsEnabled = False

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

        Steam.CargarListadoJuegos(listaJuegosNueva)
        lvJuegos.IsEnabled = True

    End Sub

    Private Sub LvJuegos_ItemClick(sender As Object, e As ItemClickEventArgs) Handles lvJuegos.ItemClick

        Dim grid As Grid = e.ClickedItem
        Dim juego As Juego = grid.Tag

        Dim cuenta As Cuenta = imagenLogrosExpandido.Tag

        Steam.CargarLogros(cuenta, juego)

    End Sub


End Class
