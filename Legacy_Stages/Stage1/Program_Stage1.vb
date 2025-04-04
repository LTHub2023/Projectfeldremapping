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
        Dim outputPath As String = "C:\Path\To\InputFile_ModifiedFile.fdf"
        Dim formname As String = "FormName"
        Dim logPath As String = Path.ChangeExtension(outputPath, ".log")

        ' === 1. Load mappings from Access DB ===
        Dim allMappings As New List(Of MappingEntry)
        Dim connStr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbPath & ";"

        Using conn As New OleDbConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT Field, Mapping_Fieldname, Ref_Value FROM MappingTable WHERE Formname = '" & formname & "'"
            Using cmd As New OleDbCommand(sql, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim entry As New MappingEntry()
                        entry.Field = reader("Field").ToString().Trim()
                        entry.MappedField = reader("Mapping_Fieldname").ToString().Trim()
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
        Dim fieldGroups As New Dictionary(Of String, List(Of MappingEntry))
        For Each m As MappingEntry In allMappings
            If Not fieldGroups.ContainsKey(m.Field) Then
                fieldGroups(m.Field) = New List(Of MappingEntry)
            End If
            fieldGroups(m.Field).Add(m)
        Next

        ' === 4. Apply replacements ===
        For i As Integer = 0 To lines.Count - 1
            Dim currentLine As String = lines(i)

            For Each fieldKey As String In fieldGroups.Keys
                Dim entryGroup As List(Of MappingEntry) = fieldGroups(fieldKey)
                Dim searchTag As String = "<p><b>" & fieldKey & "</b>"

                If currentLine.IndexOf(searchTag) = -1 Then
                    Continue For
                End If

                ' Try 1:1 mapping
                For Each entry As MappingEntry In entryGroup
                    If entry.Ref_Value = "" Then
                        Dim newTag As String = "<p><b>" & entry.MappedField & "</b>"
                        lines(i) = currentLine.Replace(searchTag, newTag)
                        log.Add("Line " & (i + 1).ToString() & ": [1:1] Replaced '" & fieldKey & "' with '" & entry.MappedField & "'")
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
                            Dim newTag As String = "<p><b>" & entry.MappedField & "</b>"
                            lines(i) = currentLine.Replace(searchTag, newTag)
                            log.Add("Line " & (i + 1).ToString() & ": [1:N] Replaced '" & fieldKey & "' with '" & entry.MappedField & "' (Value: " & valueRaw & ")")
                            GoTo NextLine
                        End If
                    Next
                End If
            Next

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
