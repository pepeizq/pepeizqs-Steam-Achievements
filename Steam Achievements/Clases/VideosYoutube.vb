Imports Newtonsoft.Json

'https://www.youtube.com/oembed?url=http://www.youtube.com/watch?v=HIMNkDu1RPM&format=json
'https://www.googleapis.com/youtube/v3/search?part=snippet&q=cats&type=video&videoCaption=closedCaption&key=AIzaSyADBmNBeOc0PKACOJgjYL2aX_fpe0kLIbQ

Public Class VideosYoutube

    <JsonProperty("items")>
    Public Resultados As List(Of VideosYoutubeResultado)

End Class

Public Class VideosYoutubeResultado

    <JsonProperty("id")>
    Public ID As VideosYoutubeResultadoID

End Class

Public Class VideosYoutubeResultadoID

    <JsonProperty("videoId")>
    Public VideoID As String

End Class