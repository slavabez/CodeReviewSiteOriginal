Imports System.Threading
Imports System.Net
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports CodeReviewSite.CodeReviewSiteDataSetTableAdapters


Public Class dashboard

    Dim serverThread As Thread

    Dim myCon As New OleDb.OleDbConnection

    Dim dataSet As New CodeReviewSiteDataSet()

    Dim invalidAttempts As Integer = 3

    Dim usernamesDataAdapter As New CodeReviewSite.CodeReviewSiteDataSetTableAdapters.usernamesTableAdapter




    Private Sub dashboard_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'initialises and starts the Web Server
        serverThread = New Thread(AddressOf Webserver.Main)
        serverThread.Start()

    End Sub

    Private Sub LblLinkToPortfolios_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LblLinkToPortfolios.LinkClicked
        'Opens the address in the default browser
        Webserver.openAddressOfTheServer()

    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles usrnBox.TextChanged
        'Only if there's something in BOTH the username and the password field the login button will be displayed
        If usrnBox.Text.Length > 0 Then
            If pswdTxtBox.Text.Length > 0 Then
                btnLogin.Enabled = True
            Else
                btnLogin.Enabled = False
            End If
        Else
            btnLogin.Enabled = False
        End If
    End Sub

    Private Sub pswdTxtBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pswdTxtBox.TextChanged
        'Only if there's something in BOTH the username and the password field the login button will be displayed
        If usrnBox.Text.Length > 0 Then
            If pswdTxtBox.Text.Length > 0 Then
                btnLogin.Enabled = True
            Else
                btnLogin.Enabled = False
            End If
        Else
            btnLogin.Enabled = False
        End If
    End Sub

    Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        'invalid attempts. If > 3, display warning, close the application.
        If invalidAttempts <= 0 Then
            Application.Exit()

        End If
        Dim username As String = usrnBox.Text
        Dim password As String = pswdTxtBox.Text

        Dim login = usernamesDataAdapter.ScalarQueryLogin(username, password)

        If login Is Nothing Then

            invalidAttempts = invalidAttempts - 1

            Dim checkIfFrozen = usernamesDataAdapter.ScalarQueryCheck(username)

            If checkIfFrozen Is Nothing Then
                'the user doesn't exist
                MsgBox("The user entered does not exist")
            Else

            End If

            MessageBox.Show("Good attempt, but you're not a real user! You have " & invalidAttempts & " attempt(s) remaining...")

        Else
            'Logged in successfully, such user exists
            Dim checkIfFrozen = usernamesDataAdapter.ScalarQueryCheck(u


            MsgBox("Hi, login successful!")

        End If



    End Sub

    Private Sub OpenConnectionToDatabase()
        Try
            myCon.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=N:\.do_not_delete\desktop.xp\GitHub\CodeReviewSite\CodeReviewSite.accdb;Jet OLEDB:Database Password=Starcraft2;"
            myCon.Open()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub CloseConnectionToDatabase()
        Try
            myCon.Close()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub exitBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitBtn.Click

    End Sub
End Class
