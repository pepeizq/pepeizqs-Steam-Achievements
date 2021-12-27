Imports Newtonsoft.Json

Public Class Juego

    Public Property ID As String
    Public Property Titulo As String
    Public Property Imagen As String
    Public Property Icono As String
    Public Property Logros As List(Of Logro)
    Public Property Escaneado As Boolean

    Public Sub New(id As String, titulo As String, imagen As String, icono As String, logros As List(Of Logro))
        Me.ID = id
        Me.Titulo = titulo
        Me.Imagen = imagen
        Me.Icono = icono
        Me.Logros = logros
    End Sub

End Class

Public Class SteamJuegos

    <JsonProperty("response")>
    Public Respuesta As SteamJuegosRespuesta

End Class

Public Class SteamJuegosRespuesta

    <JsonProperty("game_count")>
    Public CantidadJuegos As String

    <JsonProperty("games")>
    Public Juegos As List(Of SteamJuegosRespuestaJuego)

End Class

Public Class SteamJuegosRespuestaJuego

    <JsonProperty("appid")>
    Public ID As String

    <JsonProperty("name")>
    Public Titulo As String

    <JsonProperty("img_icon_url")>
    Public Icono As String

End Class
