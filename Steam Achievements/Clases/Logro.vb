Public Class Logro

    Public Property ID As String
    Public Property Estado As Boolean
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property Imagen As String
    Public Property Juego As Juego

    Public Sub New(ByVal id As String, ByVal estado As Boolean, ByVal nombre As String, ByVal descripcion As String,
                   ByVal imagen As String, ByVal juego As Juego)
        Me.ID = id
        Me.Estado = estado
        Me.Nombre = nombre
        Me.Descripcion = descripcion
        Me.Imagen = imagen
        Me.Juego = juego
    End Sub

End Class
