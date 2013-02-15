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

            Dim checkIfExists = usernamesDataAdapter.ScalarQueryCheck(username)

            If checkIfExists Is Nothing Then
                'the user doesn't exist
                MsgBox("The user with the entered username does not exist")
            Else
                'the user does exist, decrease his invalidAttempts.
                'If invalid attempts are equal to zero or less, then freeze the account
                Dim checkIfFrozen = usernamesDataAdapter.ScalarQueryCheckIsFrozen(username, True)

                If checkIfFrozen Is Nothing Then
                    'The account is not frozen, decrease the number of invalidAttempts, freeze if neccessary
                    usernamesDataAdapter.UpdateQueryDecreaseInvalidAttemptsByOne(username)

                    'check if invalidAttempts = 0
                    Dim checkIfZero = usernamesDataAdapter.ScalarQueryCheckInvalidAttempts(username, 0)
                    If checkIfZero Is Nothing Then
                        'the invalidAttempts field is not equal to zero, do nothing
                    Else
                        'the invalidAttemots field is equal to zero, proceed to Freeze this account
                        usernamesDataAdapter.UpdateQuerySetToFrozen(username)


                    End If

                Else
                    'The account is Frozen, display the requred prompt
                    MsgBox("Your username has been frozen to prevent your account from being hacked. Please contact your Administrator.")

                End If


            End If

            MessageBox.Show("Wrong password. Note that your account will get frozen if you enter a wrong password three times.")

        Else
            'Logged in successfully, such user exists
            'Now let's check whether the user is an admin. If he's an admin, he can't be frozen
            'A non-admin user that is frozen shouldn't be loggedin

            Dim checkIfAdmin = usernamesDataAdapter.ScalarQueryCheckIfAdmin(username, True)
            If checkIfAdmin Is Nothing Then
                'Not an admin. Let's check if the user is frozen first

                Dim checkIfFrozen = usernamesDataAdapter.ScalarQueryCheckIsFrozen(username, True)
                If checkIfFrozen Is Nothing Then
                    'not frozen. Now let's check if the user is an Author
                    Dim checkIfAuthor = usernamesDataAdapter.ScalarQueryCheckIfAuthor(username, True)
                    If checkIfAuthor Is Nothing Then
                        'not an Author, log his in as a standard user
                        MsgBox("Welcome, " & username & ". You have successfully logged in as a User, now taking you to your Dashboard.")
                        Me.Hide()
                        userDashboard.Show()


                    Else
                        'Author, log him into the Author dashboard
                        MsgBox("Welcome, " & username & ". You have successfully logged in as an Author, now taking you to your Dashboard.")
                        Me.Hide()
                        authorDashboard.Show()

                    End If



                Else
                    'Frozen user
                    MsgBox("It looks like your username has been frozen due to security reasons. Please contact an Administrator.")

                End If


            Else
                'Admin, log him into the admin dashboard
                MsgBox("Welcome, " & username & ". You have successfully logged in as an Administrator, now taking you to your Dashboard.")
                Me.Hide()
                adminDashboard.Show()
            End If


            MsgBox("Hi, login successful!")
            usernamesDataAdapter.UpdateQueryRefreshInvalidAttempts(username)



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
