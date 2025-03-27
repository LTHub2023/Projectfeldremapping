Imports System
Imports System.Data.OleDb
Imports System.IO
Imports System.Collections.Generic

Module Program

    Class MappingEntry
        Public Feld As String
        Public CDASH As String
        Public MC_Wert As String
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
            Dim sql As String = "SELECT Feld, CDASH_Feldname, MC_Wert FROM MappingTable WHERE Formname = '" & formname & "'"
            Using cmd As New OleDbCommand(sql, conn)
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim entry As New MappingEntry()
                        entry.Feld = reader("Feld").ToString().Trim()
                        entry.CDASH = reader("CDASH_Feldname").ToString().Trim()
                        entry.MC_Wert = reader("MC_Wert").ToString().Trim()
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
            If Not feldGroups.ContainsKey(m.Feld) Then
                feldGroups(m.Feld) = New List(Of MappingEntry)
            End If
            feldGroups(m.Feld).Add(m)
        Next

        ' === 4. Apply replacements ===
        For i As Integer = 0 To lines.Count - 1
            Dim currentLine As String = lines(i)

            For Each feldKey As String In feldGroups.Keys
                Dim entryGroup As List(Of MappingEntry) = feldGroups(feldKey)
                Dim searchTag As String = "<p><b>" & feldKey & "</b>"

                If currentLine.IndexOf(searchTag) = -1 Then
                    Continue For
                End If

                ' Try 1:1 mapping
                For Each entry As MappingEntry In entryGroup
                    If entry.MC_Wert = "" Then
                        Dim newTag As String = "<p><b>" & entry.CDASH & "</b>"
                        lines(i) = currentLine.Replace(searchTag, newTag)
                        log.Add("Line " & (i + 1).ToString() & ": [1:1] Replaced '" & feldKey & "' with '" & entry.CDASH & "'")
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
                        If entry.MC_Wert = valueRaw Then
                            Dim newTag As String = "<p><b>" & entry.CDASH & "</b>"
                            lines(i) = currentLine.Replace(searchTag, newTag)
                            log.Add("Line " & (i + 1).ToString() & ": [1:N] Replaced '" & feldKey & "' with '" & entry.CDASH & "' (Value: " & valueRaw & ")")
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
