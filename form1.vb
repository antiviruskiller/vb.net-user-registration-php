Imports System.Text
Imports System.Security.Cryptography
Imports Microsoft.VisualBasic.FileIO
Imports System.IO
Imports System.Net
Imports System.Collections.Specialized

Public Class Form1


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
      Dim username As String = TextBox1.Text
        Dim password As String = TextBox2.Text
        Dim encryptedPassword As String = EncryptString(password, "YourEncryptionKey1234")

        Dim request As WebRequest = WebRequest.Create("https://haxcore.net/aes/register.php")
        request.Method = "POST"
        Dim postData As String = "username=" & Uri.EscapeDataString(username) & "&password=" & Uri.EscapeDataString(encryptedPassword)
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        MessageBox.Show(responseFromServer)
        reader.Close()
        dataStream.Close()
        response.Close()
    End Sub

    Function EncryptString(ByVal input As String, ByVal key As String) As String
        ' Convert the key string to a valid TripleDES key
        Dim desKey As Byte() = GenerateTripleDesKey(key)

        ' Create a TripleDESCryptoServiceProvider instance
        Dim des As New TripleDESCryptoServiceProvider()
        des.Key = desKey
        des.Mode = CipherMode.ECB
        des.Padding = PaddingMode.PKCS7

        ' Create an encryptor
        Dim transform As ICryptoTransform = des.CreateEncryptor()

        ' Convert input string to bytes
        Dim data As Byte() = Encoding.ASCII.GetBytes(input)

        ' Encrypt the data
        Dim result As Byte() = transform.TransformFinalBlock(data, 0, data.Length)

        ' Return base64-encoded encrypted result
        Return Convert.ToBase64String(result)
    End Function

    Function GenerateTripleDesKey(ByVal key As String) As Byte()
        ' TripleDES key size is either 128 bits (16 bytes) or 192 bits (24 bytes)
        ' We'll use SHA-256 to generate a 256-bit (32 bytes) hash from the input key
        Dim sha256 As New System.Security.Cryptography.SHA256Managed()
        Dim keyBytes As Byte() = Encoding.ASCII.GetBytes(key)
        Dim hashedKey As Byte() = sha256.ComputeHash(keyBytes)

        ' Take the first 24 bytes of the hash to use as the TripleDES key
        Dim desKey As Byte() = New Byte(23) {}
        Array.Copy(hashedKey, desKey, 24)

        Return desKey
    End Function
End Class
