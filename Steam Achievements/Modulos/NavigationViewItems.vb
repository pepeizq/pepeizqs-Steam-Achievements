Imports Windows.UI

Module NavigationViewItems

    Public Function Generar(titulo As String, simbolo As SymbolIcon, tag As String, visibilidad As Visibility)

        Dim tb As New TextBlock With {
            .Text = titulo,
            .Foreground = New SolidColorBrush(Colors.White),
            .Tag = titulo
        }

        Dim item As New NavigationViewItem With {
            .Content = tb,
            .Icon = simbolo,
            .Foreground = New SolidColorBrush(Colors.White),
            .Tag = tag,
            .Visibility = visibilidad
        }

        Dim tbToolTip As TextBlock = New TextBlock With {
            .Text = titulo
        }

        ToolTipService.SetToolTip(item, tbToolTip)
        ToolTipService.SetPlacement(item, PlacementMode.Mouse)

        Return item

    End Function

End Module
