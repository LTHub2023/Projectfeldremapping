Imports System.Data.OleDb

Public Module MappingsDatabaseReader

    ' Entry point: Load mapping entries from the database
    Public Function LoadMappings(dbPath As String, formName As String) As List(Of MappingEntry)
        If Not IsValidInput(dbPath, formName) Then Return New List(Of MappingEntry)()
        Using connection = CreateConnection(dbPath)
            connection.Open()
            Return QueryMappingEntries(connection, formName)
        End Using
    End Function

    ' Validate input parameters
    Private Function IsValidInput(dbPath As String, formName As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(dbPath) AndAlso Not String.IsNullOrWhiteSpace(formName)
    End Function

    ' Create a database connection with the given path
    Private Function CreateConnection(dbPath As String) As OleDbConnection
        Dim connStr = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={dbPath};"
        Return New OleDbConnection(connStr)
    End Function

    ' Query the database and map the result to MappingEntry list
    Private Function QueryMappingEntries(conn As OleDbConnection, formName As String) As List(Of MappingEntry)
        Dim sql = $"SELECT Feld, CDASH_Feldname, MC_Wert FROM [CDASH_Mapping_localtest] WHERE Formname = '{formName}'"
        Using cmd As New OleDbCommand(sql, conn)
            Using reader = cmd.ExecuteReader()
                Return If(reader IsNot Nothing, ReadEntries(reader), New List(Of MappingEntry)())
            End Using
        End Using
    End Function

    ' Convert a data reader to a list of MappingEntry objects
    Private Function ReadEntries(reader As OleDbDataReader) As List(Of MappingEntry)
        Return Enumerable.Range(0, Integer.MaxValue) _
            .Select(Function(_) If(reader.Read(), reader, Nothing)) _
            .TakeWhile(Function(r) r IsNot Nothing) _
            .Select(AddressOf MapReaderToEntry) _
            .ToList()
    End Function

    ' Map a single record to a MappingEntry
    Private Function MapReaderToEntry(reader As OleDbDataReader) As MappingEntry
        Return New MappingEntry With {
            .Feld = reader("Feld").ToString().Trim(),
            .CDASH = reader("CDASH_Feldname").ToString().Trim(),
            .MC_Wert = reader("MC_Wert").ToString().Trim()
        }
    End Function

End Module