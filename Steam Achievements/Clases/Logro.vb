Imports Newtonsoft.Json

Public Class Logro

    Public Property ID As String
    Public Property Estado As String
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property Imagen As String
    Public Property Fecha As String

    Public Sub New(ByVal id As String, ByVal estado As String, ByVal nombre As String, ByVal descripcion As String,
                   ByVal fecha As String, ByVal imagen As String)
        Me.ID = id
        Me.Estado = estado
        Me.Nombre = nombre
        Me.Descripcion = descripcion
        Me.Fecha = fecha
        Me.Imagen = imagen
    End Sub

End Class

Public Class LogrosOtraCuenta

    Public Property Cuenta As Cuenta
    Public Property LogrosJuego As SteamJugadorLogros

    Public Sub New(ByVal cuenta As Cuenta, ByVal logrosJuego As SteamJugadorLogros)
        Me.Cuenta = cuenta
        Me.LogrosJuego = logrosJuego
    End Sub

End Class

'------------------------------------------------------------

Public Class SteamJugadorLogros

    <JsonProperty("playerstats")>
    Public Datos As SteamJugadorLogrosDatos

End Class

Public Class SteamJugadorLogrosDatos

    <JsonProperty("success")>
    Public Estado As Boolean

    <JsonProperty("steamID")>
    Public UsuarioID As String

    <JsonProperty("gameName")>
    Public TituloJuego As String

    <JsonProperty("achievements")>
    Public Logros As List(Of SteamJugadorLogrosDatosLogro)

End Class

Public Class SteamJugadorLogrosDatosLogro

    <JsonProperty("apiname")>
    Public NombreAPI As String

    <JsonProperty("achieved")>
    Public Estado As String

    <JsonProperty("unlocktime")>
    Public Fecha As String

End Class

'-------------------------------------------------------------------------

Public Class SteamJuegoLogros

    <JsonProperty("game")>
    Public Datos As SteamJuegoLogrosDatos

End Class

Public Class SteamJuegoLogrosDatos

    <JsonProperty("gameName")>
    Public TituloJuego As String

    <JsonProperty("availableGameStats")>
    Public Datos2 As SteamJuegoLogrosDatos2

End Class

Public Class SteamJuegoLogrosDatos2

    <JsonProperty("achievements")>
    Public Logros As List(Of SteamJuegoLogrosDatosLogro)

End Class

Public Class SteamJuegoLogrosDatosLogro

    <JsonProperty("name")>
    Public NombreAPI As String

    <JsonProperty("displayName")>
    Public NombreMostrar As String

    <JsonProperty("description")>
    Public Descripcion As String

    <JsonProperty("icon")>
    Public IconoCompletado As String

    <JsonProperty("icongray")>
    Public IconoPendiente As String

End Class
