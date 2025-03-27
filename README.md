# FDF Remapping Tool

A simple Visual Basic tool for remapping form fields in an FDF (Forms Data Format) file using a Microsoft Access database.

## Features

- Reads settings from an `.ini` file
- Loads field mappings from an Access database
- Remaps FDF field names and values
- Outputs a modified FDF file with updated mappings

## Project Structure

- **`AppConfig.ini`**  
  Paths for the database, FDF input, output file, and target form name.
- **`ConfigIniReader.vb`**  
  Reads and parses the `.ini` file.
- **`FdfProcessor.vb`**  
  Handles FDF reading, parsing, and remapping.
- **`MappingsDatabaseReader.vb`**  
  Connects to the Access database and retrieves the mapping definitions.
- **`MappingEntry.vb`**  
  Data structure representing individual mapping entries.
- **`Program.vb`**  
  Main entry point that ties it all together.

## Configuration

Edit `AppConfig.ini` according to your setup:

```ini
dbPath=databank.mdb
fdfFilePath=file.fdf
outputPath=file_modified.fdf
formName=CoMed_PD
```

## Usage

1. Install all necessary dependencies (like MS Access drivers).
2. Update `AppConfig.ini` with the correct file paths.
3. Build and run the project.
4. A new FDF file is generated at the specified output path.

## License

- Code contributed under [`@kiochan/vbs-fdf-mapping`](https://github.com/kiochan/vbs-fdf-mapping) is provided under the MIT license.
- All remaining code is covered by the original project [owner’s `@LTHub2023/Projectfeldremapping`](https://github.com/LTHub2023/Projectfeldremapping) license.
