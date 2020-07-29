Imports Newtonsoft.Json

Public Class Cuenta

    Public Property ID64 As String
    Public Property Nombre As String
    Public Property Avatar As String

    Public Sub New(ByVal id64 As String, ByVal nombre As String, ByVal avatar As String)
        Me.ID64 = id64
        Me.Nombre = nombre
        Me.Avatar = avatar
    End Sub

End Class

Public Class SteamCuenta

    <JsonProperty("response")>
    Public Property Datos As SteamCuentaDatos

End Class

Public Class SteamCuentaDatos

    <JsonProperty("players")>
    Public Property Jugador As List(Of SteamCuentaDatosJugador)

End Class

Public Class SteamCuentaDatosJugador

    <JsonProperty("steamid")>
    Public Property ID64 As String

    <JsonProperty("personaname")>
    Public Property Nombre As String

    <JsonProperty("avatarfull")>
    Public Property Avatar As String

End Class

Public Class SteamCuentaVanidad

    <JsonProperty("response")>
    Public Property Respuesta As SteamCuentaVanidadID

End Class

Public Class SteamCuentaVanidadID

    <JsonProperty("steamid")>
    Public Property ID64 As String

End Class