Public Class Juego

    Public Property ID As String
    Public Property Titulo As String
    Public Property Imagen As String

    Public Sub New(ByVal id As String, ByVal titulo As String, ByVal imagen As String)
        Me.ID = id
        Me.Titulo = titulo
        Me.Imagen = imagen
    End Sub

End Class
