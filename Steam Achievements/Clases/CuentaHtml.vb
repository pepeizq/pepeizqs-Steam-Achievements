Public Class CuentaHtml

    Public Property Cuenta As Cuenta
    Public Property HtmlLogros As String

    Public Sub New(ByVal cuenta As Cuenta, ByVal htmllogros As String)
        Me.Cuenta = cuenta
        Me.HtmlLogros = htmllogros
    End Sub

End Class
