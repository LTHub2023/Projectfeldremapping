
Imports System
Imports System.Data.OleDb
Imports System.IO
Imports System.Collections.Generic
Imports System.Text.RegularExpressions

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

        ' Check if the FDF file and database file exist
		If Not System.IO.File.Exists(fdfFilePath) Then
			Console.WriteLine("❌ FDF file not found: " & fdfFilePath)
			Console.ReadLine()
			Return ' This stops the program here
		End If

		If Not System.IO.File.Exists(dbPath) Then
			Console.WriteLine("❌ Database file not found: " & dbPath)
			Console.ReadLine()
			Return ' This stops the program here
		End If
		
        ' Debug mode
        ' Set to True to enable debug output
        ' Set to False to disable debug output
        ' This is a simple way to control debug output without using a full logging framework    
		Dim debugMode As Boolean = true ' Set to False to disable debug output

        If debugMode Then Console.WriteLine("Opening database and loading mappings...")
		
		If debugMode Then
			Console.WriteLine("FDF file path: " & fdfFilePath) ' show tha path
			Console.WriteLine("Output path: " & outputPath) ' show tha path
			Console.WriteLine("DB path: " & dbPath) ' show tha path
		End If

        If debugMode Then Console.WriteLine("Form name: " & formname) ' show tha path
        If debugMode Then Console.WriteLine("Log path: " & logPath) ' show tha path
        If debugMode Then Console.WriteLine("Loading mappings from Access DB...")

        ' === 1. Load mappings from Access DB ===
        Dim allMappings As New List(Of MappingEntry)
        Dim connStr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbPath & ";"

        Using conn As New OleDbConnection(connStr)
            conn.Open()
            Dim sql As String = "SELECT Field, Mapping_Fieldname, Ref_Value FROM [MappingTable] WHERE Formname = '" & formname & "'"
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
        
        If debugMode Then Console.WriteLine("Mappings loaded: " & allMappings.Count.ToString()) 

        ' === 2. Read FDF file lines into List(Of String) ===
        Dim linesArray() As String = File.ReadAllLines(fdfFilePath)
        Dim lines As New List(Of String)(linesArray)
        Dim log As New List(Of String)
        if debugMode Then Console.WriteLine("FDF file lines loaded: " & lines.Count.ToString()) 
        
        ' === 3. Group mappings by Field manually ===
        Dim fieldGroups As New Dictionary(Of String, List(Of MappingEntry))
        Dim m As MappingEntry
        For Each m In allMappings
            If Not fieldGroups.ContainsKey(m.Field) Then
                fieldGroups(m.Field) = New List(Of MappingEntry)
            End If
            fieldGroups(m.Field).Add(m)
        Next
        If debugMode Then Console.WriteLine("Grouped mappings by field.")

        ' === 4. Apply replacements ===
        For i As Integer = 0 To lines.Count - 1
            Dim currentLine As String = lines(i)
            Dim lineChanged As Boolean = False

            For Each fieldKey As String In fieldGroups.Keys
                Dim entryGroup As List(Of MappingEntry) = fieldGroups(fieldKey)
                Dim regexPattern As String = "<p><b>" & Regex.Escape(fieldKey) & "(_\d+)?</b>"
                Dim matches As MatchCollection = Regex.Matches(currentLine, regexPattern)

                For Each match As Match In matches
                    Dim matchedTag As String = match.Value
                    Dim suffixMatch As Match = Regex.Match(matchedTag, fieldKey & "(_\d+)?")
                    Dim suffix As String = ""
                    If suffixMatch.Groups.Count > 1 AndAlso suffixMatch.Groups(1).Success Then
                        suffix = suffixMatch.Groups(1).Value
                        If debugMode Then Console.WriteLine("    Suffix detected: " & suffix)
                    End If

                    ' 1:1 replacement (no Ref_Value)
                    Dim replaced As Boolean = False
                    Dim entry As MappingEntry
                    For Each entry In entryGroup
                        If entry.Ref_Value Is Nothing OrElse entry.Ref_Value.Trim() = "" Then
                            Dim newTag As String = "<p><b>" & entry.MappedField & suffix & "</b>"
                            currentLine = currentLine.Replace(matchedTag, newTag)
                            log.Add("Line " & (i + 1).ToString() & ": [1:1] Replaced '" & fieldKey & suffix & "' with '" & entry.MappedField & suffix & "'")
                            replaced = True
                            lineChanged = True
                            Exit For
                        End If
                    Next

                    ' 1:NO replacement (with MC_Wert)
                    If Not replaced Then
                        Dim valueMarker As String = "Value: "
                        Dim valueIndex As Integer = currentLine.IndexOf(valueMarker)
                        If valueIndex > -1 Then
                            Dim valuePart As String = currentLine.Substring(valueIndex + valueMarker.Length)
                            Dim valueEnd As Integer = valuePart.IndexOf("&#10;")
                            Dim valueRaw As String
                            If valueEnd > -1 Then
                                valueRaw = valuePart.Substring(0, valueEnd).Trim()
                            Else
                                valueRaw = valuePart.Trim()
                            End If
                            If debugMode Then Console.WriteLine("    Value detected: " & valueRaw)
                            ' Check if the value matches any Ref_Value in the group
                            For Each entry In entryGroup
                                If entry.Ref_Value = valueRaw Then
                                    Dim newTag As String = "<p><b>" & entry.MappedField & suffix & "</b>"
                                    currentLine = currentLine.Replace(matchedTag, newTag)
                                    log.Add("Line " & (i + 1).ToString() & ": [1:N] Replaced '" & fieldKey & suffix & "' with '" & entry.MappedField & suffix & "' (Value: " & valueRaw & ")")
                                    If debugMode Then Console.WriteLine("    [1:N] Replacement applied")
                                    lineChanged = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Next
            Next

            lines(i) = currentLine

            If Not lineChanged Then
                Dim tagStart As Integer = currentLine.IndexOf("<p><b>")
                If tagStart > -1 Then
                    Dim tagEnd As Integer = currentLine.IndexOf("</b>", tagStart)
                    If tagEnd > tagStart Then
                        Dim tagContent As String = currentLine.Substring(tagStart + 6, tagEnd - (tagStart + 6))
                        log.Add("Line " & (i + 1).ToString() & ": [NO CHANGE] Tag found in line: " & tagContent)
                        If debugMode Then Console.WriteLine("    [NO CHANGE] Tag: " & tagContent)
                    Else
                        log.Add("Line " & (i + 1).ToString() & ": [NO CHANGE]")
                        If debugMode Then Console.WriteLine("    [NO CHANGE] No tag found")    
                    End If
                Else
                    log.Add("Line " & (i + 1).ToString() & ": [NO CHANGE]")
                End If
            End If
        Next

        ' === 5. Save modified file and log ===
        File.WriteAllLines(outputPath, lines.ToArray())
        File.WriteAllLines(logPath, log.ToArray())

        Console.WriteLine("FDF field name replacements completed.")
        Console.WriteLine("Modified file saved to: " & outputPath)
        Console.WriteLine("Log saved to: " & logPath)
        Console.ReadLine()
    End Sub
End Module