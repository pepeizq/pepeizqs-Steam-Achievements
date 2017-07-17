Public Class Logro

    Public Property ID As String
    Public Property Estado As Boolean
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property Imagen As String

    Public Sub New(ByVal id As String, ByVal estado As Boolean, ByVal nombre As String, ByVal descripcion As String, ByVal imagen As String)
        Me.ID = id
        Me.Estado = estado
        Me.Nombre = nombre
        Me.Descripcion = descripcion
        Me.Imagen = imagen
    End Sub

End Class
