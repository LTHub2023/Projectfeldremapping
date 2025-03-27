
Imports System
Imports System.Data.OleDb
Imports System.IO
Imports System.Collections.Generic

Module Program

    Class MappingEntry
        Public Field As String
        Public MappedField As String
        Public Ref_Value As String
    End Class

    Sub Main()
        Dim dbPath As String = "C:\Path\To\Mapping.mdb"
        Dim fdfFilePath As String = "C:\Path\To\InputFile.fdf"
        Dim outputPath As String = "C:\Path\To\ModifiedFile.fdf"
        Dim formname As String = "FormName"
        Dim logPath As String = Path.ChangeExtension(outputPath, ".log")

        ' === 1. Load mappings from Access DB ===
        Dim allMappings As New List(Of MappingEntry)
        Dim connStr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbPath & ";"

        Using conn As New OleDbConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT Field, MappedField_Feldname, Ref_Value FROM MappingTable WHERE Formname = '" & formname & "'"
            Using cmd As New OleDbCommand(sql, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim entry As New MappingEntry()
                        entry.Field = reader("Field").ToString().Trim()
                        entry MappedField = reader( "MappedField_Feldname").ToString().Trim()
                        entry.Ref_Value = reader("Ref_Value").ToString().Trim()
                        allMappings.Add(entry)
                    End While
                End Using
            End Using
        End Using

        ' === 2. Read FDF file lines ===
        Dim linesArray() As String = File.ReadAllLines(fdfFilePath)
        Dim lines As New List(Of String)(linesArray)
        Dim log As New List(Of String)

        ' === 3. Group mappings by field ===
        Dim feldGroups As New Dictionary(Of String, List(Of MappingEntry))
        For Each m As MappingEntry In allMappings
            If Not feldGroups.ContainsKey(m.Field) Then
                feldGroups(m.Field) = New List(Of MappingEntry)
            End If
            feldGroups(m.Field).Add(m)
        Next

        ' === 4. Apply replacements ===
        For i As Integer = 0 To lines.Count - 1
            Dim currentLine As String = lines(i)
            Dim lineChanged As Boolean = False

            For Each feldKey As String In feldGroups.Keys
                Dim entryGroup As List(Of MappingEntry) = feldGroups(feldKey)
                Dim searchTag As String = "<p><b>" & feldKey & "</b>"

                If currentLine.IndexOf(searchTag) = -1 Then
                    Continue For
                End If

                ' Try 1:1 mapping
                For Each entry As MappingEntry In entryGroup
                    If entry.Ref_Value = "" Then
                        Dim newTag As String = "<p><b>" & entry MappedField & "</b>"
                        currentLine = currentLine.Replace(searchTag, newTag)
                        log.Add("Line " & (i + 1).ToString() & ": [1:1] Replaced '" & feldKey & "' with '" & entry MappedField & "'")
                        lineChanged = True
                        GoTo NextLine
                    End If
                Next

                ' Try 1:N mapping based on Value:
                Dim valueMarker As String = "Value: "
                Dim valueIndex As Integer = currentLine.IndexOf(valueMarker)
                If valueIndex > -1 Then
                    Dim valuePart As String = currentLine.Substring(valueIndex + valueMarker.Length)
                    Dim valueEnd As Integer = valuePart.IndexOf("&#10;")
                    Dim valueRaw As String = If(valueEnd > -1, valuePart.Substring(0, valueEnd).Trim(), valuePart.Trim())

                    For Each entry As MappingEntry In entryGroup
                        If entry.Ref_Value = valueRaw Then
                            Dim newTag As String = "<p><b>" & entry MappedField & "</b>"
                            currentLine = currentLine.Replace(searchTag, newTag)
                            log.Add("Line " & (i + 1).ToString() & ": [1:N] Replaced '" & feldKey & "' with '" & entry MappedField & "' (Value: " & valueRaw & ")")
                            lineChanged = True
                            GoTo NextLine
                        End If
                    Next
                End If
            Next

            ' If no replacement was made, log NO CHANGE
            If Not lineChanged Then
                Dim tagStart As Integer = currentLine.IndexOf("<p><b>")
                If tagStart > -1 Then
                    Dim tagEnd As Integer = currentLine.IndexOf("</b>", tagStart)
                    If tagEnd > tagStart Then
                        Dim originalTag As String = currentLine.Substring(tagStart, (tagEnd - tagStart) + 5)
                        log.Add("Line " & (i + 1).ToString() & ": [NO CHANGE] Tag found in line: " & originalTag)
                    Else
                        log.Add("Line " & (i + 1).ToString() & ": [NO CHANGE] no tag in line")
                    End If
                Else
                    log.Add("Line " & (i + 1).ToString() & ": [NO CHANGE] no tag in line")
                End If
            End If

NextLine:
        Next

        ' === 5. Save output and log ===
        File.WriteAllLines(outputPath, lines.ToArray())
        File.WriteAllLines(logPath, log.ToArray())

        Console.WriteLine("Remapping completed.")
        Console.WriteLine("Output: " & outputPath)
        Console.WriteLine("Log: " & logPath)
        Console.ReadLine()
    End Sub
End Module
