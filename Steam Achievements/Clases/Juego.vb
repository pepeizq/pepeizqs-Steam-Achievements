Public Class Juego

    Public Property ID As String
    Public Property Titulo As String
    Public Property Imagen As String
    Public Property Icono As String

    Public Sub New(ByVal id As String, ByVal titulo As String, ByVal imagen As String, ByVal icono As String)
        Me.ID = id
        Me.Titulo = titulo
        Me.Imagen = imagen
        Me.Icono = icono
    End Sub

End Class
