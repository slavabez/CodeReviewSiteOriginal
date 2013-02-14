'Visual Basic.Net JingCai Programming 100 Examples
'Author: Yong Zhang
'Publisher: Water Publisher China
'ISBN: 750841156
'found online at http://www.java2s.com/Tutorial/VB/0400__Socket-Network/TcpListenerbasedWebserver.htm
'edited by PNP to work as simple, non-threaded, example
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text
Imports System.IO

Module Webserver
    Private server As WebServer = New WebServer
    Sub Main()
        server.serve()
    End Sub



    Public Class WebServer
        Private tcpListener As System.Net.Sockets.TcpListener
        Private clientSocket As System.Net.Sockets.Socket
        Dim serverIP As IPAddress
        ' Set WWW Root Path
        Dim rootPath As String = "www\"
        ' Set default page
        Dim defaultPage As String = "index.html"

        Public Sub openAddressOfTheServer()
            Dim address As String
            ' Dim hostName As String = Dns.GetHostName()
            'creating a valid address to go to the server's webpage
            address = "http://" + serverIP.ToString + ":8080"
            'opening that address using the default browser
            System.Diagnostics.Process.Start(address)

        End Sub

        Public Sub serve()
            Dim hostName As String = Dns.GetHostName()


            'This FOR loop assigns the first IPv4 address it can find
            For value As Integer = 0 To Dns.GetHostEntry(hostName).AddressList.Length
                If (checkIfIPp4(Dns.GetHostEntry(hostName).AddressList(value))) = True Then
                    serverIP = Dns.GetHostEntry(hostName).AddressList(value)
                    Exit For
                End If
            Next


            Dim Port As String = "8080"

                tcpListener = New TcpListener(serverIP, Int32.Parse(Port))
                tcpListener.Start()
                Console.WriteLine("Web server started at: " & hostName & " " & serverIP.ToString() & ":" & Port)
                While (True)
                    clientSocket = tcpListener.AcceptSocket()
                    ' Socket Information
                    Dim clientInfo As IPEndPoint = CType(clientSocket.RemoteEndPoint, IPEndPoint)
                    Console.WriteLine("Client: " + clientInfo.Address.ToString() + ":" + clientInfo.Port.ToString())
                    ProcessRequest()
                End While
        End Sub

        Function checkIfIPp4(ByVal address As IPAddress) As Boolean

            Dim addressAsString = address.ToString

            If (IPAddress.TryParse(addressAsString, address)) Then
                Select Case address.AddressFamily
                    Case System.Net.Sockets.AddressFamily.InterNetwork
                        'Haha, we have an IPv4 address
                        Return True
                    Case System.Net.Sockets.AddressFamily.InterNetworkV6
                        'Haha, we have an IPv6 address
                        Return False
                    Case Else
                        'Hm, we have something that isn't even an IP address...
                        Return False
                End Select

            End If

            Return False

        End Function

        Protected Sub ProcessRequest()
            Dim recvBytes(1024) As Byte
            Dim htmlReq As String = Nothing
            Dim bytes As Int32

            ' Receive HTTP Request from Web Browser
            'Some browsers send empty requests sometimes, in addition to the real requests.
            Dim tries As Integer = 10
            While tries > 0
                bytes = clientSocket.Receive(recvBytes, 0, clientSocket.Available, SocketFlags.None)
                If bytes > 0 Then
                    tries = 0
                Else
                    tries = tries - 1
                    Console.WriteLine("Failed again, retry")
                    'Pause for a moment (50 milliseconds) to wait for the next request
                    Thread.Sleep(50)
                End If
            End While

            htmlReq = Encoding.ASCII.GetString(recvBytes, 0, bytes)
            Console.WriteLine("HTTP Request: ")
            Console.WriteLine(htmlReq)

            Dim strArray() As String
            Dim strRequest As String

            strArray = htmlReq.Trim.Split(" ")

            ' Determine the HTTP method (GET only)
            If strArray(0).Trim().ToUpper.Equals("GET") Then
                strRequest = strArray(1).Trim

                If (strRequest.StartsWith("/")) Then
                    strRequest = strRequest.Substring(1)
                End If

                If (strRequest.EndsWith("/") Or strRequest.Equals("")) Then
                    strRequest = strRequest & defaultPage
                End If

                strRequest = rootPath & strRequest
                sendHTMLResponse(strRequest)

            ElseIf strArray(0).Trim() <> "" Then ' Not HTTP GET method
                strRequest = rootPath & "Error\400.html"
                sendHTMLResponse(strRequest)
            End If

            clientSocket.Close()
        End Sub

        ' Send HTTP Response
        Private Sub sendHTMLResponse(ByVal httpRequest As String)
            ' Get the file content of HTTP Request
            Dim streamReader As FileStream
            Dim response As String



            If Not File.Exists(httpRequest) Then


                httpRequest = rootPath & "Error\400.html" 'should really be 404.html
                response = "404 Not Found"
            Else
                response = "200 OK"
            End If

            streamReader = New FileStream(httpRequest, IO.FileMode.Open, IO.FileAccess.Read)
            Dim len As Integer = StreamReader.Length
            Dim strBuff(len) As Byte
            StreamReader.Read(strBuff, 0, len)
            StreamReader.Close()
            StreamReader = Nothing

            ' Set HTML Header
            Dim htmlHeader As String = _
              "HTTP/1.0 " & response & ControlChars.CrLf & _
              "Server: WebServer 1.0" & ControlChars.CrLf & _
              "Content-Length: " & len & ControlChars.CrLf & _
              "Content-Type: " & getContentType(httpRequest) & _
               ControlChars.CrLf & ControlChars.CrLf

            ' The content Length of HTML Header
            Dim headerByte() As Byte = Encoding.ASCII.GetBytes(htmlHeader)
            Console.WriteLine("HTML Header: " & ControlChars.CrLf & htmlHeader)
            ' Send HTML Header back to Web Browser
            clientSocket.Send(headerByte, 0, headerByte.Length, SocketFlags.None)

            ' Send HTML Content back to Web Browser
            clientSocket.Send(strBuff, 0, len, SocketFlags.None)

        End Sub

        ' Get Content Type
        Private Function getContentType(ByVal httpRequest As String) As String
            If (httpRequest.EndsWith("html")) Then
                Return "text/html"
            ElseIf (httpRequest.EndsWith("htm")) Then
                Return "text/html"
            ElseIf (httpRequest.EndsWith("txt")) Then
                Return "text/plain"
            ElseIf (httpRequest.EndsWith("gif")) Then
                Return "image/gif"
            ElseIf (httpRequest.EndsWith("jpg")) Then
                Return "image/jpeg"
            ElseIf (httpRequest.EndsWith("jpeg")) Then
                Return "image/jpeg"
            ElseIf (httpRequest.EndsWith("pdf")) Then
                Return "application/pdf"
            ElseIf (httpRequest.EndsWith("pdf")) Then
                Return "application/pdf"
            ElseIf (httpRequest.EndsWith("doc")) Then
                Return "application/msword"
            ElseIf (httpRequest.EndsWith("xls")) Then
                Return "application/vnd.ms-excel"
            ElseIf (httpRequest.EndsWith("ppt")) Then
                Return "application/vnd.ms-powerpoint"
            Else
                Return "text/plain"
            End If
        End Function
    End Class
End Module
