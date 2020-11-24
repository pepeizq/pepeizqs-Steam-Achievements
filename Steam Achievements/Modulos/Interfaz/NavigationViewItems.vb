Imports Windows.UI

Namespace Interfaz
    Module NavigationViewItems

        Public Function Generar(titulo As String, simbolo As FontAwesome5.EFontAwesomeIcon)

            Dim tb As New TextBlock With {
                .Text = titulo,
                .Foreground = New SolidColorBrush(App.Current.Resources("ColorPrimario"))
            }

            Dim icono As New FontAwesome5.FontAwesome With {
                .Icon = simbolo,
                .Foreground = New SolidColorBrush(App.Current.Resources("ColorPrimario"))
            }

            Dim item As New NavigationViewItem With {
                .Content = tb,
                .Icon = icono,
                .Foreground = New SolidColorBrush(Colors.White)
            }

            Dim tbToolTip As TextBlock = New TextBlock With {
                .Text = titulo
            }

            ToolTipService.SetToolTip(item, tbToolTip)
            ToolTipService.SetPlacement(item, PlacementMode.Mouse)

            AddHandler item.PointerEntered, AddressOf Entra_NVItem_Icono
            AddHandler item.PointerExited, AddressOf Sale_NVItem_Icono

            Return item

        End Function

    End Module
End Namespace

