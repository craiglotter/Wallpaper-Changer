Imports System.IO


Public Class Main_Screen

    Dim progresslabel As String = ""
    Dim attachmentpath As String = ""
    Dim filename As String = ""

    Dim attachmentsendfolder As String = ""
    Dim attachmentsentfolder As String = ""
    Dim attachmentdeletefolder As String = ""

    Dim shownminimizetip As Boolean = False

    Dim DateTimePicker1_Save As String = ""
    Dim DateTimePicker2_Save As String = ""

    Dim busyworking As Boolean = False

    '**************************
    Dim mainObjectName As String = "Wallpaper"
    '**************************

    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message()
                If FullErrors_Checkbox.Checked = True Then
                    Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ": " & ex.ToString
                Else
                    Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ": " & ex.Message.ToString
                End If
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
                Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                dir = Nothing
                Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ": " & ex.ToString)
                filewriter.WriteLine("")
                filewriter.Flush()
                filewriter.Close()
                filewriter = Nothing
                Label2.Text = "Error encountered in last action"
            End If
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error." & vbCrLf & vbCrLf & exc.ToString, MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub RunWorker()
        Try
            If busyworking = False Then
                busyworking = True
                Me.ControlBox = False

                Label2.Text = "Preparing to download Staff and Student list"
                progresslabel = ""
                ProgressBar1.Enabled = True
                ProgressBar1.Value = 0
                DateTimePicker1.Enabled = False
                DateTimePicker2.Enabled = False
                NumericUpDown1.Enabled = False
                CheckBox1.Enabled = False
                Label11.Enabled = False

                comboBox1.Enabled = False
                sanbase.Enabled = False
                Button1.Enabled = False
                filename = ""
                attachmentpath = ""

                BackgroundWorker1.RunWorkerAsync()
            End If
        Catch ex As Exception
            Error_Handler(ex, "Run Worker")
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            NotifyIcon1.BalloonTipText = "You have chosen to hide " & My.Application.Info.ProductName & ". To bring it back up, simply click here."
            NotifyIcon1.BalloonTipTitle = "" & My.Application.Info.ProductName & ""
            NotifyIcon1.Text = "Click to bring up " & My.Application.Info.ProductName & ""
            shownminimizetip = False
            Control.CheckForIllegalCrossThreadCalls = False
            DateTimePicker1.Value = Now
            DateTimePicker2.Value = Now
            Me.Text = My.Application.Info.ProductName & " (" & Format(My.Application.Info.Version.Major, "0000") & Format(My.Application.Info.Version.Minor, "00") & Format(My.Application.Info.Version.Build, "00") & "." & Format(My.Application.Info.Version.Revision, "00") & ")"
            If My.Computer.FileSystem.DirectoryExists((Application.StartupPath & "\Temp").Replace("\\", "\")) = False Then
                My.Computer.FileSystem.CreateDirectory((Application.StartupPath & "\Temp").Replace("\\", "\"))
            End If
            loadSettings()
            Label2.Text = "Application loaded"
            Label7.Select()
        Catch ex As Exception
            Error_Handler(ex, "Form Load")
        End Try
    End Sub


    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            progresslabel = "Locating Wallpapers"
            BackgroundWorker1.ReportProgress(0)

            If sanbase.Text.Length > 0 And My.Computer.FileSystem.DirectoryExists(sanbase.Text) = True Then
                Dim firstfile As String = ""
                Dim currentfile As String = ""
                Dim selectedfile As String = ""
                Dim tinfo As DirectoryInfo = New DirectoryInfo((Application.StartupPath & "\Temp").Replace("\\", "\"))
                Dim dinfo As DirectoryInfo = New DirectoryInfo(sanbase.Text)
                For Each finfo As FileInfo In tinfo.GetFiles
                    If finfo.Extension.ToLower = ".jpg" Or finfo.Extension.ToLower = ".bmp" Then
                        currentfile = finfo.Name.ToLower
                    End If
                    finfo = Nothing
                Next
                ' MsgBox("currentfile: " & currentfile)
                Dim recordname As Boolean = False
                Dim notthisiteration As Boolean = False
                For Each finfo As FileInfo In dinfo.GetFiles
                    notthisiteration = False
                    If finfo.Extension.ToLower = ".jpg" Or finfo.Extension.ToLower = ".bmp" Then
                        If firstfile = "" Then
                            firstfile = finfo.Name.ToLower
                            ' MsgBox("firstfile: " & firstfile)
                        End If
                        If finfo.Name.ToLower = currentfile.ToLower And recordname = False Then
                            recordname = True
                            notthisiteration = True
                        End If
                        If recordname = True And notthisiteration = False Then
                            selectedfile = finfo.Name.ToLower
                            recordname = False
                        End If
                    End If
                    finfo = Nothing
                Next
                progresslabel = "New Wallpaper Selected"
                BackgroundWorker1.ReportProgress(50)
                'MsgBox("selectedfile: " & selectedfile)
                If selectedfile = "" Or selectedfile = currentfile Then
                    selectedfile = firstfile
                End If
                'MsgBox("selectedfile: " & selectedfile)
                tinfo = Nothing
                dinfo = Nothing
                If Not selectedfile = "" Then
                    If Not currentfile = "" Then
                        My.Computer.FileSystem.DeleteFile((Application.StartupPath & "\Temp\" & currentfile).Replace("\\", "\"))
                    End If
                    My.Computer.FileSystem.CopyFile((sanbase.Text & "\" & selectedfile).Replace("\\", "\"), (Application.StartupPath & "\Temp\" & selectedfile).Replace("\\", "\"))
                    If My.Computer.FileSystem.FileExists((Application.StartupPath & "\SetWallpaper.exe").Replace("\\", "\")) = True Then




                        If comboBox1.SelectedIndex = -1 Then
                            comboBox1.SelectedIndex = 0
                        End If


                        Dim info As System.Diagnostics.ProcessStartInfo = New System.Diagnostics.ProcessStartInfo
                        info.Arguments = """" & (Application.StartupPath & "\Temp\" & selectedfile).Replace("\\", "\") & """" & " " & """" & comboBox1.Items.Item(comboBox1.SelectedIndex).ToString & """"

                        info.CreateNoWindow = True
                        info.ErrorDialog = True
                        info.FileName = (Application.StartupPath & "\SetWallpaper.exe").Replace("\\", "\")
                        Process.Start(info)
                        info = Nothing
                        progresslabel = "Wallpaper successfully Changed"
                        BackgroundWorker1.ReportProgress(100)
                    End If
                Else
                    e.Cancel = True
                    progresslabel = "No " & mainObjectName & "s were located"
                    BackgroundWorker1.ReportProgress(100)
                End If
            Else
                e.Cancel = True
                progresslabel = mainObjectName & " Base Folder not specified"
                BackgroundWorker1.ReportProgress(100)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Background Worker: " & progresslabel)
            progresslabel = "Operation Failed: Error reported (" & progresslabel & ")"
        End Try
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Try
            ProgressBar1.Value = e.ProgressPercentage
            Label2.Text = progresslabel
        Catch ex As Exception
            Error_Handler(ex, "Worker Progress Changed")
        End Try
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Try
            ProgressBar1.Value = 100
            ProgressBar1.Enabled = False

            CheckBox1.Enabled = True
            If CheckBox1.Checked = True Then
                NumericUpDown1.Enabled = True
                DateTimePicker1.Enabled = False
                DateTimePicker2.Enabled = False
            Else
                NumericUpDown1.Enabled = False
                DateTimePicker1.Enabled = True
                DateTimePicker2.Enabled = True
            End If

           
            comboBox1.Enabled = True
            sanbase.Enabled = True
            Button1.Enabled = True
            Label11.Enabled = True

            If e.Cancelled = True Then
                Label2.Text = "Failed to update " & mainObjectName & ": (" & progresslabel & ")"
            Else
                Label2.Text = "Operation Successfully Completed"
            End If

        Catch ex As Exception
            Error_Handler(ex, "Run Worker Completed")
        End Try
        Me.ControlBox = True
        busyworking = False
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Try
            Label2.Text = "About displayed"
            AboutBox1.ShowDialog()
        Catch ex As Exception
            Error_Handler(ex, "Display About Screen")
        End Try
    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripMenuItem.Click
        Try
            Label2.Text = "Help displayed"
            HelpBox1.ShowDialog()
        Catch ex As Exception
            Error_Handler(ex, "Display Help Screen")
        End Try
    End Sub



    Private Sub loadSettings()
        Try
            Label2.Text = "Loading application settings..."




            Dim configfile As String = (Application.StartupPath & "\config.sav").Replace("\\", "\")
            If My.Computer.FileSystem.FileExists(configfile) Then
                Dim reader As StreamReader = New StreamReader(configfile)
                Dim lineread As String
                Dim variablevalue As String
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    If lineread.IndexOf("=") <> -1 Then

                        variablevalue = lineread.Remove(0, lineread.IndexOf("=") + 1)

                        If lineread.StartsWith("sanbase=") Then
                            If My.Computer.FileSystem.DirectoryExists(variablevalue) = True Then
                                sanbase.Text = variablevalue
                                FolderBrowserDialog1.SelectedPath = variablevalue
                            End If
                        End If
                        If lineread.StartsWith("combobox1=") Then
                            comboBox1.SelectedIndex = variablevalue
                        End If
 
                        If lineread.StartsWith("DateTimePicker1=") Then
                            DateTimePicker1.Value = Date.Parse(variablevalue)
                            SaveSettings_Memory()
                        End If
                        If lineread.StartsWith("DateTimePicker2=") Then
                            DateTimePicker2.Value = Date.Parse(variablevalue)
                            SaveSettings_Memory()
                        End If
                        If lineread.StartsWith("NumericUpDown1=") Then
                            NumericUpDown1.Value = Integer.Parse(variablevalue)
                        End If
                        If lineread.StartsWith("CheckBox1=") Then
                            CheckBox1.Checked = Boolean.Parse(variablevalue)
                        End If
                        If lineread.StartsWith("FullErrors_Checkbox=") Then
                            FullErrors_Checkbox.Checked = Boolean.Parse(variablevalue)
                        End If

                    End If
                End While
                reader.Close()
                reader = Nothing
            End If


            'default values
            If comboBox1.SelectedIndex = -1 Then
                comboBox1.SelectedIndex = 0
            End If




            Label2.Text = "Application Settings successfully loaded"
        Catch ex As Exception
            Error_Handler(ex, "Load Settings")
        End Try
    End Sub


    Private Sub SaveSettings()
        Try
            Label2.Text = "Saving application settings..."
            Dim configfile As String = (Application.StartupPath & "\config.sav").Replace("\\", "\")
            Dim writer As StreamWriter = New StreamWriter(configfile, False)

            If sanbase.Text.Length > 0 Then
                writer.WriteLine("sanbase=" & sanbase.Text)
            End If
           
            If comboBox1.SelectedIndex <> -1 Then
                writer.WriteLine("combobox1=" & comboBox1.SelectedIndex)
            End If

            LoadSettings_Memory()

            writer.WriteLine("DateTimePicker1=" & DateTimePicker1.Value.ToString)
            writer.WriteLine("DateTimePicker2=" & DateTimePicker2.Value.ToString)

            writer.WriteLine("NumericUpDown1=" & NumericUpDown1.Value.ToString)
            writer.WriteLine("CheckBox1=" & CheckBox1.Checked.ToString)
            writer.WriteLine("FullErrors_Checkbox=" & FullErrors_Checkbox.Checked.ToString)
            

            writer.Flush()
            writer.Close()
            writer = Nothing

            Label2.Text = "Application Settings successfully saved"

        Catch ex As Exception
            Error_Handler(ex, "Save Settings")
        End Try
    End Sub


    Private Sub Main_Screen_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            If CheckBox1.Checked = True Then
                LoadSettings_Memory()
            End If
            SaveSettings()
        Catch ex As Exception
            Error_Handler(ex, "Closing Application")
        End Try
    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            Label7.Text = Format(Now, "HH:mm:ss")
            If Label7.Text = Label8.Text Or Label7.Text = Label1.Text Then
                If Label7.Text = Label1.Text And CheckBox1.Checked = True Then
                    'ignore because we're running on the scheduled timer
                Else
                    RunWorker()
                    If CheckBox1.Checked = True Then
                        DateTimePicker1.Value = DateTimePicker1.Value.AddMinutes(NumericUpDown1.Value)
                    End If
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Timer Ticking")
        End Try
    End Sub


    Private Sub DateTimePicker1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker1.ValueChanged
        Try
            Label8.Text = Format(DateTimePicker1.Value, "HH:mm:ss")
        Catch ex As Exception
            Error_Handler(ex, "Change Scheduled Time")
        End Try
    End Sub

    Private Sub DateTimePicker2_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DateTimePicker2.ValueChanged
        Try
            Label1.Text = Format(DateTimePicker2.Value, "HH:mm:ss")
        Catch ex As Exception
            Error_Handler(ex, "Change Scheduled Time")
        End Try
    End Sub


    Private Sub NotifyIcon1_BalloonTipClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotifyIcon1.BalloonTipClicked
        Try
            Me.WindowState = FormWindowState.Normal
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
            Me.Refresh()
        Catch ex As Exception
            Error_Handler(ex, "Click on NotifyIcon")
        End Try
    End Sub


    Private Sub NotifyIcon1_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseClick
        Try
            Me.WindowState = FormWindowState.Normal
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
            Me.Refresh()
        Catch ex As Exception
            Error_Handler(ex, "Click on NotifyIcon")
        End Try
    End Sub


    Private Sub NotifyIcon1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NotifyIcon1.Click
        Try
            Me.WindowState = FormWindowState.Normal
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
            Me.Refresh()
        Catch ex As Exception
            Error_Handler(ex, "Click on NotifyIcon")
        End Try
    End Sub

    Private Sub Main_Screen_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Try
            If Me.WindowState = FormWindowState.Minimized Then
                Me.ShowInTaskbar = False
                NotifyIcon1.Visible = True
                If shownminimizetip = False Then
                    NotifyIcon1.ShowBalloonTip(1)
                    shownminimizetip = True
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Change Window State")
        End Try
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        Try
            If CheckBox1.Checked = True Then
                SaveSettings_Memory()
                NumericUpDown1.Enabled = True
                DateTimePicker1.Enabled = False
                DateTimePicker2.Enabled = False
                DateTimePicker2.Value = Now.AddSeconds(-2)
                DateTimePicker1.Value = DateTimePicker2.Value
                DateTimePicker1.Value = DateTimePicker1.Value.AddMinutes(NumericUpDown1.Value)
            Else

                DateTimePicker1.Enabled = True

                DateTimePicker2.Enabled = True

                LoadSettings_Memory()

                NumericUpDown1.Enabled = False
            End If

        Catch ex As Exception
            Error_Handler(ex, "Enable Interval based timer")
        End Try
    End Sub

    Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        Try
            If CheckBox1.Checked = True Then
                ' DateTimePicker1.Value = DateTimePicker1.Value.AddMinutes(NumericUpDown1.Value)
                DateTimePicker1.Value = Now().AddMinutes(NumericUpDown1.Value)
                'SaveSettings_Memory()
            End If

        Catch ex As Exception
            Error_Handler(ex, "Increase interval")
        End Try
    End Sub

    Private Sub SaveSettings_Memory()
        Try
            DateTimePicker1_Save = DateTimePicker1.Value.ToString
            DateTimePicker2_Save = DateTimePicker2.Value.ToString
           
        Catch ex As Exception
            Error_Handler(ex, "SaveSettings_Memory")
        End Try
    End Sub

    Private Sub LoadSettings_Memory()
        Try
            If DateTimePicker1_Save.Length > 0 Then
                DateTimePicker1.Value = Date.Parse(DateTimePicker1_Save)
            End If
            If DateTimePicker2_Save.Length > 0 Then
                DateTimePicker2.Value = Date.Parse(DateTimePicker2_Save)
            End If


        Catch ex As Exception
            Error_Handler(ex, "LoadSettings_Memory")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            If FolderBrowserDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                sanbase.Text = FolderBrowserDialog1.SelectedPath
            End If
        Catch ex As Exception
            Error_Handler(ex, "Select " & mainObjectName & " Folder")
        End Try
    End Sub

    Private Sub Label11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label11.Click
        RunWorker()
    End Sub


End Class

