Imports Newtonsoft.Json

Public Class Cuenta

    <JsonProperty("response")>
    Public Property Respuesta As CuentaRespuesta

End Class

Public Class CuentaRespuesta

    <JsonProperty("players")>
    Public Property Jugador As List(Of CuentaJugadores)

End Class

Public Class CuentaJugadores

    <JsonProperty("steamid")>
    Public Property ID64 As String

    <JsonProperty("personaname")>
    Public Property Nombre As String

    <JsonProperty("avatarfull")>
    Public Property Avatar As String

End Class

Public Class CuentaVanidad

    <JsonProperty("response")>
    Public Property Respuesta As CuentaVanidadRespuesta

End Class

Public Class CuentaVanidadRespuesta

    <JsonProperty("steamid")>
    Public Property ID64 As String

End Class