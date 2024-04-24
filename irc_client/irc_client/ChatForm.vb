Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Meebey.SmartIrc4net
Module GlobalModule
    Public ignoreList As New List(Of String)()
End Module
Public Class privado
    Private zoomFactor As Single = 1.0

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function FlashWindowEx(ByRef pwfi As FLASHWINFO) As Boolean
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure FLASHWINFO
        Public cbSize As UInteger
        Public hwnd As IntPtr
        Public dwFlags As UInteger
        Public uCount As UInteger
        Public dwTimeout As UInteger
    End Structure

    Private Const FLASHW_ALL As UInteger = &H3
    Private Const FLASHW_TIMERNOFG As UInteger = &HC
    Public ReadOnly targetNick As String
    Private ReadOnly irc As IrcClient
    Private WithEvents mainForm As Form1 ' Nuevo parámetro para referenciar al formulario principal


    '  Private ReadOnly _richTextBox1Form1 As RichTextBox
    'Private ReadOnly _fontSize As Single

    ' Nuevo parámetro para referenciar al formulario principal
    Private fontSize As Single ' Tamaño del texto
    Public Sub FlashTaskbarIcon(hwnd As IntPtr)
        Dim fi As FLASHWINFO = New FLASHWINFO With {
        .cbSize = Convert.ToUInt32(Marshal.SizeOf(GetType(FLASHWINFO))),
        .hwnd = hwnd,
        .dwFlags = FLASHW_ALL Or FLASHW_TIMERNOFG,
        .uCount = UInt32.MaxValue,
        .dwTimeout = 0
    }
        FlashWindowEx(fi)
    End Sub
    Public Sub New(targetNick As String, irc As IrcClient, mainForm As Form1, fontSize As Single)
        InitializeComponent()
        Me.targetNick = If(targetNick.StartsWith("@") OrElse targetNick.StartsWith("+"), targetNick.Substring(1), targetNick)
        Me.irc = irc
        Me.mainForm = mainForm
        Me.fontSize = fontSize ' Guardar el tamaño de texto proporcionado
        Me.Text = "Chat privado con " & targetNick
        RichTextBox1.ZoomFactor = fontSize
    End Sub
    Public Sub AddMessage(message As String)
        ' Invocar el método en el hilo principal de manera asincrónica
        If RichTextBox1.InvokeRequired Then
            RichTextBox1.Invoke(Sub() AddMessage(message))
            'Me.TopMost = True
            ' Me.TopMost = False
            RichTextBox1.SelectionStart = RichTextBox1.TextLength
            ' Mover el scroll al final del texto
            RichTextBox1.ScrollToCaret()
            FlashTaskbarIcon(Me.Handle)
        Else
            ' Mostrar el mensaje en el cuadro de texto de mensajes
            Dim elnick() As String = message.Trim().Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)

            Dim cogernick As String = elnick(1)
            Dim startIndex As Integer = RichTextBox1.TextLength
            DecodeAndDisplayColors(message)
            ' RichTextBox1.AppendText($"{message}")
            RichTextBox1.Select(startIndex, cogernick.Length + 10) ' Sumamos 2 para incluir los símbolos < y >
            RichTextBox1.SelectionColor = Color.DarkBlue

            'Me.TopMost = True
            'Me.TopMost = False
            RichTextBox1.SelectionStart = RichTextBox1.TextLength
            ' Mover el scroll al final del texto
            RichTextBox1.ScrollToCaret()
            FlashTaskbarIcon(Me.Handle)
        End If
    End Sub
    Public Sub SincronizarTamaño()
        RichTextBox1.ZoomFactor = mainForm.RichTextBox1.ZoomFactor
    End Sub
    Private Sub DecodeAndDisplayColors(input As String)
        Me.Invoke(Sub()

                      Dim regex As New Regex("\u0003(\d{1,2})(,(\d{1,2}))?")
                      Dim matches As MatchCollection = regex.Matches(input)

                      Dim currentPosition As Integer = 0
                      For Each match As Match In matches
                          Dim colorCode As Integer = Integer.Parse(match.Groups(1).Value)
                          Dim backgroundColorCode As Integer = If(match.Groups(3).Success, Integer.Parse(match.Groups(3).Value), -1)

                          Dim textLength As Integer = match.Index - currentPosition
                          If textLength > 0 Then
                              RichTextBox1.AppendText(input.Substring(currentPosition, textLength))
                          End If

                          Dim startIndex As Integer = RichTextBox1.TextLength
                          Dim length As Integer = match.Length - 1 ' Exclude the escape character '\u0003'

                          ' Apply color
                          If colorCode >= 0 AndAlso colorCode <= 15 Then
                              RichTextBox1.SelectionColor = GetIRCColor(colorCode)
                          End If

                          ' Apply background color if provided
                          If backgroundColorCode >= 0 AndAlso backgroundColorCode <= 15 Then
                              RichTextBox1.SelectionBackColor = GetIRCColor(backgroundColorCode)
                          End If

                          currentPosition = match.Index + match.Length
                      Next

                      ' Append the remaining text
                      If currentPosition < input.Length Then
                          RichTextBox1.AppendText(input.Substring(currentPosition))
                      End If
                  End Sub)
    End Sub

    Private Function GetIRCColor(colorCode As Integer) As Color
        Select Case colorCode
            Case 0 : Return Color.White ' White
            Case 1 : Return Color.Black ' Black
            Case 2 : Return Color.Blue ' Blue
            Case 3 : Return Color.Green ' Green
            Case 4 : Return Color.Red ' Red
            Case 5 : Return Color.Brown ' Brown
            Case 6 : Return Color.Purple ' Purple
            Case 7 : Return Color.Orange ' Orange
            Case 8 : Return Color.Yellow ' Yellow
            Case 9 : Return Color.LightGreen ' Light Green
            Case 10 : Return Color.Teal ' Teal
            Case 11 : Return Color.LightCyan ' Light Cyan
            Case 12 : Return Color.LightBlue ' Light Blue
            Case 13 : Return Color.Pink ' Pink
            Case 14 : Return Color.Gray ' Gray
            Case 15 : Return Color.LightGray ' Light Gray
            Case Else : Return Color.Black ' Default to Black
        End Select
    End Function

    Private Sub txtSendMessage_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtSendMessage.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Return) Then
            Dim message As String = txtSendMessage.Text

            ' Verificar que el mensaje no esté vacío
            If Not String.IsNullOrEmpty(message) Then
                ' Dividir el mensaje en líneas
                Dim lines() As String = message.Split({Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)

                ' Enviar cada línea del mensaje al destino especificado
                For Each line As String In lines
                    ' Eliminar espacios en blanco adicionales al principio y al final de la línea
                    Dim trimmedLine As String = line.Trim()

                    ' Verificar si la línea está vacía después de quitar los espacios en blanco adicionales
                    If Not String.IsNullOrEmpty(trimmedLine) Then
                        ' Enviar la línea al destino especificado
                        Me.irc.RfcPrivmsg(targetNick, trimmedLine)

                        ' Mostrar el mensaje enviado en el cuadro de texto de mensajes
                        Dim startIndex As Integer = RichTextBox1.TextLength
                        Dim horaActual As DateTime = DateTime.Now
                        Dim hora As String = horaActual.ToString("HH") ' Asegura que siempre tenga 2 dígitos
                        Dim minutos As String = horaActual.ToString("mm")
                        Dim msg As String = $"{hora}:{minutos}"

                        ' Construir el mensaje completo a mostrar en el RichTextBox
                        Dim displayMsg As String = $"{msg}     <{mainForm.nick}> {trimmedLine}" & vbCrLf

                        ' Mostrar el mensaje en el RichTextBox
                        RichTextBox1.SuspendLayout()
                        DecodeAndDisplayColors(displayMsg)
                        RichTextBox1.Select(startIndex, mainForm.nick.Length + 12) ' Sumamos 2 para incluir los símbolos < y >
                        RichTextBox1.SelectionColor = Color.MediumBlue
                        RichTextBox1.ResumeLayout()
                    End If
                Next

                ' Limpiar el cuadro de texto después de enviar el mensaje
                txtSendMessage.Clear()

                ' Mover

            End If
        End If
    End Sub
    Private Sub RichTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles RichTextBox1.KeyDown
        ' Si se presiona Ctrl + C en el RichTextBox, copia el texto seleccionado.
        If e.Control AndAlso e.KeyCode = Keys.C Then
            ' MsgBox(RichTextBox1.SelectedText)
            ' Verificar si hay texto seleccionado en el RichTextBox

            ' Copiar el texto seleccionado al portapapeles
            Clipboard.SetText(RichTextBox1.SelectedText)

        End If
    End Sub
    Private Sub RichTextBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles RichTextBox1.MouseUp

        If RichTextBox1.SelectionLength = 0 Then

            txtSendMessage.Focus()
        End If

    End Sub
    Private Sub privado_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' Remover esta ventana privada del diccionario en el formulario principal
        Exit Sub
    End Sub

    Private Sub privado_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        txtSendMessage.Focus()

        ' RichTextBox1.Font = New Font(RichTextBox1.Font.FontFamily, _fontSize)
        If mainForm.turquesa Then
            RichTextBox1.BackColor = Color.LightSeaGreen
            txtSendMessage.BackColor = Color.DarkTurquoise
        ElseIf mainForm.dark Then
            RichTextBox1.BackColor = Color.DarkGray
            txtSendMessage.BackColor = Color.WhiteSmoke
        ElseIf mainForm.orange Then
            RichTextBox1.BackColor = Color.Coral
            txtSendMessage.BackColor = Color.SandyBrown
        ElseIf mainForm.gold Then
            RichTextBox1.BackColor = Color.Goldenrod
            txtSendMessage.BackColor = Color.Khaki
        End If
        ' RichTextBox1.Font = New Font(RichTextBox1.Font.FontFamily, GlobalVariables.RichTextBoxTextSize)
    End Sub

    Private Sub PonerIgnoreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PonerIgnoreToolStripMenuItem.Click
        ignoreList.Add(targetNick)
        Me.Close()
    End Sub

    Private Sub txtSendMessage_TextChanged(sender As Object, e As EventArgs) Handles txtSendMessage.TextChanged

    End Sub






    ' Variable para almacenar el formato del nuevo mensaje


End Class
