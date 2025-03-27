
# 🧠 FDF Remapper – Stage 3 Version (VB.NET / .NET 2.0)

This is the third-stage version of the FDF Remapper tool — a VB.NET console application for replacing field names in `.fdf` form files using mappings from a local Access database.

In this version, the tool becomes production-grade, supporting value-based replacements, suffix-based mapping, and full logging.

---

## ✅ New Features in Stage 3

- ✅ Full logging, including `[NO CHANGE]` entries for unmatched lines
- ✅ Supports both:
  - **1:1 mappings** (direct tag replacement)
  - **1:N mappings** (based on `Value:` content)
- ✅ Adds support for fields with numeric suffixes:
  - Example: `Field_2` → `MappedField_2`
- ✅ Suffix replacement is restricted to a defined set of base keys:
  - `"Field"`, `"Visit"`, `"AENo"`

---

## 🧾 Sample Log Output

```
Line 10: [1:1] Replaced 'Field' with 'StandardField'
Line 12: [1:N] Replaced 'Visit' with 'VISITID' (Value: Screening)
Line 13: [1:1] Replaced 'AENo_3' with 'AESEQ_3'
Line 14: [NO CHANGE] Tag found in line: Unknown_1
Line 15: [NO CHANGE] no tag in line
```

---

## ⚙️ Usage

### Edit paths and form name in `Program.vb`:

```vbnet
Dim dbPath As String = "C:\Path\To\Mapping.mdb"
Dim fdfFilePath As String = "C:\Input\FormFile.fdf"
Dim outputPath As String = "C:\Output\ModifiedFile.fdf"
Dim formname As String = "FormNameUsedInMapping"
```

### Compile using VB.NET Compiler

```bash
vbc /target:exe /out:FDFRemapper.exe Program.vb
```

---

## 🧠 How It Works

1. Loads all mapping entries for a specific form from Access `.mdb`
2. Parses each line in `.fdf`
3. Replaces matching tags using:
   - Exact key match (1:1)
   - Value match (1:N)
   - Suffix-aware remapping for allowed fields
4. Logs every change, and unmatched tags

---

## 🎯 Suffix Mapping Rules

Only fields that start with these base keys will support `_2`, `_3`, etc.:

- `"Field"`
- `"Visit"`
- `"AENo"`

These must exist in the Access DB **without** the suffix — suffixes are applied dynamically.

---

## 🧭 Roadmap

- [x] 1:1 and 1:N mapping logic
- [x] `[NO CHANGE]` logging
- [x] Suffix-based mapping with whitelist
- [ ] GUI (WinForms)
- [ ] Batch `.fdf` folder processing
- [ ] ASP.NET internal tool

---

## 🧳 Compatibility

- Built with VB.NET (.NET Framework 2.0)
- Works on Windows XP, Windows 7
