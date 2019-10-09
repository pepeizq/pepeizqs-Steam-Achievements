Imports Newtonsoft.Json

'https://www.youtube.com/oembed?url=http://www.youtube.com/watch?v=HIMNkDu1RPM&format=json

Public Class YouTube

    <JsonProperty("html")>
    Public Property Html As String

End Class