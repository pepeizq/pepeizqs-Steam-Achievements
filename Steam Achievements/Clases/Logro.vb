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

Public Class SteamLogros

    <JsonProperty("playerstats")>
    Public Datos As SteamLogrosDatos

End Class

Public Class SteamLogrosDatos

    <JsonProperty("success")>
    Public Estado As Boolean

    <JsonProperty("steamID")>
    Public UsuarioID As String

    <JsonProperty("gameName")>
    Public TituloJuego As String

    <JsonProperty("achievements")>
    Public Logros As List(Of SteamLogrosDatosLogro)

End Class

Public Class SteamLogrosDatosLogro

    <JsonProperty("apiname")>
    Public NombreAPI As String

    <JsonProperty("achieved")>
    Public Estado As String

    <JsonProperty("unlocktime")>
    Public Fecha As String

End Class
