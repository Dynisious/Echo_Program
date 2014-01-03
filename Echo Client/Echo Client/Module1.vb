'Imports is basically just shortcutting so instead of me having to type 'system.net.sockets.[namespace]' i just type [namespace]
'its useful for things you are going to have to access a lot
Imports System.Net.Sockets
Imports System.IO

'This is the client app
'From here the client can send messages to the server
Module Module1

    Sub Main()

        'try statements are very useful, they're a way to handle exceptions how you want them handled rather than just crashing the app
        'but don't use them as a way to fix bugs!!! only as a way to account for human error like i have.
        Try
            Dim serverIp As String 'This is the IP Address of the machine im accessing as a string
            Dim serverSocket As Integer 'This is the port number the server is listening on
            Dim serverSender As TcpClient 'this is a TCPClient, it holds a 'Socket', which ill explain in the 'READ ME', and it also
            'contains useful functions for messeging a machine
            While True 'this 'While loop' will loop forever unless i call 'exit while'
                '-----This collects the IP Address-----
                Console.WriteLine("Input Server IP") 'this writes the sting on the console
                serverIp = Console.ReadLine() 'this function waits untill the user presses enter to write to the string
                '--------------------------------------

                '-----This collects the port number-----
                Console.WriteLine("Input Listening Port")
                serverSocket = CInt(Console.ReadLine()) 'this function waits untill the user presses enter to write to the string 
                'and 'CInt converts it to an integer
                '---------------------------------------

                Console.WriteLine("Connecting to localhost")
                Try 'This 'try' tries to connect to the IP Address and Port the user specifies
                    serverSender = New TcpClient(serverIp, serverSocket) 'Connects to the server
                    Exit While 'Exits the loop so that it stops looping
                Catch exp As SocketException 'The code inside the 'try' jumps to here if theres a socket exception
                    'rather than just crashing
                    Console.WriteLine("Either your inputed IP Address or Port Number are incorrect or the server is not available:")
                    Console.WriteLine(exp.ToString) 'This writes all the information on the exception to the console
                End Try
            End While
            Dim sendStream As Stream = serverSender.GetStream 'the 'stream' can be thought of as a tunnel with the client on one end
            'and the server on the other and packets get passed through the tunnel and gets checked at each end. this is the stream used by the
            ''TcpClient' we made earlier
            serverSender.SendBufferSize = 225 'This sets the maximum number of bytes we can send to the server through the 'TcpClient'
            Console.WriteLine("Connected to '" + serverIp + ":" + CStr(serverSocket) + "'") ''CStr' converts the integer to a string
            Console.WriteLine("Input Lines:")
            Dim str As String = Console.ReadLine()
            While True 'loops forever
                Dim sendBuff() As Byte = System.Text.Encoding.ASCII.GetBytes(str) 'This is an array of bytes that equals the string 'str' as an
                'array of bytes, this is so we can send it to the server because everything that gets sent across networks is in bytes
                sendStream.Write(sendBuff, 0, sendBuff.Length) 'This writes to the stream so that the server can read our message, we pass in
                'the array of bytes, where in the array we start and how many bytes in the array we go through
                If str.StartsWith(".") Then 'checks to see if the start of the string is a '.', it can be as many characters as we want though
                    'so i could see if it starts with 'apple pie' for instance
                    Exit While 'exits the while loop because the message starts with '.'
                End If
                str = Console.ReadLine() 'the code waits here untill the user presses enter then goes to the top of the loop
            End While
            Console.WriteLine("Done")
        Catch exp As Exception 'this catches any exception while this try statement is running
            Console.WriteLine("Exception:")
            Console.Write(exp.ToString()) 'writes out the details of the exception
        End Try
    End Sub
End Module
