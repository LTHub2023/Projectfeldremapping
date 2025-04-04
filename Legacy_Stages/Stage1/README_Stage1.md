
# 🧠 FDF Remapper – Initial Working Version (VB.NET / .NET 2.0)

This is the first working version of a console application that remaps field names in `.fdf` form files using mappings defined in a local Access database (`.mdb`).

It was developed for structured form processing workflows (e.g., clinical trial PDFs, medical case report forms), and supports both 1:1 and 1:N mappings.

---

## ✅ Key Features in This Version

- ✅ Field name mapping via Access `.mdb` file
- ✅ Supports both **1:1** (direct) and **1:N** (value-based) mappings
- ✅ Outputs a log file indicating successful replacements
- ❌ No log entries for unchanged lines yet
- ❌ No support for keys with suffixes (e.g., `FieldName_2`)
- 🖥️ Designed to run on .NET Framework 2.0

---

## 🧾 Sample Log Output

> Note: Lines that are not changed are **not logged** in this version.

```
Line 9: [1:1] Replaced 'FieldA' with 'StandardFieldA'
Line 10: [1:N] Replaced 'FieldB' with 'StandardFieldB' (Value: ValueX)
Line 11: [1:1] Replaced 'FieldC' with 'StandardFieldC'
```

---

## 🧪 How It Works

1. The tool loads a mapping table from a local Access `.mdb` file.
2. It processes the `.fdf` file line by line.
3. If a matching mapping is found:
   - For **1:1**, the field name is replaced directly.
   - For **1:N**, the replacement is based on matching the field’s value.
4. Replacements are logged line by line in a `.log` file.

---

## ⚙️ Usage

### Edit `Program.vb` with correct file paths:

```vbnet
Dim dbPath As String = "C:\YourPath\Mapping.mdb"
Dim fdfFilePath As String = "C:\Input\YourFile.fdf"
Dim outputPath As String = "C:\Output\ModifiedFile.fdf"
Dim formname As String = "FormNameUsedInMapping"
```

### Compile using Visual Studio or command line:

```bash
vbc /target:exe /out:FDFRemapper.exe Program.vb
```

### Run:

The tool will generate:
- A modified `.fdf` file with replaced field names
- A `.log` file showing the lines that were changed

---

## 📌 Roadmap

Planned improvements for future versions:

- [x] Add `[NO CHANGE]` logging
- [x] Add suffix-based key handling (`Field_2`, `Field_3`)
- [ ] GUI support
- [ ] Batch mode for processing multiple `.fdf` files
- [ ] Web version (ASP.NET)

---

## 🧳 Compatibility

- VB.NET (.NET Framework 2.0)
- Works on Windows XP, Windows 7
