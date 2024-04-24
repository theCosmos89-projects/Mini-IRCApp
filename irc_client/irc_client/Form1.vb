Imports Meebey.SmartIrc4net
Imports System.ComponentModel
Imports System.IO
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Security
Imports System.Text.RegularExpressions
Imports System.Threading


Public Class Form1
    Private rfcNamesCalled As Boolean = False
    Dim fall As Boolean
    Dim zoom As Single = 0.08F
    Private irc As IrcClient
    'Dim oldtotalzoom As Single = 0
    Dim totalzoom As Single = 0
    Dim addtopic As String
    Dim usersin As Integer
    Dim op As Boolean = False
    Dim deop As Boolean = False
    Dim prohivido As Boolean = False
    Dim regnick As Boolean = False
    ' Dim nochange As Boolean = False
    Dim ban As Boolean = False
    Dim conected As Boolean = False
    Dim maxmin As Integer
    Dim qnick As String
    Private isscrolling As Boolean = False
    Private channelTopics As New Dictionary(Of String, String)()
    Public turquesa As Boolean = False
    Public orange As Boolean = False
    Public dark As Boolean = False
    Public gold As Boolean = False
    Dim winname As String = "Cliente de IRC creado por cosmos89 - "
    Public ircClients As New List(Of IrcClient)()
    Private ttopic As String = ""
    Public nick As String = ""
    Private pass As String = ""
    Private canal As String = ""
    Private newnick As String = ""
    Dim msg As String
    Dim nickreg As String

    Private currentChannel As String = ""
    Private privateChatForms As New Dictionary(Of String, privado)()
    Private channelText As New Dictionary(Of String, String)()

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged
        If Not isscrolling Then
            RichTextBox1.SelectionStart = RichTextBox1.Text.Length
            RichTextBox1.ScrollToCaret()
        End If
    End Sub

    Private Sub RichTextBox1_MouseWheel(sender As Object, e As MouseEventArgs) Handles RichTextBox1.MouseWheel
        isscrolling = True
    End Sub

    Private Sub RichTextBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles RichTextBox1.MouseDown
        isscrolling = True
    End Sub

    Private Sub RichTextBox1_MouseLeave(sender As Object, e As EventArgs) Handles RichTextBox1.MouseLeave
        isscrolling = False
    End Sub

    Private Sub RichTextBox1_Scroll(sender As Object, e As EventArgs) Handles RichTextBox1.VScroll
        isscrolling = False
    End Sub

    Private Sub EliminarItemsDuplicados(listBox As ListBox)
        Dim itemsUnicos As New List(Of Object)

        ' Recorre los elementos del ListBox y agrega los únicos a una lista temporal
        For Each item As Object In listBox.Items
            If Not itemsUnicos.Contains(item) Then
                itemsUnicos.Add(item)
            End If
        Next

        ' Limpia el ListBox y luego agrega los elementos únicos de la lista temporal
        listBox.Items.Clear()
        For Each item As Object In itemsUnicos
            listBox.Items.Add(item)
        Next
    End Sub
    Private Sub OnOp(sender As Object, e As IrcEventArgs)
        Me.Invoke(Sub()
                      If e.Data.Channel = currentChannel Then
                          ListBox1.Items.Clear()
                          ircClients.Last().RfcNames(currentChannel)
                          EliminarItemsDuplicados(ListBox1)
                      End If

                  End Sub)
    End Sub
    Private Sub OnDeop(sender As Object, e As IrcEventArgs)
        Me.Invoke(Sub()

                      If e.Data.Channel = currentChannel Then
                          ListBox1.Items.Clear()
                          ircClients.Last().RfcNames(currentChannel)
                          EliminarItemsDuplicados(ListBox1)
                      End If

                  End Sub)
    End Sub

    Private Sub OnDevoice(sender As Object, e As IrcEventArgs)
        Me.Invoke(Sub()
                      If e.Data.Channel = currentChannel Then
                          ListBox1.Items.Clear()
                          ircClients.Last().RfcNames(currentChannel)
                          EliminarItemsDuplicados(ListBox1)
                      End If
                  End Sub)
    End Sub




    Private Sub OnVoice(sender As Object, e As IrcEventArgs)
        Me.Invoke(Sub()
                      If e.Data.Channel = currentChannel Then
                          ListBox1.Items.Clear()
                          ircClients.Last().RfcNames(currentChannel)
                          EliminarItemsDuplicados(ListBox1)
                      End If

                  End Sub)
    End Sub




    Private Sub OnKick(sender As Object, e As KickEventArgs)
        Me.Invoke(Sub()
                      Dim horaActual As DateTime = DateTime.Now
                      Dim hora As String = horaActual.ToString("HH:mm") ' Asegura que siempre tenga 4 dígitos
                      Dim msg As String = ""
                      Dim arrPnick As String = "@" & e.Whom ' o "-" & pnick según tu requerimiento
                      Dim voicePnick As String = "+" & e.Whom
                      If e.Whom = nick Or arrPnick = nick Or voicePnick = nick Then
                          ' Si el nick expulsado coincide con tu nick, significa que te han kickeado
                          RichTextBox1.SelectionColor = Color.DarkViolet
                          MessageBox.Show($"Has sido expulsado del canal {e.Channel} por {e.Who}", $"Expulsion de {e.Channel}", MessageBoxButtons.OK, MessageBoxIcon.Information)
                          Dim canalAnterior As String = currentChannel

                          ' Salir del canal actual
                          If currentChannel = e.Channel Then
                              ' ircClients.Last().RfcPart(currentChannel)
                              usersin = 0
                              currentChannel = ""
                              ttopic = ""
                              ' Limpiar los elementos relacionados con el canal actual
                              ListBox1.Items.Clear()
                              RichTextBox1.Clear()
                          End If

                          ' Verificar si el canal actual no es "Abrir Sala"
                          If currentChannel <> "Abrir Canal" Then
                              ' Eliminar el canal del menú desplegable si está presente
                              Dim itemToRemove As ToolStripItem = ToolStripDropDownButton1.DropDownItems.Cast(Of ToolStripItem).FirstOrDefault(Function(item) item.Text = e.Channel)
                              If itemToRemove IsNot Nothing Then
                                  ToolStripDropDownButton1.DropDownItems.Remove(itemToRemove)
                              End If
                          End If

                          ' Limpiar el TextBox2
                          TextBox2.Clear()
                      Else

                          Dim currentTopIndex As Integer = ListBox1.TopIndex
                          If currentChannel = e.Channel AndAlso ListBox1.Items.Contains(e.Whom) Or ListBox1.Items.Contains(arrPnick) Or ListBox1.Items.Contains(voicePnick) Then


                              Dim previousColor1 As Color = RichTextBox1.SelectionColor
                              RichTextBox1.SelectionColor = previousColor1

                              Dim startIndex As Integer = RichTextBox1.TextLength
                              msg = $"{hora}     <== {e.Whom} ha sido expulsado del canal por {e.Who}" & vbCrLf

                              RichTextBox1.SuspendLayout()
                              RichTextBox1.AppendText(msg)
                              RichTextBox1.Select(startIndex, msg.Length)
                              RichTextBox1.SelectionColor = Color.Crimson
                              RichTextBox1.ResumeLayout()


                              Dim nombresAEliminar() As String = {e.Whom, arrPnick, voicePnick}

                              For Each nombreAEliminar As String In nombresAEliminar
                                  ' Buscar el índice del elemento con el nombre especificado
                                  Dim indice As Integer = ListBox1.FindStringExact(nombreAEliminar)

                                  While indice <> -1 ' Continuar eliminando elementos hasta que no se encuentre más coincidencias
                                      ListBox1.Items.RemoveAt(indice) ' Eliminar el elemento en el índice encontrado
                                      indice = ListBox1.FindStringExact(nombreAEliminar) ' Buscar el siguiente índice del elemento con el mismo nombre
                                  End While
                              Next
                              usersin = ListBox1.Items.Count


                              If Not channelText.ContainsKey(currentChannel) Then
                                  ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                                  channelText.Add(currentChannel, RichTextBox1.Rtf)
                              Else
                                  ' Si el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                                  channelText(currentChannel) = RichTextBox1.Rtf
                              End If
                          End If
                          ListBox1.TopIndex = currentTopIndex
                      End If
                  End Sub)
    End Sub


    Private Sub RichTextBox1_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles RichTextBox1.LinkClicked
        ' Abre la URL en el navegador predeterminado
        Process.Start(e.LinkText)
    End Sub
    Private Sub OnBan(sender As Object, e As BanEventArgs)
        Me.Invoke(Sub()
                      '  If currentChannel = e.Channel Then
                      Dim previousColor2 As Color = RichTextBox1.SelectionColor
                      Dim msg As String
                      Dim horaActual As DateTime = DateTime.Now
                      Dim hora As String = horaActual.ToString("HH") ' Asegura que siempre tenga 2 dígitos
                      Dim minutos As String = horaActual.ToString("mm") ' Asegura que siempre tenga 2 dígitos



                      If e.Hostmask.Contains(nick) Then
                          MsgBox(e.Who + " te ha baneado de " + e.Channel)
                      Else

                          If currentChannel = e.Channel Then
                              RichTextBox1.SelectionColor = previousColor2

                              Dim startIndex As Integer = RichTextBox1.TextLength

                              If e.Who <> "" Then
                                  Dim bannick As String = "por " + e.Who
                                  msg = $"{hora}:{minutos}     <== El usuario con máscara: {e.Hostmask} ha sido baneado del canal {bannick}" & vbCrLf
                              Else
                                  msg = $"{hora}:{minutos}     <== El usuario con máscara: {e.Hostmask} ha sido baneado del canal." & vbCrLf
                              End If
                              RichTextBox1.SuspendLayout()
                              RichTextBox1.AppendText(msg)

                              RichTextBox1.Select(startIndex, msg.Length)
                              RichTextBox1.SelectionColor = Color.Crimson
                              RichTextBox1.ResumeLayout()
                              ' Si la máscara de ban coincide con tu máscara de host, significa que has sido baneado
                              If Not channelText.ContainsKey(currentChannel) Then
                                  ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                                  channelText.Add(currentChannel, RichTextBox1.Rtf)
                              Else
                                  ' Si el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                                  channelText(currentChannel) = RichTextBox1.Rtf
                              End If
                          End If
                      End If
                      '   msg = $"{hora}:{minutos}     <==El usuario con máscara: {e.Hostmask} ha sido baneado del canal." & vbCrLf

                  End Sub)
    End Sub
    Private Sub OnQuit(sender As Object, e As QuitEventArgs)
        Me.Invoke(Sub()
                      Try

                          Dim horaActual As DateTime = DateTime.Now
                          Dim hora As String = horaActual.ToString("HH:mm") ' Asegura que siempre tenga 4 dígitos
                          Dim msg As String = ""
                          Dim qnick As String = e.Who
                          Dim arrPnick As String = "@" & qnick ' o "-" & pnick según tu requerimiento
                          Dim voicePnick As String = "+" & qnick

                          If privateChatForms.ContainsKey(qnick) OrElse privateChatForms.ContainsKey(arrPnick) OrElse privateChatForms.ContainsKey(voicePnick) Then
                              privateChatForms(qnick).Close()
                              privateChatForms.Remove(qnick)
                          End If

                          ' Verificar si el usuario que ha cerrado el IRC estaba en el canal actual
                          If currentChannel IsNot Nothing AndAlso ListBox1.Items.Contains(qnick) Or ListBox1.Items.Contains(arrPnick) Or ListBox1.Items.Contains(voicePnick) Then

                              If Not CheckBox2.Checked Then
                                  Dim startIndex As Integer = RichTextBox1.TextLength
                                  msg = $"{hora}     <== El usuario {qnick} ha cerrado el IRC." & vbCrLf

                                  RichTextBox1.SuspendLayout()
                                  RichTextBox1.AppendText(msg)
                                  RichTextBox1.Select(startIndex, msg.Length)
                                  RichTextBox1.SelectionColor = Color.DarkRed
                                  RichTextBox1.ResumeLayout()
                              End If
                              Dim nombresAEliminar() As String = {qnick, arrPnick, voicePnick}

                              For Each nombreAEliminar As String In nombresAEliminar
                                  ' Buscar el índice del elemento con el nombre especificado
                                  Dim indice As Integer = ListBox1.FindStringExact(nombreAEliminar)

                                  While indice <> -1 ' Continuar eliminando elementos hasta que no se encuentre más coincidencias
                                      ListBox1.Items.RemoveAt(indice) ' Eliminar el elemento en el índice encontrado
                                      indice = ListBox1.FindStringExact(nombreAEliminar) ' Buscar el siguiente índice del elemento con el mismo nombre
                                  End While
                              Next
                              usersin = ListBox1.Items.Count
                              If Not channelText.ContainsKey(currentChannel) Then
                                  channelText.Add(currentChannel, RichTextBox1.Rtf)
                              Else
                                  channelText(currentChannel) = RichTextBox1.Rtf
                              End If

                          End If

                      Catch ex As Exception
                          ' Manejar cualquier excepción que pueda ocurrir durante el procesamiento del evento
                          ' Console.WriteLine("Error al procesar el evento OnQuit: " & ex.Message)
                      End Try
                  End Sub)
    End Sub



    Private Sub OnNick(sender As Object, e As NickChangeEventArgs)
        Me.Invoke(Sub()
                      Try
                          Dim currentTopIndex As Integer = ListBox1.TopIndex
                          Dim oldNick As String = e.OldNickname
                          Dim newNick As String = e.NewNickname

                          Dim horaActual As DateTime = DateTime.Now
                          Dim hora As String = horaActual.ToString("HH:mm") ' Asegura que siempre tenga 4 dígitos
                          Dim msg As String = ""

                          ' 
                          '  End If
                          Dim arrPnick As String = "@" & oldNick ' o "-" & pnick según tu requerimiento
                          Dim voicePnick As String = "+" & oldNick
                          Dim arrPnick1 As String = "@" & newNick ' o "-" & pnick según tu requerimiento
                          Dim voicePnick1 As String = "+" & newNick
                          Dim startIndex As Integer = RichTextBox1.TextLength
                          If currentChannel IsNot Nothing AndAlso ListBox1.Items.Contains(oldNick) Or ListBox1.Items.Contains(arrPnick) Or ListBox1.Items.Contains(voicePnick) Or ListBox1.Items.Contains(newNick) Or ListBox1.Items.Contains(arrPnick1) Or ListBox1.Items.Contains(voicePnick1) Then
                              If oldNick = nick Or arrPnick = nick Or voicePnick = nick Or newNick = nick Or arrPnick1 = nick Or voicePnick1 = nick Then
                                  msg = $"{hora}     <=> Has cambiado el nick de {oldNick} a {newNick}." & vbCrLf
                                  nick = newNick
                                  nickreg = newNick
                              Else
                                  msg = $"{hora}     <=> El usuario {oldNick} ha cambiado su apodo a {newNick}." & vbCrLf
                                  If nick = oldNick Then
                                      nickreg = newNick
                                  End If
                              End If

                                  RichTextBox1.SuspendLayout()
                              RichTextBox1.AppendText(msg)
                              RichTextBox1.Select(startIndex, msg.Length)
                              RichTextBox1.SelectionColor = Color.Purple
                              RichTextBox1.ResumeLayout()
                          End If
                          ' MsgBox("onnick")


                          ' ListBox1.Items.Clear()
                          'ircClients.Last().RfcNames(currentChannel)

                          Dim nombresAnteriores() As String = {oldNick, arrPnick, voicePnick} ' Variantes del nombre anterior

                          For Each nombreAnterior As String In nombresAnteriores
                              ' Buscar el índice del elemento con el nombre anterior
                              Dim indice As Integer = ListBox1.FindStringExact(nombreAnterior)

                              If indice <> -1 Then ' Verifica si se encontró el elemento
                                  Dim nuevoNombre As String = If(nombreAnterior = oldNick, newNick, nombreAnterior(0) & newNick) ' Determina el nuevo nombre
                                  ListBox1.Items(indice) = nuevoNombre ' Cambia el nombre del elemento
                              End If
                          Next

                          If Not channelText.ContainsKey(currentChannel) Then
                              ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                              channelText.Add(currentChannel, RichTextBox1.Rtf)
                          Else
                              ' Si el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                              channelText(currentChannel) = RichTextBox1.Rtf
                          End If

                          ListBox1.TopIndex = currentTopIndex
                      Catch ex As Exception
                          ' Manejar cualquier excepción que pueda ocurrir durante el procesamiento del evento
                          ' Console.WriteLine("Error al procesar el evento OnNick: " & ex.Message)
                      End Try
                  End Sub)
    End Sub

    Private Sub OnJoin(sender As Object, e As JoinEventArgs)
        Me.Invoke(Sub()
                      Dim horaActual As DateTime = DateTime.Now
                      Dim hora As String = horaActual.ToString("HH:mm") ' Asegura que siempre tenga 4 dígitos
                      Dim msg As String = ""
                      Dim nnick As String = e.Who

                      If e.Channel = currentChannel AndAlso nnick <> nick Then ' Verificar si el evento pertenece al canal actual y el usuario no es el propio cliente


                          Dim prefix As String = ""

                          If e.Data.MessageArray.Length > 1 AndAlso e.Data.MessageArray(1).ToLower() = "mode" Then
                              Dim modes As String = e.Data.MessageArray(3)

                              If modes.Contains("@") AndAlso nnick = e.Data.MessageArray(4) Then
                                  ' Si el usuario es operador en el canal
                                  prefix = "@"
                              ElseIf modes.Contains("+") AndAlso nnick = e.Data.MessageArray(4) Then
                                  ' Si el usuario tiene voz en el canal
                                  prefix = "+"
                              End If
                          End If

                          ' Agregar el prefijo al nick del usuario
                          nnick = prefix & nnick

                          ' Agregar el nick al ListBox si no está presente
                          If Not ListBox1.Items.Contains(nnick) Then
                              ListBox1.BeginUpdate()
                              ListBox1.Items.Add(nnick)
                              ListBox1.EndUpdate()
                              usersin = ListBox1.Items.Count
                              If Not CheckBox1.Checked Then
                                  msg = $"{hora}     ==> El usuario {nnick} ha entrado." & vbCrLf
                                  Dim startIndex As Integer = RichTextBox1.TextLength

                                  RichTextBox1.SuspendLayout()
                                  RichTextBox1.AppendText(msg)
                                  RichTextBox1.Select(startIndex, msg.Length)
                                  RichTextBox1.SelectionColor = Color.DarkGreen
                                  RichTextBox1.ResumeLayout()
                              End If
                              usersin = ListBox1.Items.Count
                              If Not channelText.ContainsKey(currentChannel) Then
                                  ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                                  channelText.Add(currentChannel, RichTextBox1.Rtf)
                              Else
                                  ' Si el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                                  channelText(currentChannel) = RichTextBox1.Rtf
                              End If
                          End If
                      End If
                      'End If
                      '   End If
                  End Sub)
    End Sub


    Private Sub OnPart(sender As Object, e As PartEventArgs)
        ' Manejar el evento cuando un usuario sale de la sala
        Me.Invoke(Sub()
                      Dim previousColor7 As Color = RichTextBox1.SelectionColor
                      Dim currentTopIndex As Integer = ListBox1.TopIndex
                      Dim horaActual As DateTime = DateTime.Now
                      Dim hora As String = horaActual.ToString("HH") ' Asegura que siempre tenga 2 dígitos
                      Dim minutos As String = horaActual.ToString("mm") ' Asegura que siempre tenga 2 dígitos
                      Dim msg As String

                      If e.Channel = currentChannel Then ' Verificar si el evento pertenece al canal actual

                          Dim pnick As String = e.Who

                          ' Quitar el nick del ListBox si está presente

                          If Not CheckBox2.Checked Then
                              RichTextBox1.SelectionColor = previousColor7
                              Dim startIndex As Integer = RichTextBox1.TextLength


                              msg = $"{hora}:{minutos}     <==El usuario {pnick} ha salido del canal." & vbCrLf
                              RichTextBox1.SuspendLayout()
                              RichTextBox1.AppendText(msg)

                              RichTextBox1.Select(startIndex, msg.Length)
                              RichTextBox1.SelectionColor = Color.DarkRed
                              RichTextBox1.ResumeLayout()

                          End If


                          Dim qnick As String = e.Who
                          Dim arrPnick As String = "@" & qnick ' o "-" & pnick según tu requerimiento
                          Dim voicePnick As String = "+" & qnick

                          Dim nombresAEliminar() As String = {qnick, arrPnick, voicePnick}

                          For Each nombreAEliminar As String In nombresAEliminar
                              ' Buscar el índice del elemento con el nombre especificado
                              Dim indice As Integer = ListBox1.FindStringExact(nombreAEliminar)

                              While indice <> -1 ' Continuar eliminando elementos hasta que no se encuentre más coincidencias
                                  ListBox1.Items.RemoveAt(indice) ' Eliminar el elemento en el índice encontrado
                                  indice = ListBox1.FindStringExact(nombreAEliminar) ' Buscar el siguiente índice del elemento con el mismo nombre
                              End While
                          Next


                          usersin = ListBox1.Items.Count

                          If Not channelText.ContainsKey(currentChannel) Then
                              ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                              channelText.Add(currentChannel, RichTextBox1.Rtf)
                          Else
                              ' Si el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                              channelText(currentChannel) = RichTextBox1.Rtf
                          End If


                      End If
                      ' End If

                      ' Cerrar el formulario privado asociado al nick si está abierto
                      'End If
                      'Dim pnick As String = e.Who
                      ' msg = $"{hora}:{minutos}     <==El usuario {e.Who} ha salido del canal." & vbCrLf

                      ListBox1.TopIndex = currentTopIndex
                  End Sub)
    End Sub
    Private Sub OnConnected(sender As Object, e As EventArgs)
        Me.Invoke(Sub()
                      Try
                            Button1.Text = "DESCONECTAR"
                          canal = InputBox("Ingrese un canal donde entrar:", "Ingresar Canal", "#cosmos")


                          If String.IsNullOrWhiteSpace(canal) Then
                              ' MessageBox.Show("No ha insertado un canal, vaya al Menu-Abrir Sala para abrir una sala", "Operación Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Information)
                              Exit Sub ' Cerrar la aplicación si se cancela la operación
                          End If
                          If Not canal.StartsWith("#") Then
                                  canal = "#" + canal ' Salir si el canal no empieza con #
                              End If

                              Dim irc = DirectCast(sender, IrcClient)
                              irc.RfcJoin(canal)



                              If Not ban AndAlso Not prohivido Then
                                  canal = canal.ToLower
                                  ToolStripDropDownButton1.DropDownItems.Add(canal)
                              currentChannel = canal
                              newnick = nick
                                  conected = True
                              End If

                          'End If
                      Catch es As Exception
                          ' MessageBox.Show("Se produjo un error en la conexión...", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error)
                      End Try
                  End Sub)
    End Sub
    Private Sub OnChannelMessage(sender As Object, e As IrcEventArgs)
        Try
            'Dim ircClient = DirectCast(sender, IrcClient)
            Dim channel = e.Data.Channel
            Dim horaActual As DateTime = DateTime.Now
            Dim hora As String = horaActual.ToString("HH") ' Asegura que siempre tenga 2 dígitos
            Dim minutos As String = horaActual.ToString("mm") ' Asegura que siempre tenga 2 dígitos


            Dim message As String = $"{hora}:{minutos}     <{e.Data.Nick}> {e.Data.Message}" & vbCrLf


            ' Ejecutar en el hilo principal para evitar InvalidOperationException
            Me.Invoke(Sub()
                          ' Almacenar el mensaje en el diccionario de mensajes del canal


                          ' Si el canal actual es el mismo que el del mensaje entrante, mostrarlo en RichTextBox
                          If channel = currentChannel Then
                              Dim startIndex As Integer = RichTextBox1.TextLength
                              RichTextBox1.SuspendLayout()

                              DecodeAndDisplayColors(message)
                              ' Colorear el nombre del usuario (e.data.nick) con un color diferente
                              RichTextBox1.Select(startIndex, e.Data.Nick.Length + 12) ' Sumamos 2 para incluir los símbolos < y >
                              RichTextBox1.SelectionColor = Color.DarkBlue
                              RichTextBox1.ResumeLayout()
                          End If
                          If Not channelText.ContainsKey(currentChannel) Then
                              ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                              channelText.Add(currentChannel, RichTextBox1.Rtf)
                          Else
                              ' Si el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                              channelText(currentChannel) = RichTextBox1.Rtf
                          End If
                      End Sub)
        Catch ex As Exception
            'Restart()

        End Try
    End Sub



    Private Sub OnNames(sender As Object, e As NamesEventArgs)
        ' Limpiar la lista de usuarios antes de agregar los nuevos

        ' Invocar la actualización de la interfaz de usuario para agregar los nuevos elementos al ListBox
        Me.Invoke(Sub()

                      ListBox1.BeginUpdate()


                      For Each user As String In e.RawUserList

                          ListBox1.Items.Add(user)
                          '  usersin += 1
                      Next
                      EliminarItemsDuplicados(ListBox1)
                      usersin = ListBox1.Items.Count
                      ListBox1.EndUpdate()

                  End Sub)
    End Sub



    Private Sub AbrirSALAToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles AbrirSALAToolStripMenuItem1.Click
        OpenChannel()

    End Sub

    Public Sub DecodeAndDisplayColors(input As String)
        Me.Invoke(Sub()
                      Dim regex As New Regex("\u0003(\d{1,2})(,(\d{1,2}))?|\^B(.*?)\^B")
                      Dim matches As MatchCollection = regex.Matches(input)

                      Dim currentPosition As Integer = 0
                      For Each match As Match In matches
                          Dim textLength As Integer = match.Index - currentPosition
                          If textLength > 0 Then
                              RichTextBox1.AppendText(input.Substring(currentPosition, textLength))
                          End If

                          If match.Groups(1).Success Then
                              ' Handle color codes
                              Dim colorCode As Integer = Integer.Parse(match.Groups(1).Value)
                              Dim backgroundColorCode As Integer = If(match.Groups(3).Success, Integer.Parse(match.Groups(3).Value), -1)

                              ' Apply color
                              If colorCode >= 0 AndAlso colorCode <= 15 Then
                                  RichTextBox1.SelectionColor = GetIRCColor(colorCode)
                              End If

                              ' Apply background color if provided
                              If backgroundColorCode >= 0 AndAlso backgroundColorCode <= 15 Then
                                  RichTextBox1.SelectionBackColor = GetIRCColor(backgroundColorCode)
                              End If
                          ElseIf match.Groups(4).Success Then
                              ' Handle bold text
                              Dim boldText As String = match.Groups(4).Value
                              Dim startIndex As Integer = RichTextBox1.TextLength
                              RichTextBox1.AppendText(boldText)
                              Dim length As Integer = RichTextBox1.TextLength - startIndex
                              RichTextBox1.Select(startIndex, length)
                              RichTextBox1.SelectionFont = New Font(RichTextBox1.Font, FontStyle.Bold)
                              RichTextBox1.SelectionColor = RichTextBox1.ForeColor ' reset color
                          End If

                          currentPosition = match.Index + match.Length
                      Next

                      ' Append the remaining text
                      If currentPosition < input.Length Then
                          RichTextBox1.AppendText(input.Substring(currentPosition))
                      End If
                  End Sub)
    End Sub



    Public Function GetIRCColor(colorCode As Integer) As Color
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
    Private Sub OnTopic(sender As Object, e As TopicEventArgs)
        ' Manejar el evento cuando se recibe el topic del canal
        Dim topic As String = e.Topic

        ' Almacenar el topic asociado al canal actual en el diccionario
        channelTopics(currentChannel) = topic

        ' Actualizar el topic solo si corresponde al canal
        If currentChannel = e.Channel Then
            ' If Not String.IsNullOrEmpty(topic) Then
            ActualizarTopic(topic)
            '  End If
        End If

        'End If
    End Sub

    Private Sub ActualizarTopic(topic As String)
        Me.Invoke(Sub()


                      addtopic = "Topic: " + topic & vbCrLf

                      If String.IsNullOrEmpty(topic) Then
                          'addtopic = "<No topic>"
                          ttopic = " - No topic"
                          'DecodeAndDisplayColors(addtopic)
                          addtopic = ""
                          RichTextBox1.Text = addtopic
                      Else
                          '  If Not channelText.ContainsKey(currentChannel) Then
                          '   channelText.Add(currentChannel, New StringBuilder())
                          ' End If
                          'RichTextBox1.Text = addtopic
                          DecodeAndDisplayColors(addtopic)
                          'channelText(currentChannel).Append(addtopic)
                          ttopic = " - " + topic

                      End If


                      If Not channelText.ContainsKey(currentChannel) Then
                          ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                          channelText.Add(currentChannel, RichTextBox1.Rtf)
                      Else
                          ' Si 'el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                          channelText(currentChannel) = RichTextBox1.Rtf
                      End If
                  End Sub)
    End Sub

    Private Sub ReceiveMessages()
        Try
            ' Dim errorOccurred As Boolean = False
            fall = True

            While fall
                ' Do While Not errorOccurred

                ' Loop para recibir mensajes continuamente
                ircClients.Last().Listen()
                ' Application.DoEvents()
            End While
            ' Desplazar automáticamente el RichTextBox hacia abajo
      

        Catch ex As Exception
            'errorOccurred = True
            ' MessageBox.Show("Error de general: " & ex.Message, "Error de la aplicacion", MessageBoxButtons.OK, MessageBoxIcon.Error)

            ' Manejar la excepción adecuadamente, por ejemplo, cerrar la conexión y notificar al usuario
            'Restart()
        End Try
        ' Loop


    End Sub
    Private Sub Irc_OnQueryMessage(sender As Object, e As IrcEventArgs)
        Me.Invoke(Sub()
                      Try
                          Dim qnick As String = e.Data.Nick
                          Dim message As String = e.Data.Message
                          Dim horaActual As DateTime = DateTime.Now
                          Dim hora As String = horaActual.ToString("HH") ' Asegura que siempre tenga 2 dígitos
                          Dim minutos As String = horaActual.ToString("mm")

                          Dim msg As String = $"{hora}:{minutos}"

                          ' Verificar si el nick tiene un prefijo "@" o "+"
                          Dim hasPrefix As Boolean = qnick.StartsWith("@") OrElse qnick.StartsWith("+")
                          Dim cleanNick As String = If(hasPrefix, qnick.Substring(1), qnick)

                          ' Verificar si ya existe una ventana privada asociada al nick del remitente
                          If privateChatForms.ContainsKey(cleanNick) AndAlso Not privateChatForms(cleanNick).IsDisposed Then
                              ' Si ya existe y no está cerrada, agregar el mensaje al formulario existente
                              privateChatForms(cleanNick).AddMessage($"{msg}     <{cleanNick}> {message}" & vbCrLf)
                          Else
                              If Not ignoreList.Contains(qnick) Then
                                  ' Si no existe o está cerrada, crear un nuevo formulario privado y agregarlo al diccionario
                                  Dim newPrivateChatForm As New privado(cleanNick, ircClients.Last(), Me, RichTextBox1.ZoomFactor)
                                  AddHandler newPrivateChatForm.FormClosed, Sub(s, ea) privateChatForms.Remove(cleanNick)
                                  newPrivateChatForm.Show()
                                  newPrivateChatForm.AddMessage($"{msg}     <{qnick}> {message}" & vbCrLf)
                                  privateChatForms(cleanNick) = newPrivateChatForm
                                  My.Computer.Audio.Play(My.Resources.bip, AudioPlayMode.WaitToComplete)
                              End If
                          End If
                      Catch ex As Exception
                          '  MsgBox("on query error")
                      End Try
                  End Sub)
    End Sub



    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox2.KeyPress
        If Button1.Text = "DESCONECTAR" Then
            'MsgBox("yes")

            If e.KeyChar = Convert.ToChar(Keys.Return) Then
                Try
                    SendMessage()
                    e.Handled = True
                Catch ex As Exception
                End Try
            End If


        Else
            e.Handled = True
        End If
        If currentChannel = Nothing Then
            e.Handled = True
        End If

    End Sub

    Private Sub SendMessage()
        ' Obtener el mensaje del TextBox2
        Dim message As String = TextBox2.Text

        ' Verificar que el mensaje no esté vacío
        If Not String.IsNullOrEmpty(message) Then
            ' Dividir el mensaje en líneas
            Dim lines() As String = message.Split({Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)

            ' Enviar cada línea del mensaje al canal actual
            For Each line As String In lines
                ' Eliminar espacios en blanco adicionales al principio y al final de la línea
                Dim trimmedLine As String = line.Trim()

                ' Verificar si la línea está vacía después de quitar los espacios en blanco adicionales
                If Not String.IsNullOrEmpty(trimmedLine) Then
                    ' Construir el mensaje completo con la hora actual y el nombre de usuario
                    Dim startIndex As Integer = RichTextBox1.TextLength
                    Dim horaActual As DateTime = DateTime.Now
                    Dim hora As String = horaActual.ToString("HH") ' Asegura que siempre tenga 2 dígitos
                    Dim minutos As String = horaActual.ToString("mm")
                    Dim msg As String = $"{hora}:{minutos}     <{nick}> {trimmedLine}" & vbCrLf

                    ' Mostrar el mensaje en el RichTextBox
                    RichTextBox1.SuspendLayout()
                    DecodeAndDisplayColors(msg)
                    RichTextBox1.Select(startIndex, nick.Length + 12) ' Sumamos 2 para incluir los símbolos < y >
                    RichTextBox1.SelectionColor = Color.MediumBlue
                    RichTextBox1.ResumeLayout()

                    ' Enviar el mensaje al canal actual
                    ircClients.Last().SendMessage(SendType.Message, currentChannel, trimmedLine)
                End If
            Next

            ' Agregar el mensaje al diccionario de mensajes del canal
            If Not channelText.ContainsKey(currentChannel) Then
                ' Si el canal no está en el diccionario, añade una nueva entrada con el contenido RTF del RichTextBox
                channelText.Add(currentChannel, RichTextBox1.Rtf)
            Else
                ' Si el canal ya está en el diccionario, actualiza el contenido RTF del canal con el contenido actual del RichTextBox
                channelText(currentChannel) = RichTextBox1.Rtf
            End If

            ' Limpiar el TextBox2 después de enviar el mensaje
            TextBox2.Clear()
        End If
    End Sub






    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        ' Manejar el doble clic en un elemento del ListBox
        If ListBox1.SelectedItem IsNot Nothing Then
            Dim selectedNick As String = ListBox1.SelectedItem.ToString()

            Dim hasPrefix As Boolean = selectedNick.StartsWith("@") OrElse selectedNick.StartsWith("+")
            Dim cleanNick As String = If(hasPrefix, selectedNick.Substring(1), selectedNick)
            ' Verificar si ya existe un formulario privado para este nick
            If privateChatForms.ContainsKey(cleanNick) Then
                Dim existingForm As privado = privateChatForms(cleanNick)
                ' Si ya existe, verificar si está cerrado
                If existingForm.IsDisposed Then
                    ' Si está cerrado, crear un nuevo formulario privado y agregarlo al diccionario
                    existingForm = New privado(cleanNick, ircClients.Last(), Me, RichTextBox1.ZoomFactor)
                    privateChatForms(cleanNick) = existingForm
                End If
                ' Mostrar el formulario privado existente
                existingForm.Show()
                existingForm.BringToFront()
            Else
                ' Si no existe, abrir un nuevo formulario privado y agregarlo al diccionario
                Dim privateChatForm As New privado(cleanNick, ircClients.Last(), Me, RichTextBox1.ZoomFactor)
                privateChatForm.Show()
                privateChatForm.BringToFront()
                privateChatForms.Add(cleanNick, privateChatForm)
            End If
        End If
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        End
    End Sub


    Function EliminarCaracteres(ByVal input As String) As String
        ' Expresión regular para encontrar los caracteres específicos
        Dim regex As New Regex("\u0003(\d{1,2})(,(\d{1,2}))?")

        ' Eliminar los caracteres específicos usando la expresión regular
        Dim resultado As String = regex.Replace(input, "")

        Return resultado
    End Function
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'If Button1.Text = "CONECTADO" Then
        Dim fechaHoraActual As DateTime = DateTime.Now
        Dim fechaHora As String = fechaHoraActual.ToString("dd/MM/yyyy HH:mm:ss")
        Me.Text = winname + "Nick: " + nick + " / Canal: " + currentChannel + " [" + usersin.ToString + "] - " + fechaHora + EliminarCaracteres(ttopic)

    End Sub

    Public Sub AddPrivateChatForm(nick As String, privateChatForm As privado)
        privateChatForms.Add(nick, privateChatForm)
    End Sub

    Public Sub RemovePrivateChatForm(nick As String)
        If privateChatForms.ContainsKey(nick) Then
            privateChatForms(nick).Dispose() ' Cerrar la ventana privada
            privateChatForms.Remove(nick)
        End If
    End Sub


    Private Sub OnRawMessage(sender As Object, e As IrcEventArgs)
        ' Aquí puedes utilizar la máscara del nick según sea necesario
        Me.Invoke(Sub()
                      Try




                          Dim data As String = e.Data.Message ' Aquí puedes procesar la respuesta del servidor.

                          ' Verificar si el mensaje contiene el nick y no contiene "Hispano"

                          'MsgBox(data)
                          'If data.Contains("ACTION") Then
                          If data.Contains("(you're banned)") Then
                              ban = True
                              MessageBox.Show(data.ToString, "Error al entrar al canal; estas baneado.", MessageBoxButtons.OK, MessageBoxIcon.Information)
                              If currentChannel <> "Abrir Canal" Then
                                  ' Eliminar el canal del menú desplegable si está presente
                                  Dim itemToRemove As ToolStripItem = Nothing
                                  For Each item As ToolStripItem In ToolStripDropDownButton1.DropDownItems
                                      If item.Text = currentChannel Then
                                          itemToRemove = item
                                          Exit For
                                      End If
                                  Next

                                  If itemToRemove IsNot Nothing AndAlso Not TypeOf itemToRemove Is ToolStripSeparator Then
                                      ToolStripDropDownButton1.DropDownItems.Remove(itemToRemove)
                                  End If
                              End If
                              currentChannel = ""
                              ttopic = ""
                              usersin = 0
                              ban = False
                              Exit Sub
                          End If
                          If data.Contains("está registrado") Then

                              regnick = True
                                  MessageBox.Show(data.ToString, "Nick registrado", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                  If Not conected Then
                                      CleanupAndResetUI()
                                  Else
                                      Dim rnd As New Random()
                                      Dim numero As Integer = rnd.Next(18, 9001)
                                      newnick = "SmartUser" + numero.ToString
                                      ircClients.Last().RfcNick($"{newnick}")
                                      nick = newnick
                                      Exit Sub
                                  End If

                          End If
                          If data.Contains("Contraseña incorrecta para el nick " + newnick) Then

                              regnick = True
                                  MessageBox.Show(data.ToString, "Contraseña incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                  If Not conected Then
                                      CleanupAndResetUI()
                                  Else
                                      Dim rnd As New Random()
                                      Dim numero As Integer = rnd.Next(18, 9001)
                                      newnick = "SmartUser" + numero.ToString
                                      ircClients.Last().RfcNick($"{newnick}")
                                      nick = newnick
                                      Exit Sub
                                  End If
                              End If

                          If data.Contains("Can't change nickname") Then

                          
                              MessageBox.Show(data.ToString, "Error al cambiar el nick", MessageBoxButtons.OK, MessageBoxIcon.Information)
                              ' Application.DoEvents()
                              Exit Sub
                          End If
                          If data.Contains("El nick " + newnick + " está prohibido") Then
                              prohivido = True
                              MessageBox.Show(data.ToString, "Nick prohivido", MessageBoxButtons.OK, MessageBoxIcon.Information)
                              If Not conected Then
                                  CleanupAndResetUI()
                              Else
                                  Dim rnd As New Random()
                                  Dim numero As Integer = rnd.Next(18, 9001)
                                  newnick = "SmartUser" + numero.ToString
                                  ircClients.Last().RfcNick($"{newnick}")
                                  nick = newnick
                                  Exit Sub
                              End If

                          End If




                          'Application.DoEvents()
                      Catch ex As Exception
                          '  Restart()
                      End Try
                  End Sub)
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Me.Invoke(Sub()
        'Try
        ' Verificar si el botón está en "CONECTAR"
        Dim rnd As New Random()
        Dim numero As Integer = rnd.Next(18, 9001)
        If Button1.Text = "CONECTAR" Then
            nick = InputBox("Ingrese su nick:", "Ingresar Nick", "SmartUser" + numero.ToString)

            ' Verificar si el usuario canceló la operación
            If String.IsNullOrWhiteSpace(nick) Then
                MessageBox.Show("Debe insertar un nick.", "Operación Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            pass = InputBox("Ingrese la contraseña para " & nick & " (opcional)", "Ingresar Contraseña", "")



            ' Restablecer la instancia de IrcClient
            irc = New IrcClient()



            ' Crear una nueva instancia de IrcClient y conectar al servidor IRC
            'Dim irc As New IrcClient()
            AddHandler irc.OnRawMessage, AddressOf OnRawMessage
                AddHandler irc.OnNickChange, AddressOf OnNick
                AddHandler irc.OnConnected, AddressOf OnConnected
                AddHandler irc.OnChannelMessage, AddressOf OnChannelMessage
                AddHandler irc.OnQueryMessage, AddressOf Irc_OnQueryMessage
                AddHandler irc.OnNames, AddressOf OnNames
                AddHandler irc.OnOp, AddressOf OnOp
                AddHandler irc.OnDeop, AddressOf OnDeop
                AddHandler irc.OnVoice, AddressOf OnDeop
                AddHandler irc.OnDevoice, AddressOf OnDevoice
                AddHandler irc.OnJoin, AddressOf OnJoin
                AddHandler irc.OnPart, AddressOf OnPart
                AddHandler irc.OnQuit, AddressOf OnQuit
                AddHandler irc.OnKick, AddressOf OnKick
                AddHandler irc.OnBan, AddressOf OnBan
                AddHandler irc.OnTopic, AddressOf OnTopic
                AddHandler irc.OnChannelAction, AddressOf MeAction
            AddHandler irc.OnDisconnected, AddressOf ResetUI



            ircClients.Add(irc)
                ircClients.Last().Encoding = System.Text.Encoding.UTF8

                ' Button1.Text = "..."

                ircClients.Last().Connect(ComboBox1.Text, TextBox3.Text)


                ' Autenticarse con el servidor
                ircClients.Last().Login(nick, nick, 0, nick, pass)



            ComboBox1.Enabled = False
                TextBox3.Enabled = False
                fall = True

            Application.DoEvents()
            ' Iniciar un hilo para recibir mensajes
            Dim receiveThread As New Thread(AddressOf ReceiveMessages)
                receiveThread.Start()


            TextBox2.Focus()

            ' Verificar si hay una conexión establecida
        ElseIf Button1.Text = "DESCONECTAR" Then
            ' Verificar si hay una conexión establecida

            ' Verificar si el cliente IRC está conectado antes de desconectarlo


            CleanupAndResetUI()

        End If

        ' End Sub)
    End Sub
    Private Sub CleanupAndResetUI()
        Me.Invoke(Sub()

                      If ircClients.Any() Then
                          For Each client As IrcClient In ircClients
                              If client IsNot Nothing AndAlso client.IsConnected Then
                                  ' Desconectar el cliente IRC
                                  Task.Run(Sub()
                                               client.Disconnect()
                                               Application.DoEvents()
                                           End Sub)
                              End If
                          Next
                      End If
                      ' Limpiar los recursos y restablecer la interfaz de usuario
                      ircClients.Last().RfcQuit()
                      irc = Nothing
                      RichTextBox1.Clear()
                      ListBox1.Items.Clear()
                      ComboBox1.Enabled = True
                      TextBox3.Enabled = True
                      Button1.Text = "CONECTAR"
                      If Not regnick Or Not prohivido Then
                          currentChannel = ""
                      End If
                      ttopic = ""
                      nick = ""
                      pass = ""
                      canal = ""
                      maxmin = 0
                      usersin = 0
                      conected = False
                      prohivido = False
                      ban = False
                      regnick = False
                      'nochange = False
                      ListBox1.Font = New Font(ListBox1.Font.FontFamily, 13)
                      RichTextBox1.ZoomFactor = 1.0F
                      'zoomFactor = 1.0F

                      ' Cerrar los formularios de chat privado
                      For Each formularioPrivado As privado In privateChatForms.Values
                          formularioPrivado.Close()
                      Next

                      ' Limpiar los elementos del menú de canales
                      For i As Integer = ToolStripDropDownButton1.DropDownItems.Count - 1 To 0 Step -1
                          Dim item As ToolStripItem = ToolStripDropDownButton1.DropDownItems(i)
                          If Not (TypeOf item Is ToolStripSeparator OrElse item.Text = "Abrir Canal") Then
                              ToolStripDropDownButton1.DropDownItems.Remove(item)
                          End If
                      Next


                  End Sub)


    End Sub
    Private Sub ResetUI()
        Me.Invoke(Sub()


                      irc = Nothing
                      RichTextBox1.Clear()
                      ListBox1.Items.Clear()
                      ComboBox1.Enabled = True
                      TextBox3.Enabled = True
                      Button1.Text = "CONECTAR"
                      If Not regnick Or Not prohivido Then
                          currentChannel = ""
                      End If
                      ttopic = ""
                      nick = ""
                      pass = ""
                      canal = ""
                      maxmin = 0
                      usersin = 0
                      conected = False
                      prohivido = False
                      ban = False
                      regnick = False
                      'nochange = False
                      ListBox1.Font = New Font(ListBox1.Font.FontFamily, 13)
                      RichTextBox1.ZoomFactor = 1.0F
                      'zoomFactor = 1.0F

                      ' Cerrar los formularios de chat privado
                      For Each formularioPrivado As privado In privateChatForms.Values
                          formularioPrivado.Close()
                      Next

                      ' Limpiar los elementos del menú de canales
                      For i As Integer = ToolStripDropDownButton1.DropDownItems.Count - 1 To 0 Step -1
                          Dim item As ToolStripItem = ToolStripDropDownButton1.DropDownItems(i)
                          If Not (TypeOf item Is ToolStripSeparator OrElse item.Text = "Abrir Canal") Then
                              ToolStripDropDownButton1.DropDownItems.Remove(item)
                          End If
                      Next


                  End Sub)


    End Sub
    Private Sub MeAction(sender As Object, e As IrcEventArgs)

        ' Aquí puedes manejar el evento OnChannelAction.
        ' Por ejemplo, puedes acceder al canal y al usuario que realizó la acción, así como al mensaje de la acción.
        Me.Invoke(Sub()
                      Dim channel As String = e.Data.Channel
                      If channel = currentChannel Then
                          Dim user As String = e.Data.Nick
                          Dim action As String = e.Data.Message
                          Dim startIndex As Integer = RichTextBox1.TextLength
                          Dim nuevaFrase As String = action.Substring(action.IndexOf(" ") + 1)
                          Dim message = user + " dice: " + nuevaFrase + vbCrLf
                          RichTextBox1.SuspendLayout()

                          DecodeAndDisplayColors(message)

                          RichTextBox1.Select(startIndex, user.Length + 6) ' Sumamos 2 para incluir los símbolos < y >
                          RichTextBox1.SelectionColor = Color.DarkBlue
                          RichTextBox1.ResumeLayout()
                      End If
                  End Sub)
        ' Luego puedes realizar las acciones necesarias en respuesta a la acción del usuario en el canal.
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.Text = ComboBox1.Items(0)
        Dim thecolor As String = My.Settings.Color
        ' MsgBox(thecolor)
        If thecolor = "orange" Then
            PictureBox3_Click(sender, e)
        ElseIf thecolor = "gold" Then
            PictureBox4_Click(sender, e)
        ElseIf thecolor = "turquesa" Then
            PictureBox2_Click(sender, e)
        ElseIf thecolor = "dark" Then
            PictureBox1_Click(sender, e)
        End If
    End Sub



    Private Sub CambiarNickToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CambiarNickToolStripMenuItem.Click
        Me.Invoke(Sub()
                      If Button1.Text = "DESCONECTAR" Then
                          newnick = InputBox("Ingrese su nuevo nick:", "Ingresar Nick", "")
                          Dim password As String = InputBox("Ingrese la contraseña para " + newnick + " (opcional):", "Ingresar Contraseña", "")
                          password = ":" + password
                          If newnick <> "" Then





                              ' Cambiar el nick del cliente IRC
                              ircClients.Last().RfcNick($"{newnick}{password}")
                              ' Actualizar la lista de usuarios después de cambiar el nick
                              'ircClients.Last().RfcNames(currentChannel)
                              Application.DoEvents()

                              ' Agregar el nuevo nick a la primera posición del ListBox
                              If regnick = True Then Exit Sub
                              If prohivido = True Then Exit Sub
                              ' Eliminar el nick anterior si existe
                              '    ListBox1.BeginUpdate()

                              ' If ListBox1.Items.Contains(nick) Then
                              ' ListBox1.Items.Remove(nick)
                              'End If
                              '  ListBox1.EndUpdate()
                              'TextBox2.Clear()

                              '                    MessageBox.Show($"Ha cambiado su nick de {nick} a {newnick}", "Nick cambiado", MessageBoxButtons.OK, MessageBoxIcon.Information)

                              nick = newnick
                              'MessageBox.Show("Su nuevo nick ahora es: " + newnick, "Cambio de nick", MessageBoxButtons.OK, MessageBoxIcon.Information)
                          End If
                          ' reg = False
                      End If
                      ' reg = False
                      'End If
                      ' reg = False


                  End Sub)
    End Sub 'aApXYBz3Ht4E

    Private Sub CerrarCanalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CerrarCanalToolStripMenuItem.Click
        Try
            If Button1.Text = "DESCONECTAR" Then
                ' Almacenar el canal actual antes de cerrarlo
                Dim canalAnterior As String = currentChannel

                ' Salir del canal actual
                ircClients.Last().RfcPart(currentChannel)

                ' Limpiar los elementos relacionados con el canal actual
                ListBox1.Items.Clear()
                RichTextBox1.Clear()
                usersin = 0
                ' Verificar si el canal actual no es "Abrir Sala"
                If currentChannel <> "Abrir Canal" Then

                    ' Eliminar el canal del menú desplegable si está presente
                    Dim itemToRemove As ToolStripItem = Nothing
                    For Each item As ToolStripItem In ToolStripDropDownButton1.DropDownItems
                        If item.Text = currentChannel Then
                            itemToRemove = item
                            Exit For
                        End If
                    Next

                    ' Verificar si el elemento a eliminar no es la barra separadora
                    If itemToRemove IsNot Nothing AndAlso Not TypeOf itemToRemove Is ToolStripSeparator Then
                        ToolStripDropDownButton1.DropDownItems.Remove(itemToRemove)
                    End If
                End If

                ' Limpiar el TextBox2
                TextBox2.Clear()
                currentChannel = ""
                ttopic = ""
                ' Preguntar al usuario si desea volver a entrar en el canal

            End If
        Catch ex As Exception
            ' Manejar cualquier excepción que pueda ocurrir durante el proceso
            MessageBox.Show("Error al cerrar el canal: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Try
            Me.BackColor = Color.LightSeaGreen
            RichTextBox1.BackColor = Color.LightSeaGreen
            ListBox1.BackColor = Color.DarkCyan
            TextBox2.BackColor = Color.DarkTurquoise
            ListBox1.ForeColor = Color.White
            turquesa = True
            orange = False
            dark = False
            gold = False

            For Each formularioPrivado As privado In privateChatForms.Values
                ' Cambiar el color de fondo del formulario privado
                formularioPrivado.BackColor = Color.DarkGray

                ' Cambiar el color de fondo del RichTextBox del formulario privado
                formularioPrivado.RichTextBox1.BackColor = Color.LightSeaGreen

                ' Cambiar el color de fondo del ListBox del formulario privado
                'formularioPrivado.ListBox1.BackColor = Color.DimGray

                ' Cambiar el color de fondo del TextBox del formulario privado
                formularioPrivado.txtSendMessage.BackColor = Color.DarkTurquoise

                ' Actualizar las variables de estado

            Next
        Catch ex As Exception
            MessageBox.Show("Ocurrió un error al cambiar el color de fondo.")
        End Try
    End Sub


    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        Try
            Me.BackColor = Color.Coral
            RichTextBox1.BackColor = Color.Coral
            ListBox1.BackColor = Color.Tomato
            TextBox2.BackColor = Color.SandyBrown
            ListBox1.ForeColor = Color.White
            turquesa = False
            orange = True
            dark = False
            gold = False
            For Each formularioPrivado As privado In privateChatForms.Values
                ' Cambiar el color de fondo del formulario privado
                formularioPrivado.BackColor = Color.DarkGray

                ' Cambiar el color de fondo del RichTextBox del formulario privado
                formularioPrivado.RichTextBox1.BackColor = Color.Coral

                ' Cambiar el color de fondo del ListBox del formulario privado
                'formularioPrivado.ListBox1.BackColor = Color.DimGray

                ' Cambiar el color de fondo del TextBox del formulario privado
                formularioPrivado.txtSendMessage.BackColor = Color.SandyBrown

                ' Actualizar las variables de estado

            Next
        Catch ex As Exception
            MessageBox.Show("Ocurrió un error al cambiar el color de fondo.")
        End Try
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Try
            Me.BackColor = Color.DarkGray
            RichTextBox1.BackColor = Color.DarkGray
            ListBox1.BackColor = Color.DimGray
            TextBox2.BackColor = Color.WhiteSmoke
            ListBox1.ForeColor = Color.White
            turquesa = False
            orange = False
            dark = True
            gold = False

            For Each formularioPrivado As privado In privateChatForms.Values
                ' Cambiar el color de fondo del formulario privado
                formularioPrivado.BackColor = Color.DarkGray

                ' Cambiar el color de fondo del RichTextBox del formulario privado
                formularioPrivado.RichTextBox1.BackColor = Color.DarkGray

                ' Cambiar el color de fondo del TextBox del formulario privado
                formularioPrivado.txtSendMessage.BackColor = Color.WhiteSmoke

                ' Actualizar las variables de estado

            Next
        Catch ex As Exception
            MessageBox.Show("Ocurrió un error al cambiar el color de fondo.")
        End Try
    End Sub
    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click
        Try
            Me.BackColor = Color.Goldenrod
            RichTextBox1.BackColor = Color.Goldenrod
            ListBox1.BackColor = Color.DarkGoldenrod
            TextBox2.BackColor = Color.Khaki
            ListBox1.ForeColor = Color.White
            turquesa = False
            orange = False
            dark = False
            gold = True


            For Each formularioPrivado As privado In privateChatForms.Values
                ' Cambiar el color de fondo del formulario privado
                formularioPrivado.BackColor = Color.Goldenrod

                ' Cambiar el color de fondo del RichTextBox del formulario privado
                formularioPrivado.RichTextBox1.BackColor = Color.Goldenrod

                ' Cambiar el color de fondo del TextBox del formulario privado
                formularioPrivado.txtSendMessage.BackColor = Color.Khaki

                ' Actualizar las variables de estado

            Next
        Catch ex As Exception
            MessageBox.Show("Ocurrió un error al cambiar el color de fondo.")
        End Try
    End Sub
    Private Sub ToolStripDropDownButton1_DropDownItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ToolStripDropDownButton1.DropDownItemClicked
        Dim clickedItem As ToolStripItem = e.ClickedItem
        If TypeOf clickedItem Is ToolStripMenuItem Then
            Dim menuItem As ToolStripMenuItem = DirectCast(clickedItem, ToolStripMenuItem)
            Dim newChannel As String = menuItem.Text
            If newChannel <> "Abrir Canal" Then

                If newChannel = currentChannel Then

                    Exit Sub

                Else
                    RecargarSala(newChannel)


                End If
            End If
        End If
    End Sub



    Private Sub RecargarSala(channelName As String)
        If currentChannel <> channelName Then ' Verificar si el canal actual es diferente al nuevo canal
            ListBox1.Items.Clear()
            usersin = 0
            Dim oldTotalZoom As Single = totalzoom ' Almacena el valor actual de totalzoom
            currentChannel = channelName

            ircClients.Last().RfcJoin(currentChannel)
            ircClients.Last().RfcNames(currentChannel)

            Dim newTopic As String = If(channelTopics.ContainsKey(currentChannel), channelTopics(currentChannel), "")
            If String.IsNullOrEmpty(newTopic) Then
                ttopic = " - No topic"
            Else
                ttopic = " - " + newTopic
            End If

            If channelText.ContainsKey(currentChannel) Then
                RichTextBox1.Rtf = channelText(currentChannel)
                ' ListBox1.Font = New Font(ListBox1.Font.FontFamily, 13.2)
            End If

            ' Ajustar el zoom factor aquí
            If maxmin > 0 Then
                For i As Integer = 1 To maxmin
                    RichTextBox1.ZoomFactor -= zoom
                Next
                totalzoom = oldTotalZoom + zoom * maxmin
            ElseIf maxmin < 0 Then
                For i As Integer = 1 To Math.Abs(maxmin)
                    RichTextBox1.ZoomFactor += zoom
                Next
                totalzoom = oldTotalZoom - zoom * Math.Abs(maxmin)
            End If
        End If
    End Sub

    Private Sub OpenChannel()
        Try
            If Button1.Text = "DESCONECTAR" Then
                Dim newChannel As String = InputBox("Ingrese el nombre del nuevo canal:", "Agregar Canal", "#")
                If String.IsNullOrWhiteSpace(newChannel) Then
                    Exit Sub ' Salir si se cancela la operación o se ingresa un canal inválido
                End If
                If Not newChannel.StartsWith("#") Then
                    newChannel = "#" + newChannel ' Agregar el símbolo # al canal si no lo tiene
                End If

                ' Convertir el nuevo canal a minúsculas
                newChannel = newChannel.ToLower()

                ' Verificar si el canal ya está en el menú desplegable
                Dim canalExistente As Boolean = False
                For Each item As ToolStripItem In ToolStripDropDownButton1.DropDownItems
                    If TypeOf item Is ToolStripMenuItem AndAlso DirectCast(item, ToolStripMenuItem).Text.ToLower() = newChannel Then
                        canalExistente = True
                        Exit For
                    End If
                Next

                ' Si el canal no está en el menú desplegable, agregarlo
                If Not canalExistente AndAlso Not ban Then
                    usersin = 0
                    ' Limpiar el ListBox antes de cargar los usuarios del nuevo canal
                    ListBox1.Items.Clear()
                    RichTextBox1.Clear()

                    ircClients.Last().RfcJoin(newChannel)

                    currentChannel = newChannel
                    ActualizarTopic("")
                    If maxmin > 0 Then
                        For i As Integer = 1 To maxmin
                            RichTextBox1.ZoomFactor -= zoom
                        Next
                        totalzoom = totalzoom + zoom * maxmin
                    ElseIf maxmin < 0 Then
                        For i As Integer = 1 To Math.Abs(maxmin)
                            RichTextBox1.ZoomFactor += zoom
                        Next
                        totalzoom = totalzoom - zoom * Math.Abs(maxmin)
                    End If
                    ' ListBox1.Font = New Font(ListBox1.Font.FontFamily, 13.2)
                    'maxmin = 0


                    Dim nuevoCanalMenuItem As New ToolStripMenuItem(newChannel)
                    ToolStripDropDownButton1.DropDownItems.Add(nuevoCanalMenuItem)
                Else
                    RecargarSala(newChannel)

                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error al abrir el canal." + ex.ToString, "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub




    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            If Button1.Text = "DESCONECTAR" Then
                Dim anchoOriginal As Integer = ListBox1.HorizontalExtent
                Dim incremento As Integer = 1.4
                If maxmin > -14 Then
                    ListBox1.Font = New Font(ListBox1.Font.FontFamily, ListBox1.Font.Size + incremento)
                    ListBox1.HorizontalExtent = anchoOriginal

                    RichTextBox1.ZoomFactor += zoom

                    totalzoom = totalzoom + zoom
                    maxmin -= 1
                    For Each formularioPrivado As privado In privateChatForms.Values
                        formularioPrivado.SincronizarTamaño()
                    Next
                End If
            End If
        Catch ex As Exception
            ' Manejar cualquier excepción que ocurra al aumentar el tamaño del text
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            If Button1.Text = "DESCONECTAR" Then
                Dim anchoOriginal As Integer = ListBox1.HorizontalExtent
                Dim decremento As Integer = 1
                If maxmin < 8 Then
                    ListBox1.Font = New Font(ListBox1.Font.FontFamily, ListBox1.Font.Size - decremento)
                    ListBox1.HorizontalExtent = anchoOriginal
                    RichTextBox1.ZoomFactor -= zoom

                    totalzoom = totalzoom - zoom
                    ' MsgBox(totalzoom)
                    maxmin += 1


                    For Each formularioPrivado As privado In privateChatForms.Values
                        formularioPrivado.SincronizarTamaño()
                    Next
                End If
            End If
        Catch ex As Exception
            ' Manejar cualquier excepción que ocurra al disminuir el tamaño del texto
        End Try
    End Sub
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If orange Then
            My.Settings.Color = "orange"
        ElseIf gold Then
            My.Settings.Color = "gold"
        ElseIf turquesa Then
            My.Settings.Color = "turquesa"
        ElseIf dark Then
            My.Settings.Color = "dark"
        End If
        My.Settings.Save()
        '  MsgBox(My.Settings.Color.ToString)
    End Sub
    Private Sub RichTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles RichTextBox1.KeyDown
        Try
            ' Si se presiona Ctrl + C en el RichTextBox, copia el texto seleccionado.
            If e.Control AndAlso e.KeyCode = Keys.C Then
                ' MsgBox(RichTextBox1.SelectedText)
                ' Verificar si hay texto seleccionado en el RichTextBox

                ' Copiar el texto seleccionado al portapapeles
                Clipboard.SetText(RichTextBox1.SelectedText)

            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub RichTextBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles RichTextBox1.MouseUp

        If RichTextBox1.SelectionLength = 0 Then
            ' Si hay texto seleccionado, enfocar nuevamente el TextBox2
            TextBox2.Focus()
        End If

    End Sub

End Class
