Imports System.IO
Imports System.Text.RegularExpressions

Public Module ConfigIniReader
    ''' <summary>
    ''' Loads key-value pairs from an INI file. Each line is parsed if it matches the pattern "key=value",
    ''' and lines starting with "#" or ";" are treated as comments and ignored.
    ''' </summary>
    ''' <param name="iniFilePath">The full path to the INI file.</param>
    ''' <returns>A Dictionary containing all valid key-value pairs.</returns>
    Public Function LoadConfig(iniFilePath As String) As Dictionary(Of String, String)
        ' If the file does not exist, return an empty dictionary.
        If Not File.Exists(iniFilePath) Then
            Return New Dictionary(Of String, String)()
        End If

        ' Read all lines from the specified file.
        Dim lines As String() = File.ReadAllLines(iniFilePath)

        ' Regex pattern to match "key=value" while ignoring lines that begin with # or ;.
        ' Group "key" captures everything before '=', and Group "value" captures everything after '='.
        Dim pattern As New Regex("^\s*(?<key>[^#;]+?)\s*=\s*(?<value>.*?)\s*$", RegexOptions.Compiled)

        ' Convert each line into a Match object, filter out those that don't match,
        ' then build a case-insensitive dictionary out of the key-value pairs.
        Return lines.
            Select(Function(line) pattern.Match(line)).
            Where(Function(match) match.Success).
            ToDictionary(
                Function(match) match.Groups("key").Value.Trim(),
                Function(match) match.Groups("value").Value.Trim(),
                StringComparer.OrdinalIgnoreCase
            )
    End Function
End Module
