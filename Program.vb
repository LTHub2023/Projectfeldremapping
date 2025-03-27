Imports System.IO

Module Program
    Sub Main()
        ' Load config from INI file
        Dim config = LoadConfiguration("AppConfig.ini")

        ' Extract configuration values
        Dim dbPath = GetValueOrDefault(config, "dbPath")
        Dim fdfPath = GetValueOrDefault(config, "fdfFilePath")
        Dim outputPath = GetValueOrDefault(config, "outputPath")
        Dim formName = GetValueOrDefault(config, "formName")

        ' Load mappings from database
        Dim mappings = LoadMappingsFromDatabase(dbPath, formName)

        ' Read and process FDF
        If Not File.Exists(fdfPath) Then
            Console.WriteLine("FDF file not found.")
            Return
        End If

        ProcessFdfFile(fdfPath, outputPath, mappings)

        Console.WriteLine("Done.")
        Console.ReadLine()
    End Sub

    ' Load INI configuration file
    Private Function LoadConfiguration(path As String) As Dictionary(Of String, String)
        Return ConfigIniReader.LoadConfig(path)
    End Function

    ' Get value or default empty string
    Private Function GetValueOrDefault(config As Dictionary(Of String, String), key As String) As String
        Return If(config.ContainsKey(key), config(key), "")
    End Function

    ' Load mappings from the database
    Private Function LoadMappingsFromDatabase(path As String, formName As String) As Dictionary(Of String, String)
        Return MappingsDatabaseReader.LoadMappings(path, formName)
    End Function

    ' Process the FDF file
    Private Sub ProcessFdfFile(fdfPath As String, outputPath As String, mappings As Dictionary(Of String, String))
        Dim fdfLines = File.ReadAllLines(fdfPath).ToList()
        Dim result = FdfProcessor.ProcessFdfLines(fdfLines, mappings)

        SaveModifiedFdfAndLog(outputPath, result.ModifiedLines, result.LogLines)
    End Sub

    ' Save the modified FDF and log to files
    Private Sub SaveModifiedFdfAndLog(outputPath As String, modifiedLines As List(Of String), logLines As List(Of String))
        If String.IsNullOrEmpty(outputPath) Then
            Console.WriteLine("No valid output path in config.")
            Return
        End If

        File.WriteAllLines(outputPath, modifiedLines)
        Dim logPath = Path.ChangeExtension(outputPath, ".log")
        File.WriteAllLines(logPath, logLines)

        Console.WriteLine($"Modified FDF saved to: {outputPath}")
        Console.WriteLine($"Log saved to: {logPath}")
    End Sub
End Module
