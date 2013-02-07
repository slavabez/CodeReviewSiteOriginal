Imports System.Threading
Imports System.Net
Imports System.Data.OleDb
Imports System.Data.SqlClient

Public Class dashboard

    Dim serverThread As Thread

    Dim DBpath As String = ""
    Dim DBtable As String = "usernames"

    Dim myCon As New OleDb.OleDbConnection

    Dim MyDataAdapter As OleDb.OleDbDataAdapter

    Dim myDataSet As DataSet


    Private Sub dashboard_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'initialises and starts the Web Server
        serverThread = New Thread(AddressOf Webserver.Main)
        serverThread.Start()

    End Sub

    Private Sub LblLinkToPortfolios_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LblLinkToPortfolios.LinkClicked
        Dim address As String
        Dim hostName As String = Dns.GetHostName()
        'creating a valid address to go to the server's webpage
        address = "http://" + Dns.GetHostEntry(hostName).AddressList(5).ToString + ":8080"
        'opening that address using the default browser
        System.Diagnostics.Process.Start(address)

    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged
        'Only if there's something in BOTH the username and the password field the login button will be displayed
        If TextBox1.Text.Length > 0 Then
            If pswdTxtBox.Text.Length > 0 Then
                btnLogin.Enabled = True
            Else
                btnLogin.Enabled = False
            End If
        Else
            btnLogin.Enabled = False
        End If
    End Sub

    Private Sub pswdTxtBox_TextChanged(sender As System.Object, e As System.EventArgs) Handles pswdTxtBox.TextChanged
        'Only if there's something in BOTH the username and the password field the login button will be displayed
        If TextBox1.Text.Length > 0 Then
            If pswdTxtBox.Text.Length > 0 Then
                btnLogin.Enabled = True
            Else
                btnLogin.Enabled = False
            End If
        Else
            btnLogin.Enabled = False
        End If
    End Sub

    Private Sub btnLogin_Click(sender As System.Object, e As System.EventArgs) Handles btnLogin.Click
        'invalid attempts. If > 3, display warning, close the application.
        Dim invalidAttempts As Integer = 0

        Dim usrnm As String = TextBox1.Text
        Dim pswd As String = pswdTxtBox.Text


        Dim con As New OleDbConnection("")
        Dim cmd As OleDbCommand = New OleDbCommand("SELECT * FROM usernames WHERE username = '" & usrnm & "' AND password = '" & pswd & "' ", con)

        con.Open()
        Dim reader As OleDbDataReader = cmd.ExecuteReader()

        'attempting to read the record
        If reader.Read() = True Then
            MessageBox.Show("It appears that the user is valid")
            Dim sql As New SqlCommand("SELECT admin FROM usernames WHERE username = '" & usrnm & "' ")

            'con.Open()

            Dim sdr As SqlDataReader = sql.ExecuteReader

            While sdr.Read = True
                MsgBox(sdr.Item("") & " " & sdr.Item(""))
            End While

            


        Else
            MessageBox.Show("Good attempt, but you're not a real user!")
        End If



    End Sub

    Private Function OpenConnectionToDatabase()
        Try
            myCon.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=N:\.do_not_delete\desktop.xp\GitHub\CodeReviewSite\CodeReviewSite.accdb;Jet OLEDB:Database Password=Starcraft2;"
            'carry on from here!!!
        Catch ex As Exception

        End Try
    End Function

    Private Sub exitBtn_Click(sender As System.Object, e As System.EventArgs) Handles exitBtn.Click

    End Sub
End Class
