Imports System.Net.Sockets
Imports System.IO

Module Module1
    Public sockets(0) As Socket 'this is the list of 'sockets' that the server listens too

    Public Sub main()
        Dim port As Integer 'this is the Port Number it will be listening for new clients on
        While True 'loops forever
            Console.WriteLine("Input Port to listen on")
            Try
                port = CInt(Console.ReadLine()) 'sets the port number
                Exit While 'exits the loop
            Catch exp As Exception 'this triggers if the port doesn't work as an integer and clears the console
                Console.Clear() 'clears the console
            End Try
        End While
        Dim listener As New TcpListener(port) 'this opens a 'TcpListener' (will be discussed in the READ ME) on the port number specified,
        'this will listen for any new requests and is used so that the same port is always open and listening for new clients rather
        'than making a new port for every client to connect to
        sockets(0) = listener.Server 'a 'TcpListener' isnt a socket but it contains a socket so thats what is stored in my array
        listener.Start() 'this tells the 'TcpListener' to start listening for connection requests
        Console.WriteLine("Server: Listening For Conection Requests on port " + CStr(port))
        While True 'loops forever
            Dim waiting As New ArrayList() 'this is an array list which is like an array except its size changes as needed and also it handles
            'its 'objects' differently to a normal array
            For Each i As Socket In sockets 'this loops through each socket in my array called 'sockets' no matter how many there are,
                'it also stores the one being looped through in the variable 'i'
                waiting.Add(i) 'this adds the 'socket' in 'i' to the array list
            Next
            Socket.Select(waiting, Nothing, Nothing, -1) 'this 'Socket.Select' function takes three parentheses, if you put an ArrayList of
            'sockets into the first parenthese it removes all of the sockets that don't have information waiting to be read; the second
            'leaves all the sockets that have nothing waiting to be read and can be writen too; the third leaves only the sockets that
            'are throwing errors and the integer is how many microseconds the function wait's for at least one 'Socket' to respond before
            'it times-out and the code continues to run, '-1' makes it wait forever
            For Each i As Socket In waiting 'this loops through each of the 'Sockets' left in waiting after the 'Select' function went through it
                If ReferenceEquals(i, listener.Server) Then 'this checks to see if the 'Socket' is the same one as our listener because
                    'we dont want to read the listener we want to make a new 'Socket' to listen to the client
                    AddClient(listener) 'goes to my subroutine and passes in our 'TcpListener'
                Else
                    RecieveData(i) 'Goes to my subroutine and passes in the 'Socket' to be read
                End If
            Next
        End While
    End Sub

    Public Sub AddClient(ByRef nSocket As TcpListener) 'This adds a new client connection to the server
        ReDim Preserve sockets(sockets.Length) 'add a space at the end of the list
        sockets(UBound(sockets)) = nSocket.AcceptSocket() 'the 'AcceptSocket()' function generates a new 'LocalEndPoint' (Discussed in the READ ME)
        'for the socket and then makes the 'TcpListener' go back to listening for connections while the new 'Socket' talks with the client
        Console.WriteLine("Server: Accepted connection of 'Client" + CStr(UBound(sockets)) + "'")
    End Sub

    Public Sub RecieveData(ByRef socket As Socket) 'this prints out the messege sent by a client
        Try
            Dim receiveBuff(225) As Byte 'this is the arraye of bytes that we put the message in
            Dim receivedBuffLength As Integer 'this tells us how long the actual message is so that we don't have a lot of empty bytes at the end
            'of it
            receivedBuffLength = socket.Receive(receiveBuff, receiveBuff.Length, SocketFlags.None) 'this sets the integer and stores the message;
            'the first parenthese is the array of bytes we write the message to, the second is how many bytes we read from the message and
            'the third is any socket flags used (dont worry about those)
            Dim str As String = System.Text.Encoding.ASCII.GetString(receiveBuff, 0, receivedBuffLength).Trim(ChrW(0)) 'this converts the
            'array of bytes into a string; we pass in the array of bytes, where in the array we start and how many bytes we go through in
            'the array
            If str.StartsWith(".") = False Then 'this checks if it starts with '.'
                Console.WriteLine("Client" + CStr(Array.IndexOf(sockets, socket)) + ": " + str) 'this writes out the message and puts which
                'client it was sent from in front of the message
            Else
                Dim index As Integer = Array.IndexOf(sockets, socket) 'this gets the integer possition of the 'Socket' in the list of sockets
                EndClient(index) 'this goes to my subroutine and passes in the index of the 'Socket'
                Console.WriteLine("Server: 'Client" + CStr(index) + "'" + " has disconected")
            End If
        Catch exp As SocketException 'this catches the 'SocketException' exception only and none of the others
            For Each i As Socket In sockets 'loops through all of the sockets
                If Equals(socket, i) Then 'checks if we are looking at the same socket
                    Console.WriteLine("Server: 'Client{0}' was unexpectedly diconnected:", CStr(Array.IndexOf(sockets, i)))
                    EndClient(Array.IndexOf(sockets, i)) 'goes to my subroutine and passes in the index of the 'Socket' in the list
                    Exit For 'exits the 'For Each' loop
                End If
            Next
            Console.WriteLine(exp.ToString())
        Catch exp As Exception 'this catches any other exception
            For Each i As Socket In sockets
                If Equals(socket, i) Then
                    Console.WriteLine("Server: an unknown exception occoured with 'client{0}':", CStr(Array.IndexOf(sockets, i)))
                    EndClient(Array.IndexOf(sockets, i))
                    Exit For
                End If
            Next
            Console.WriteLine(exp.ToString())
        End Try
    End Sub

    Public Sub EndClient(ByVal index As Integer) 'this removes the clients 'Socket' from the list
        sockets(index).Close() 'closes the socket so that it gets erased
        For i As Integer = index To UBound(sockets) 'goes through all of the sockets above the one we are working on
            If i <> UBound(sockets) Then
                sockets(i) = sockets(i + 1) 'makes the selected 'Sockets' variable equal the one above it so the the closed one is
                'over writen
            End If
        Next
        ReDim Preserve sockets(UBound(sockets) - 1) 'removes the space at the end of the list
    End Sub
End Module
