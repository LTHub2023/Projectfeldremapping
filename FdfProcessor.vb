Imports System.IO
Imports System.Text.RegularExpressions

Public Module FdfProcessor
    Public Function ProcessFdfLines(
        fdfLines As List(Of String),
        allMappings As List(Of MappingEntry)
    ) As (ModifiedLines As List(Of String), LogLines As List(Of String))

        If fdfLines Is Nothing OrElse fdfLines.Count = 0 Then
            Return (New List(Of String), New List(Of String))
        End If

        If allMappings Is Nothing Then
            allMappings = New List(Of MappingEntry)
        End If

        Dim groupedMappings = GroupMappingsByField(allMappings)

        Dim log As New List(Of String)

        Dim modifiedLines = fdfLines.Select(
            Function(line, idx) ProcessLine(line, idx, groupedMappings, log)
        ).ToList()

        Return (modifiedLines, log)
    End Function

    Private Function GroupMappingsByField(mappings As List(Of MappingEntry)) _
        As Dictionary(Of String, List(Of MappingEntry))

        Return mappings.
            GroupBy(Function(m) m.Feld).
            ToDictionary(Function(g) g.Key,
                         Function(g) g.ToList(),
                         StringComparer.OrdinalIgnoreCase)
    End Function

    Private Function ProcessLine(
        line As String,
        idx As Integer,
        grouped As Dictionary(Of String, List(Of MappingEntry)),
        log As List(Of String)
    ) As String

        Dim originalLine = line
        Dim changed = False

        grouped.Keys.ToList().ForEach(
            Sub(feldKey)
                line = ProcessMappingsForField(line, idx, feldKey, grouped(feldKey), log, changed)
            End Sub)

        If Not changed Then
            LogNoChange(line, idx, log)
        End If

        Return line
    End Function

    Private Function ProcessMappingsForField(
        line As String,
        idx As Integer,
        feldKey As String,
        entryGroup As List(Of MappingEntry),
        log As List(Of String),
        ByRef changed As Boolean
    ) As String

        Dim pattern = "<p><b>" & Regex.Escape(feldKey) & "(_\d+)?</b>"
        Dim matches = Regex.Matches(line, pattern)

        matches.Cast(Of Match).ToList().ForEach(
            Sub(mt)
                line = ReplaceUsingMapping(line, idx, feldKey, mt.Value, entryGroup, log, changed)
            End Sub)

        Return line
    End Function

    Private Function ReplaceUsingMapping(
        line As String,
        idx As Integer,
        feldKey As String,
        matchedTag As String,
        entryGroup As List(Of MappingEntry),
        log As List(Of String),
        ByRef changed As Boolean
    ) As String

        Dim suffix = ExtractSuffix(matchedTag, feldKey)

        Dim replaced = False

        entryGroup.Where(Function(e) String.IsNullOrWhiteSpace(e.MC_Wert)).ToList().ForEach(
            Sub(e)
                If Not replaced Then
                    Dim newTag = $"<p><b>{e.CDASH}{suffix}</b>"
                    If line.Contains(matchedTag) Then
                        line = line.Replace(matchedTag, newTag)
                        log.Add($"Line {idx + 1}: [1:1] Replace '{feldKey}{suffix}' -> '{e.CDASH}{suffix}'")
                        replaced = True
                        changed = True
                    End If
                End If
            End Sub)

        If replaced Then Return line

        Dim rawValue = ExtractRawValue(line)

        If rawValue IsNot Nothing Then
            entryGroup.
                Where(Function(e) e.MC_Wert.Equals(rawValue, StringComparison.OrdinalIgnoreCase)).
                ToList().
                ForEach(Sub(e)
                    Dim newTag = $"<p><b>{e.CDASH}{suffix}</b>"
                    If line.Contains(matchedTag) Then
                        line = line.Replace(matchedTag, newTag)
                        log.Add($"Line {idx + 1}: [1:N] Replace '{feldKey}{suffix}' -> '{e.CDASH}{suffix}' (Value: {rawValue})")
                        changed = True
                    End If
                End Sub)
        End If

        Return line
    End Function

    Private Function ExtractSuffix(matchedTag As String, feldKey As String) As String
        Dim suffixMatch = Regex.Match(matchedTag, feldKey & "(_\d+)?")
        Return If(suffixMatch.Groups.Count > 1 AndAlso suffixMatch.Groups(1).Success,
                  suffixMatch.Groups(1).Value,
                  "")
    End Function

    Private Function ExtractRawValue(line As String) As String
        Dim valueMarker = "Value: "
        Dim valueIndex = line.IndexOf(valueMarker)
        If valueIndex = -1 Then Return Nothing

        Dim valuePart = line.Substring(valueIndex + valueMarker.Length)
        Dim endIndex = valuePart.IndexOf("&#10;")

        Return If(endIndex > -1,
                  valuePart.Substring(0, endIndex).Trim(),
                  valuePart.Trim())
    End Function

    Private Sub LogNoChange(line As String, idx As Integer, log As List(Of String))
        Dim tagStart = line.IndexOf("<p><b>")
        If tagStart > -1 Then
            Dim tagEnd = line.IndexOf("</b>", tagStart)
            If tagEnd > tagStart Then
                Dim content = line.Substring(tagStart + 6, tagEnd - (tagStart + 6))
                log.Add($"Line {idx + 1}: [NO CHANGE] Tag found: {content}")
            Else
                log.Add($"Line {idx + 1}: [NO CHANGE]")
            End If
        Else
            log.Add($"Line {idx + 1}: [NO CHANGE]")
        End If
    End Sub
End Module